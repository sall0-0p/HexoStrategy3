using DataLoader;
using SimLib.Api.Core;

var definition = GameLoader.LoadDefinition();
var core = new SimCore(definition);

core.TickSimulation([]);
Console.WriteLine("Done!");