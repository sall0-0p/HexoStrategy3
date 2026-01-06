using SimLib.Modules.Enums;

namespace SimLib.Modules.Map.Components;

public struct Terrain(TerrainType type)
{
    public TerrainType Type = type;
}