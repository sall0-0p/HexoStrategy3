using System.Collections.Concurrent;
using Arch.Buffer;
using Arch.Core;
using SimLib.Core.System.Jobs;
namespace SimLib.Core.System;

public class JobParallelRunner : ISystemRunner
{
    private readonly World _simWorld;
    private readonly World _renderWorld;
    private readonly List<ISimulationSystem> _systems = [];
    private readonly CommandBuffer[] _threadBuffers;

    public JobParallelRunner(World simWorld, World renderWorld)
    {
        _simWorld = simWorld;
        _renderWorld = renderWorld;
        
        var threadCount = Environment.ProcessorCount;
        _threadBuffers = new CommandBuffer[threadCount];
        
        for (var i = 0; i < threadCount; i++)
        {
            _threadBuffers[i] = new CommandBuffer();
        }
    }
    
    public void RegisterSystem(ISimulationSystem system)
    {
        _systems.Add(system);
        system.Initialise(_simWorld);
    }

    public void RunTick()
    {
        foreach (var system in _systems)
        {
            if (system.ShouldRun())
            {
                RunJobParallel(system.CreateJob());
            }
        }
        
        foreach (var buffer in _threadBuffers)
        {
            buffer.Playback(_simWorld, false);
            buffer.Playback(_renderWorld, false);
        }
    }

    private void RunJobParallel(ISystemJob job)
    {
        var queryDesc = job.Query;
        var query = _simWorld.Query(in queryDesc);
        
        var chunks = new List<Chunk>();
        foreach (var chunk in query.GetChunkIterator())
        {
            chunks.Add(chunk);
        }

        var totalChunks = chunks.Count;
        if (totalChunks == 0) return;
        
        var threadCount = _threadBuffers.Length;
        var batchSize = (totalChunks + threadCount - 1) / threadCount;
        
        Parallel.For(0, threadCount, threadIndex =>
        {
            var start = threadIndex * batchSize;
            var end = Math.Min(start + batchSize, totalChunks);
            
            if (start >= totalChunks) return;
            var buffer = _threadBuffers[threadIndex];
            
            var context = new JobContext(threadIndex);
            
            for (var i = start; i < end; i++)
            {
                job.Execute(context, _simWorld, buffer, chunks[i]);
            }
        });
    }
}