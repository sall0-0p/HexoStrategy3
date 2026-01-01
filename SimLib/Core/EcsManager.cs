using Arch.Core;
using SimLib.Api.State;
using SimLib.Modules.Map.Components;

namespace SimLib.Core;

public class EcsManager(World world)
{
    public void RunSystems()
    {
        var provincesQuery = new QueryDescription().WithAll<ProvinceDetails>();
        world.Query(in provincesQuery, (Entity entity, ref ProvinceDetails provinceData) =>
        {
            Console.WriteLine(entity.Id + ":" + provinceData.ProvinceId + " " + provinceData.Name);
        });
    }

    public WorldSnapshot ExportState()
    {
        return new WorldSnapshot();
    }
}