using DataLoader;
using SimLib.Api.Core;

var definition = GameLoader.LoadDefinition();
var core1 = new SimCore(definition);

Console.WriteLine("Tick in main sim!!");
core1.TickSimulation([]);

Console.WriteLine("------ COMPARE HERE!");
core1.TickSimulation([]);

var core2 = new SimCore(definition);

core2.TickSimulation([]);

var save = core2.GetSave();

var core3 = new SimCore(definition);
core3.LoadSave(save);
Console.WriteLine("------ COMPARE HERE!");
core3.TickSimulation([]);
