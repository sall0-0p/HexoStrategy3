using SimLib.Utils;

namespace SimLib.Modules.Misc.Components;

public struct LocalisedName(int id)
{
    public int Id = id;
}

public struct CustomName
{
    public FixedString32 Value;
    
    public CustomName(string name)
    {
        var initial = new FixedString32();
        initial.Value = name;
        Value = initial;
    }
}