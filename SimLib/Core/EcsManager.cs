using Arch.Core;
using SimLib.Api.State;
using SimLib.Api.State.Views;
using SimLib.Modules.Map.Components;

namespace SimLib.Core;

public class EcsManager(World world)
{
    public void RunSystems()
    {
        var provincesQuery = new QueryDescription().WithAll<ProvinceDetails, Terrain>();
        world.Query(in provincesQuery, (Entity entity, ref ProvinceDetails provinceData, ref Terrain terrain) =>
        {
            bool isCoastal = world.Has<IsCoastal>(entity);
            Console.WriteLine("Id: {0}:{1}, Name: {2}, Terrain: {3}, Coastal: {4}", 
                entity.Id, 
                provinceData.ProvinceId, 
                provinceData.Name, 
                terrain.Type, 
                isCoastal
                ); 
        });
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