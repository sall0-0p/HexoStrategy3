using Arch.Buffer;
using Arch.Core;

namespace SimLib.Core.System.Jobs;

public interface ISystemJob
{
    QueryDescription Query { get; }

    bool Execute(JobContext context, World world, CommandBuffer buffer, Chunk chunk);
}