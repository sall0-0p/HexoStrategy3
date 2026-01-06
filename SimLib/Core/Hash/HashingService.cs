using System.IO.Hashing;
using System.Runtime.InteropServices;
using Arch.Core;

namespace SimLib.Core.Hash;

public readonly record struct ChunkKey(int ArchetypeHash, int ChunkIndex);

public class HashingService()
{
    private readonly XxHash3 _hasher = new();

    public void ComputeWorldHashDebug(World world)
    {
        Dictionary<string, ulong> componentHashes = new();

        var queryDesc = new QueryDescription(); 
        var query = world.Query(in queryDesc);
        
        foreach (var archetype in query.GetArchetypeIterator())
        {
            foreach (var chunk in archetype)
            {
                var componentArrays = chunk.Components;
                foreach (var componentArray in componentArrays)
                {
                    if (componentArray.Length == 0) continue;

                    Type elementType = componentArray.GetType().GetElementType()!;
                    if (!elementType.IsValueType) continue;

                    int elementSize = Marshal.SizeOf(elementType);
                    int validByteLength = chunk.Count * elementSize;
                    
                    ref byte start = ref MemoryMarshal.GetArrayDataReference(componentArray);
                    var bytes = MemoryMarshal.CreateReadOnlySpan(ref start, validByteLength);
                    
                    _hasher.Reset();
                    _hasher.Append(bytes);
                    ulong hash = _hasher.GetCurrentHashAsUInt64();

                    string name = elementType.Name;
                    if (!componentHashes.ContainsKey(name)) componentHashes[name] = 0;
                    componentHashes[name] ^= hash;
                }
            }
        }
        
        Console.WriteLine("--- Hash Breakdown ---");
        foreach (var kvp in componentHashes)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
        Console.WriteLine("----------------------");
    }
        
    public ulong ComputeWorldHash(World world)
    {
        Dictionary<ChunkKey, ulong> chunkHashes = [];
        
        var queryDesc = new QueryDescription(); 
        var query = world.Query(in queryDesc);
        
        foreach (var archetype in query.GetArchetypeIterator())
        {
            var archHash = archetype.GetHashCode();
            int chunkIndex = 0;

            foreach (var chunk in archetype)
            {
                var key = new ChunkKey(archHash, chunkIndex);
                var hash = ComputeChunkHash(chunk);
                
                chunkHashes[key] = hash;
                chunkIndex++;
            }
        }
        
        ulong accumulator = 0;
        
        foreach (var hash in chunkHashes.Values)
        {
            accumulator ^= hash;
        }
        
        return accumulator;
    }
    
    private ulong ComputeChunkHash(Chunk chunk)
    {
        _hasher.Reset();

        var componentArrays = chunk.Components;

        foreach (var componentArray in componentArrays)
        {
            if (componentArray.Length == 0) continue;

            Type elementType = componentArray.GetType().GetElementType()!;
            if (!elementType.IsValueType) continue;
            
            int elementSize = Marshal.SizeOf(elementType);
            if (elementSize == 0) continue;
                
            ref byte start = ref MemoryMarshal.GetArrayDataReference(componentArray);
            int validByteLength = chunk.Count * elementSize;
                
            var bytes = MemoryMarshal.CreateReadOnlySpan(ref start, validByteLength);
            _hasher.Append(bytes);
        }
            
        return _hasher.GetCurrentHashAsUInt64();
    }
}