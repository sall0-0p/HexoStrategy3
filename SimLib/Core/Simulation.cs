using Arch.Core;
using SimLib.Api.Data;
using SimLib.Api.Order;
using SimLib.Api.State;
using SimLib.Core.System;

namespace SimLib.Core;

internal class Simulation
{
    private readonly EcsManager _ecs;
    private readonly World _simWorld;
    private readonly World _renderWorld;
    private readonly GameDefinition _definition;
    private int _tickCount = 0;

    public Simulation(GameDefinition definition)
    {
        _definition = definition;
        _simWorld = World.Create();
        _renderWorld = World.Create();
        _ecs = new EcsManager(_simWorld, _renderWorld, typeof(JobParallelRunner));
        
        new WorldBuilder.MainBuilder(_simWorld, _definition).Build();
        new WorldBuilder.MainBuilder(_renderWorld, _definition).Build();
        _ecs.InitSystems();
    }
    
    public WorldState Tick(List<IOrder> orders)
    {
        ApplyOrders(orders);
        
        _ecs.RunSystems();
        _tickCount++;
        
        return CreateSnapshot();
    }

    private void ApplyOrders(List<IOrder> orders)
    {
        
    }

    private WorldState CreateSnapshot()
    {
        return _ecs.ExportState(_tickCount - 1);
    }
}