using SimLib.Api.Data;
using SimLib.Api.Order;
using SimLib.Api.State;
using SimLib.Core;

namespace SimLib.Api.Core;

public class SimCore
{
    private readonly GameDefinition _definition;
    private readonly Simulation _simulation;
    
    public SimCore(GameDefinition definition)
    {
        _definition = definition;
        _simulation = new Simulation(definition);
    }

    public WorldSnapshot TickSimulation(List<IOrder> orders)
    {
        return _simulation.Tick(orders);
    }
}