using Arch.Core;
using SimLib.Api.State;
using SimLib.Api.State.Views;
using SimLib.Core.System;
using SimLib.Modules.Map.Components;
using SimLib.Modules.Map.Systems;

namespace SimLib.Core;

public class EcsManager(World world, Type runnerType)
{
    private ISystemRunner runner;
    
    public void InitSystems()
    {
        runner = (ISystemRunner) Activator.CreateInstance(runnerType, world)!;
        runner.RegisterSystem(new PrinterSystem());
    }
    
    public void RunSystems()
    {
        runner.RunTick(world);
    }

    public WorldSnapshot ExportState(int tickNumber)
    {
        return new WorldSnapshot
        {
            TickNumber = tickNumber,
            Checksum = 0,
            Provinces = ExtractViews((ref ProvinceDetails details) => new ProvinceView
            {
                ProvinceId = details.ProvinceId,
                Name = details.Name,
            })
        };
    }
    
    private List<TView> ExtractViews<TComponent, TView>(ExtractDelegate<TComponent, TView> mapper)
    {
        var result = new List<TView>();
        var query = new QueryDescription().WithAll<TComponent>();

        world.Query(in query, (ref TComponent component) => 
        {
            result.Add(mapper(ref component));
        });
        
        return result;
    }
    
    private delegate TView ExtractDelegate<TComponent, out TView>(ref TComponent component);
}