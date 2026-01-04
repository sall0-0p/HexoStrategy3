using Arch.Buffer;
using Arch.Core;
using SimLib.Core.System;
using SimLib.Core.System.Jobs;
using SimLib.Modules.Map.Components;

namespace SimLib.Modules.Map.Systems;

public struct PrinterJob : IJob<ProvinceDetails> 
{
    public bool Execute(JobContext context, World world, CommandBuffer buffer, ref ProvinceDetails details)
    {
        Console.WriteLine("{0} (t: {1})", details.ProvinceId, context.ThreadIndex);
        return false;
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