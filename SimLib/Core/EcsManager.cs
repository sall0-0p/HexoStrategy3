using System.Runtime.InteropServices;
using Arch.Core;
using SimLib.Api.State;
using SimLib.Core.Hash;
using SimLib.Core.System;
using SimLib.Modules.Map.Systems;

namespace SimLib.Core;

public class EcsManager
{
    private readonly ISystemRunner _systemRunner;
    private readonly HashingService _hashingService;
    private readonly World _simWorld;
    private readonly World _renderWorld;
    private ulong _latestSimHash;

    public EcsManager(World simWorld, World renderWorld, Type runnerType)
    {
        _simWorld = simWorld;
        _renderWorld = renderWorld;
        _systemRunner = (ISystemRunner) Activator.CreateInstance(runnerType, simWorld, renderWorld)!;
        _hashingService = new HashingService();
    }
    
    public void InitSystems()
    {
        _systemRunner.RegisterSystem(new PrinterSystem());
        _systemRunner.RegisterSystem(new PopulationSystem());
    }
    
    public void RunSystems()
    {
        _systemRunner.RunTick();
        SyncRenderWorld();
        RunHashing();
    }

    private void RunHashing()
    {
        var simHash = _hashingService.ComputeWorldHash(_simWorld);
        var renderHash = _hashingService.ComputeWorldHash(_renderWorld);
        Console.WriteLine("Simulation Hash: {0}", simHash);
        Console.WriteLine("Render Hash: {0}", renderHash);
        Console.WriteLine("Are sim and render hash the same? {0}", simHash == renderHash);
        _latestSimHash = simHash;
    }

    private void SyncRenderWorld()
    {
        var originalQuery = _simWorld.Query(new QueryDescription());
        var renderQuery = _renderWorld.Query(new QueryDescription());

        List<Chunk> originalChunks = [];
        List<Chunk> renderChunks = [];
        
        foreach(var chunk in originalQuery.GetChunkIterator()) originalChunks.Add(chunk);
        foreach(var chunk in renderQuery.GetChunkIterator()) renderChunks.Add(chunk);
        
        if (originalChunks.Count != renderChunks.Count) return;

        for (var i = 0; i < originalChunks.Count; i++)
        {
            var sourceChunk = originalChunks[i];
            var destChunk = renderChunks[i];

            var sourceArrays = sourceChunk.Components;
            var destArrays = destChunk.Components;

            for (var arrayIndex = 0; arrayIndex < sourceArrays.Length; arrayIndex++)
            {
                var sourceArray = sourceArrays[arrayIndex];
                var destArray = destArrays[arrayIndex];

                if (sourceArray.Length == 0) continue;
                
                var elementType = sourceArray.GetType().GetElementType()!;
                
                if (!elementType.IsValueType) continue;

                var elementSize = Marshal.SizeOf(elementType);
                var byteCount = sourceArray.Length * elementSize;

                ref var srcRef = ref MemoryMarshal.GetArrayDataReference(sourceArray);
                ref var dstRef = ref MemoryMarshal.GetArrayDataReference(destArray);

                var srcSpan = MemoryMarshal.CreateReadOnlySpan(ref srcRef, byteCount);
                var dstSpan = MemoryMarshal.CreateSpan(ref dstRef, byteCount);

                srcSpan.CopyTo(dstSpan);
            }
        }
    }
    
    public WorldState ExportState(int tickNumber)
    {
        return new WorldState
        {
            TickNumber = tickNumber,
            Checksum = (long) _latestSimHash,
            World = _renderWorld,
        };
    }
}