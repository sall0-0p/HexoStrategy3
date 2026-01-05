using Arch.Core;

namespace SimLib.Core.System;

public interface ISystemRunner
{
    void RegisterSystem(ISimulationSystem system);
    void RunTick();
}