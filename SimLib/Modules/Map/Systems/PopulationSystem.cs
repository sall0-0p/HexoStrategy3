using Arch.Buffer;
using Arch.Core;
using SimLib.Core.System;
using SimLib.Core.System.Jobs;
using SimLib.Modules.Map.Components;

namespace SimLib.Modules.Map.Systems;

public struct PopulationGrowthJob : IJob<Population> 
{
    public void Execute(JobContext context, World world, Entity entity, CommandBuffer buffer, ref Population population)
    {
        population.Quantity += population.Growth;
    }
}

public class PopulationSystem : ISimulationSystem
{
    public void Initialise(World world)
    {
        
    }

    public bool ShouldRun()
    {
        return true;
    }

    public ISystemJob CreateJob()
    {
        return new PopulationGrowthJob();
    }
}