using Arch.Buffer;
using Arch.Core;
using SimLib.Core.System;
using SimLib.Core.System.Jobs;
using SimLib.Modules.Map.Components;

namespace SimLib.Modules.Map.Systems;

public struct PrinterJob() : IJob<ProvinceDetails> 
{
    public void Execute(JobContext context, World world, CommandBuffer buffer, ref ProvinceDetails details)
    {
        Console.WriteLine("{0}: {1} (t: {2})", details.ProvinceId, details.Name, context.ThreadIndex);
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