using System.Collections.Concurrent;
using Arch.Buffer;
using Arch.Core;
using SimLib.Core.System.Jobs;
namespace SimLib.Core.System;

public class JobParallelRunner : ISystemRunner
{
    private readonly World _world;
    private readonly List<ISimulationSystem> _systems = [];
    private readonly CommandBuffer[] _threadBuffers;
    private readonly ConcurrentBag<Chunk> _dirtyChunks = [];

    public JobParallelRunner(World world)
    {
        _world = world;
        
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
        system.Initialise(_world);
    }

    public void RunTick(World world)
    {
        _dirtyChunks.Clear();
        
        foreach (var system in _systems)
        {
            if (system.ShouldRun())
            {
                RunJobParallel(system.CreateJob());
            }
        }
        
        foreach (var buffer in _threadBuffers)
        {
            buffer.Playback(_world);
        }
    }

    public IEnumerable<Chunk> GetDirtyChunks()
    {
        return _dirtyChunks;
    }

    public void ClearDirtyChunks()
    {
        _dirtyChunks.Clear();
    }

    private void RunJobParallel(ISystemJob job)
    {
        var queryDesc = job.Query;
        var query = _world.Query(in queryDesc);
        
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
                var chunk = chunks[i];
                job.Execute(context, _world, buffer, chunks[i]);
                
                _dirtyChunks.Add(chunk);
            }
        });
    }
}