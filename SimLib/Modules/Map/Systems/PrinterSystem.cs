using Arch.Buffer;
using Arch.Core;
using SimLib.Core.System;
using SimLib.Core.System.Jobs;
using SimLib.Modules.Map.Components;

namespace SimLib.Modules.Map.Systems;

public struct PrinterJob() : ISystemJob
{
    public QueryDescription Query { get; } = new QueryDescription().WithAll<ProvinceDetails>();
 
    public void Execute(JobContext context, World world, CommandBuffer buffer, Chunk chunk)
    {
        var entities = chunk.Entities;
        Console.WriteLine("There are {0} entities in a chunk!", entities.Length);

        var provinces = chunk.GetArray<ProvinceDetails>();
        
        foreach (var details in provinces)
        {
            Console.WriteLine("{0}: {1}", details.ProvinceId, details.Name);
        }
    }
}

public class PrinterSystem : ISimulationSystem
{
    public void Initialise(World world)
    {
        Console.WriteLine("System initialised!");
    }

    public bool ShouldRun()
    {
        return true;
    }

    public ISystemJob CreateJob()
    {
        return new PrinterJob();
    }
}