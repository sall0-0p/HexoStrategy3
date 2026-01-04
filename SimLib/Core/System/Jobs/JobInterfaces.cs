using Arch.Buffer;
using Arch.Core;

namespace SimLib.Core.System.Jobs;

public interface IJob<T> : ISystemJob
{
    QueryDescription ISystemJob.Query => new QueryDescription().WithAll<T>();
    bool Execute(JobContext context, World world, CommandBuffer buffer, ref T component);
    
    bool ISystemJob.Execute(JobContext context, World world, CommandBuffer buffer, Chunk chunk)
    {
        var span = chunk.GetArray<T>().AsSpan(0, chunk.Count);
        var chunkDirty = false;
        
        for (var i = 0; i < chunk.Count; i++)
        {
            chunkDirty |= Execute(context, world, buffer, ref span[i]);
        }
        
        return chunkDirty;
    }
}

public interface IJob<T1, T2> : ISystemJob
{
    QueryDescription ISystemJob.Query => new QueryDescription().WithAll<T1, T2>();

    bool Execute(JobContext context, World world, CommandBuffer buffer, ref T1 c1, ref T2 c2);

    bool ISystemJob.Execute(JobContext context, World world, CommandBuffer buffer, Chunk chunk)
    {
        var span1 = chunk.GetArray<T1>().AsSpan(0, chunk.Count);
        var span2 = chunk.GetArray<T2>().AsSpan(0, chunk.Count);
        var chunkDirty = false;

        for (var i = 0; i < chunk.Count; i++)
        {
            chunkDirty |= Execute(context, world, buffer, ref span1[i], ref span2[i]);
        }

        return chunkDirty;
    }
}

public interface IJob<T1, T2, T3> : ISystemJob
{
    QueryDescription ISystemJob.Query => new QueryDescription().WithAll<T1, T2, T3>();

    bool Execute(JobContext context, World world, CommandBuffer buffer, ref T1 c1, ref T2 c2, ref T3 c3);

    bool ISystemJob.Execute(JobContext context, World world, CommandBuffer buffer, Chunk chunk)
    {
        var span1 = chunk.GetArray<T1>().AsSpan(0, chunk.Count);
        var span2 = chunk.GetArray<T2>().AsSpan(0, chunk.Count);
        var span3 = chunk.GetArray<T3>().AsSpan(0, chunk.Count);
        var chunkDirty = false;

        for (var i = 0; i < chunk.Count; i++)
        {
            chunkDirty |= Execute(context, world, buffer, ref span1[i], ref span2[i], ref span3[i]);
        }

        return chunkDirty;
    }
}