using Arch.Buffer;
using Arch.Core;
using SimLib.Core.System;
using SimLib.Core.System.Jobs;
using SimLib.Modules.Map.Components;
using SimLib.Modules.Misc.Components;

namespace SimLib.Modules.Map.Systems;

public struct PrinterJob : IJob<ProvinceDetails> 
{
    public void Execute(JobContext context, World world, Entity entity, CommandBuffer buffer, ref ProvinceDetails details)
    {
        string name;
        
        if (world.Has<LocalisedName>(entity))
        {
            name = world.Get<LocalisedName>(entity).Id.ToString(); // TODO: CHANGE LATER TO ACTUAL LOCALISATION.
        }
        else
        {
            name = world.Get<CustomName>(entity).Value;
        }
        
        // Console.WriteLine("{0}: {1} (t: {2})", details.ProvinceId, name, context.ThreadIndex);
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