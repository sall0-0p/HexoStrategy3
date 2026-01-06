using DataLoader;
using SimLib.Api.Core;

namespace SimTest.Integration;

[TestFixture]
public class SaveLoadTest
{
    [Test]
    public void SaveLoad_ShouldPreserveDeterminism_OnRealData()
    {
        // 1. ARRANGE: Load the REAL game definition from your CSV
        // This ensures we are testing with all actual provinces and components.
        var definition = GameLoader.LoadDefinition();

        // 2. ACT: Run the "Control" simulation (2 ticks straight)
        var coreControl = new SimCore(definition);
        coreControl.TickSimulation([]); // Tick 1
        var stateControl = coreControl.TickSimulation([]); // Tick 2

        // 3. ACT: Run the "Save/Load" simulation
        // Step A: Run 1 tick and save
        var coreBeforeSave = new SimCore(definition);
        coreBeforeSave.TickSimulation([]); // Tick 1
        var saveState = coreBeforeSave.GetSave();

        // Step B: Load into a fresh core and run tick 2
        var coreAfterLoad = new SimCore(definition);
        coreAfterLoad.LoadSave(saveState);
        var stateLoaded = coreAfterLoad.TickSimulation([]); // Tick 2

        // 4. ASSERT: The world state hashes must match exactly
        // If a component (like IsCoastal or CustomName) is not saving correctly,
        // the hash in 'stateLoaded' will differ from 'stateControl', failing the test.
        Assert.That(stateLoaded.Checksum, Is.EqualTo(stateControl.Checksum), 
            "The simulation state after loading from save does not match the continuous simulation.");
    }
}