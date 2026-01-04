using System.IO.Hashing;
using System.Runtime.InteropServices;
using Arch.Core;

namespace SimLib.Core.Hash;

public class HashingService(HashManager hashManager)
{
    private readonly XxHash3 _hasher = new();

    public void ProcessDirtyChunks(World world, HashSet<Chunk> dirtyChunks)
    {
        var queryDesc = new QueryDescription(); 
        var query = world.Query(in queryDesc);
        
        foreach (var archetype in query.GetArchetypeIterator())
        {
            var archHash = archetype.GetHashCode();
            int chunkIndex = 0;

            foreach (var chunk in archetype)
            {
                if (dirtyChunks.Contains(chunk))
                {
                    var key = new ChunkKey(archHash, chunkIndex);
                    var hash = ComputeChunkHash(chunk);
                    
                    hashManager.SetChunkHash(key, hash);
                }

                chunkIndex++;
            }
        }
    }
    
    private ulong ComputeChunkHash(Chunk chunk)
    {
        _hasher.Reset();

        var componentArrays = chunk.Components;

        foreach (var componentArray in componentArrays)
        {
            if (componentArray.Length == 0) continue;

            Type elementType = componentArray.GetType().GetElementType()!;
            if (!elementType.IsValueType)
            {
                Console.WriteLine("WARNING! NON VALUE TYPE DETECTED IN COMPONENT {0}", elementType);
                continue;
            };

            int elementSize = Marshal.SizeOf(elementType);
            if (elementSize == 0) continue;
                
            ref byte start = ref MemoryMarshal.GetArrayDataReference(componentArray);
            int totalByteLength = componentArray.Length * elementSize;
                
            var bytes = MemoryMarshal.CreateReadOnlySpan(ref start, totalByteLength);
            _hasher.Append(bytes);
        }
            
        return _hasher.GetCurrentHashAsUInt64();
    }
}