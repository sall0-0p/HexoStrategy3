using Arch.Core;
using SimLib.Api.Data;
using SimLib.Api.Order;
using SimLib.Api.State;

namespace SimLib.Core;

internal class Simulation
{
    private readonly EcsManager _ecs;
    private readonly World _world;
    private readonly GameDefinition _definition;
    private int _tickCount = 0;

    public Simulation(GameDefinition definition)
    {
        _definition = definition;
        _world = World.Create();
        _ecs = new EcsManager(_world);
        
        new WorldBuilder.MainBuilder(_world, _definition).Build();
    }
    
    public WorldSnapshot Tick(List<IOrder> orders)
    {
        ApplyOrders(orders);
        
        _ecs.RunSystems();
        _tickCount++;
        
        return CreateSnapshot();
    }

    private void ApplyOrders(List<IOrder> orders)
    {
        
    }

    private WorldSnapshot CreateSnapshot()
    {
        return _ecs.ExportState(_tickCount - 1);
    }
}