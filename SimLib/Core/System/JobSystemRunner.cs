using System.Collections.Concurrent;
using Arch.Buffer;
using Arch.Core;
using SimLib.Core.System.Jobs;

namespace SimLib.Core.System;

public class JobSystemRunner : ISystemRunner
{
    private readonly World _world;
    private readonly List<ISimulationSystem> _systems = [];
    
    public JobSystemRunner(World world)
    {
        _world = world;
    }
    
    public void RegisterSystem(ISimulationSystem system)
    {
        _systems.Add(system);
        system.Initialise(_world);
    }

    public void RunTick(World world)
    {
        var jobsToRun = new List<Jobs.ISystemJob>();

        foreach (var system in _systems)
        {
            if (system.ShouldRun())
            {
                jobsToRun.Add(system.CreateJob());
            }
        }

        foreach (var job in jobsToRun)
        {
            RunJobParallel(job);
        }
    }

    private void RunJobParallel(ISystemJob job)
    {
        var chunks = new List<Chunk>();
        var queryDesc = job.Query;
        var query = _world.Query(in queryDesc);

        foreach (var c in query.GetChunkIterator())
        {
            chunks.Add(c);
        }
        
        var usedBuffers = new ConcurrentBag<CommandBuffer>();
        Parallel.ForEach(
            chunks, 
            () => new CommandBuffer(), 
            (chunk, loopState, localBuffer) => 
            {
                var context = new JobContext(Environment.CurrentManagedThreadId);
                job.Execute(context, _world, localBuffer, chunk);
            
                return localBuffer;
            },
            
            (finalBuffer) => 
            {
                usedBuffers.Add(finalBuffer);
            }
        );
        
        foreach (var buffer in usedBuffers)
        {
            buffer.Playback(_world);
        }
    }
}