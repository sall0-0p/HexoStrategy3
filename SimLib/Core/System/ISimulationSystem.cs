using Arch.Core;

namespace SimLib.Core.System;

public interface ISimulationSystem
{
    void Initialise(World world);

    bool ShouldRun();

    Jobs.ISystemJob CreateJob();
}