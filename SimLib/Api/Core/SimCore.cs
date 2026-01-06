using SimLib.Api.Data;
using SimLib.Api.Order;
using SimLib.Api.Save;
using SimLib.Api.State;
using SimLib.Core;

namespace SimLib.Api.Core;

public class SimCore(GameDefinition definition)
{
    private readonly GameDefinition _definition = definition;
    private readonly Simulation _simulation = new Simulation(definition);

    public WorldState TickSimulation(List<IOrder> orders)
    {
        return _simulation.Tick(orders);
    }

    public SaveState GetSave()
    {
        return _simulation.GetSave();
    }

    public void LoadSave(SaveState saveState)
    {
        _simulation.LoadSave(saveState);
    }
}