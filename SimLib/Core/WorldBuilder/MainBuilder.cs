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
            var entity = world.Create(
                new ProvinceDetails(def.ProvinceId, def.Name),
                new Terrain(def.TerrainType)
                );

            // Add if coastal (tag).
            if (def.Coastal)
            {
                world.Add(entity, new IsCoastal());
            }
        }
    }
}