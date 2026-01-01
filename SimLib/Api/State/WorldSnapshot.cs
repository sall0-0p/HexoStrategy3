using SimLib.Api.State.Views;

namespace SimLib.Api.State;

public record WorldSnapshot
{
    public int TickNumber { get; init; }
    public long Checksum { get; init; }
    public List<ProvinceView> Provinces { get; init; }
}