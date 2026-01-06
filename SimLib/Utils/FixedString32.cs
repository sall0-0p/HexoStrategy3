//
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SimLib.Utils;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FixedString32 : IEquatable<FixedString32>
{
    // Serializers will see these 4 fields and save them reliably.
    // 4 * 8 bytes = 32 bytes total.
    public long Part1;
    public long Part2;
    public long Part3;
    public long Part4;

    public string Value
    {
        get => ToString();
        set => SetString(value);
    }

    public static implicit operator string(FixedString32 fs) => fs.Value;
    public static implicit operator FixedString32(string s) => new() { Value = s };

    public override string ToString()
    {
        var span = GetSpan();
        // Find the null terminator or end of string
        int length = span.IndexOf((byte)0);
        if (length < 0) length = 32;
        
        return Encoding.UTF8.GetString(span.Slice(0, length));
    }

    public bool Equals(FixedString32 other)
    {
        return Part1 == other.Part1 && 
               Part2 == other.Part2 && 
               Part3 == other.Part3 && 
               Part4 == other.Part4;
    }

    private void SetString(string? input)
    {
        // Clear memory
        Part1 = 0; Part2 = 0; Part3 = 0; Part4 = 0;

        if (string.IsNullOrEmpty(input)) return;

        var span = GetSpan();
        var sourceBytes = Encoding.UTF8.GetBytes(input);
        var length = Math.Min(sourceBytes.Length, 32);

        sourceBytes.AsSpan(0, length).CopyTo(span);
    }
    
    // Helper to view the 4 longs as a 32-byte span
    private Span<byte> GetSpan()
    {
        return MemoryMarshal.CreateSpan(ref Unsafe.As<long, byte>(ref Part1), 32);
    }

    public override bool Equals(object? obj) => obj is FixedString32 other && Equals(other);
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Part1, Part2, Part3, Part4);
    }
}