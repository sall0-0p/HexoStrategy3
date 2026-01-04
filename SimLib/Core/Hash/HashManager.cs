using Arch.Core;

namespace SimLib.Core.Hash;

public readonly record struct ChunkKey(int ArchetypeHash, int ChunkIndex);
public class HashManager
{
    private readonly Dictionary<ChunkKey, ulong> _chunkHashes = [];
    public ulong WorldHash { get; private set; }

    public void SetChunkHash(ChunkKey key, ulong hash)
    {
        _chunkHashes[key] = hash;
    }

    public void RecomputeWorldHash()
    {
        ulong accumulator = 0;
        
        foreach (var hash in _chunkHashes.Values)
        {
            accumulator ^= hash;
        }
        
        WorldHash = accumulator;
    }
}