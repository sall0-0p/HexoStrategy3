using SimLib.Modules.Enums;

namespace SimLib.Modules.Map.Components;

public struct Terrain(TerrainType terrainType)
{
    public TerrainType Type = terrainType;
}