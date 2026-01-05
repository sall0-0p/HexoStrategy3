using Arch.Core;

namespace SimLib.Api.State;

public record WorldState
{
    public int TickNumber { get; init; }
    public long Checksum { get; init; }
    public World World { get; init; }
}