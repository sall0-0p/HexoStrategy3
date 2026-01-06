using Arch.Core;
using Arch.Persistence;
using SimLib.Api.Data;
using SimLib.Api.Order;
using SimLib.Api.Save;
using SimLib.Api.State;
using SimLib.Core.System;

namespace SimLib.Core;

internal class Simulation
{
    private readonly GameDefinition _definition;
    private EcsManager _ecs;
    private World _simWorld;
    private World _renderWorld;
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
    
    // Saving / loading of state;
    public SaveState GetSave()
    {
        var serializer = new ArchBinarySerializer([]);

        return new SaveState()
        {
            LastTick = _tickCount,
            World = serializer.Serialize(_simWorld),
        };
    }

    public void LoadSave(SaveState save)
    {
        var serializer = new ArchBinarySerializer();
        _simWorld.Dispose();
        _renderWorld.Dispose();

        _tickCount = save.LastTick;
        _simWorld = serializer.Deserialize(save.World);
        _renderWorld = serializer.Deserialize(save.World);
        
        _ecs = new EcsManager(_simWorld, _renderWorld, typeof(JobParallelRunner));
        _ecs.InitSystems();
    }
}