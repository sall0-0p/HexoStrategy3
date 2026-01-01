using SimLib.Api.Data;

namespace DataLoader.Map;

internal class MapParser
{
    public static List<ProvinceDefinition> LoadProvinces()
    { 
        var list = new List<ProvinceDefinition>();
        
        list.Add(new ProvinceDefinition
        {
            Name = "Lutsk",
            ProvinceId = 1,
        });
        
        list.Add(new ProvinceDefinition
        {
            Name = "Lviv",
            ProvinceId = 2,
        });
        
        list.Add(new ProvinceDefinition
        {
            Name = "Rivne",
            ProvinceId = 3,
        });

        return list;
    }
}