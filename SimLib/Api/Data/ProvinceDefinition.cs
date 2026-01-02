using SimLib.Modules.Enums;

namespace SimLib.Api.Data;

public record ProvinceDefinition
{
    public int ProvinceId { get; init; }
    public string Name { get; init; }
    public TerrainType TerrainType { get; init; }
    public bool Coastal { get; init; }
}