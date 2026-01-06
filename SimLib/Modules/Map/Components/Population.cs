namespace SimLib.Modules.Map.Components;

public struct Population(int quantity, int growth)
{
    public int Quantity = quantity;
    public int Growth = growth;
}