using System.Globalization;
using CsvHelper;
using SimLib.Api.Data;
using SimLib.Modules.Enums;

namespace DataLoader.Map;

internal class MapParser
{
    public static List<ProvinceDefinition> LoadProvinces()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var path = Path.Combine(home, "Projects/RiderProjects/StrategyLib-Db/Assets/provinces.csv");
        
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<ProvinceDef>();
        var list = new List<ProvinceDefinition>();
        foreach (ProvinceDef def in records)
        {
            TerrainType terrain;
            if (Enum.TryParse(def.Terrain, out TerrainType result))
            {
                terrain = result;
            }
            else 
            {
                Console.WriteLine($"Invalid terrain: {def.Terrain}");
                terrain = TerrainType.Plains;
            }
            
            list.Add(new ProvinceDefinition
            {
                Name = def.Name,
                ProvinceId = def.Id,
                TerrainType = terrain,
                Coastal = def.Coastal,
            });
        }

        return list;
    }
    
    private class ProvinceDef
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Terrain { get; set; }
        public bool Coastal { get; set; }
    }
}