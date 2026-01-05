namespace SimLib.Modules.Map.Components;

public struct Population(int startingValue, int growthPerTick)
{
    public int Quantity = startingValue;
    public int Growth = growthPerTick;
}