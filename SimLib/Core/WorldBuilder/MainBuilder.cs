using Arch.Core;
using SimLib.Api.Data;
using SimLib.Modules.Map.Components;

namespace SimLib.Core.WorldBuilder;

public class MainBuilder(World world, GameDefinition definition)
{
    public void Build()
    {
        BuildProvinces();
    }

    private void BuildProvinces()
    {
        foreach (var def in definition.Provinces)
        {
            world.Create(new ProvinceDetails
            {
                Name = def.Name,
                ProvinceId = def.ProvinceId,
            });
        }
    }
}