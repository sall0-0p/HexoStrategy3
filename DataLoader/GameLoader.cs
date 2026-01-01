using DataLoader.Map;
using SimLib.Api.Data;

namespace DataLoader;

public class GameLoader
{
    public static GameDefinition LoadDefinition()
    {
        return new GameDefinition 
        {
            Provinces = MapParser.LoadProvinces()
        };
    }
}