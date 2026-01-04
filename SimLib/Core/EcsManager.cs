using Arch.Core;
using SimLib.Api.State;
using SimLib.Api.State.Views;
using SimLib.Core.Hash;
using SimLib.Core.System;
using SimLib.Modules.Map.Components;
using SimLib.Modules.Map.Systems;

namespace SimLib.Core;

public class EcsManager
{
    private readonly ISystemRunner _systemRunner;
    private readonly HashManager _hashManager;
    private readonly HashingService _hashingService;
    private readonly World _world;

    public EcsManager(World world, Type runnerType)
    {
        _world = world;
        _systemRunner = (ISystemRunner) Activator.CreateInstance(runnerType, world)!;
        _hashManager = new HashManager();
        _hashingService = new HashingService(_hashManager);
    }
    
    public void InitSystems()
    {
        _systemRunner.RegisterSystem(new PrinterSystem());
    }
    
    public void RunSystems()
    {
        _systemRunner.RunTick(_world);
        RunHashing();
    }

    private void RunHashing()
    {
        var dirtyChunks = _systemRunner.GetDirtyChunks();

        if (dirtyChunks.Any())
        {
            _hashingService.ProcessDirtyChunks(_world, new HashSet<Chunk>(dirtyChunks));
            _systemRunner.ClearDirtyChunks();
        }
        
        _hashManager.RecomputeWorldHash();
        Console.WriteLine(_hashManager.WorldHash);
    }
    
    public WorldSnapshot ExportState(int tickNumber)
    {
        return new WorldSnapshot
        {
            TickNumber = tickNumber,
            Checksum = (long) _hashManager.WorldHash,
            Provinces = ExtractViews((ref ProvinceDetails details) => new ProvinceView
            {
                ProvinceId = details.ProvinceId,
            })
        };
    }
    
    private List<TView> ExtractViews<TComponent, TView>(ExtractDelegate<TComponent, TView> mapper)
    {
        var result = new List<TView>();
        var query = new QueryDescription().WithAll<TComponent>();

        _world.Query(in query, (ref TComponent component) => 
        {
            result.Add(mapper(ref component));
        });
        
        return result;
    }
    
    private delegate TView ExtractDelegate<TComponent, out TView>(ref TComponent component);
}