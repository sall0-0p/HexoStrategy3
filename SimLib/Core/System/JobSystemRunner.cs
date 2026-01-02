using Arch.Buffer;
using Arch.Core;
using SimLib.Core.System.Jobs;

namespace SimLib.Core.System;

public class JobSystemRunner : ISystemRunner
{
    private readonly World _world;
    private readonly List<ISimulationSystem> _systems = [];
    private readonly CommandBuffer[] _threadBuffers;
    
    public JobSystemRunner(World world)
    {
        _world = world;

        int threadCount = Environment.ProcessorCount;
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

        foreach (var buffer in _threadBuffers)
        {
            buffer.Playback(_world);
        }
    }

    private void RunJobParallel(ISystemJob job)
    {
        var queryDesc = job.Query;
        var query = _world.Query(in queryDesc);

        foreach (var chunk in query.GetChunkIterator())
        {
            var threadId = Environment.CurrentManagedThreadId % _threadBuffers.Length;
            var buffer = _threadBuffers[threadId];
            var context = new JobContext(threadId);
            
            job.Execute(context, _world, buffer, chunk);
        }
    }
}