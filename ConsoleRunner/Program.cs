using DataLoader;
using SimLib.Api.Core;

var definition = GameLoader.LoadDefinition();
var core = new SimCore(definition);

for (var i = 0; i < 100; i++)
{
    Console.WriteLine("Tick {0}!", i);
    core.TickSimulation([]);
}