using System.Runtime.InteropServices;
using System.Text;

namespace SimLib.Utils;

public struct FixedString32 : IEquatable<FixedString32>
{
    private unsafe fixed byte _buffer[32];
    
    public string Value
    {
        get => ToString();
        set => SetString(value);
    }
    
    public static implicit operator string(FixedString32 fs) => fs.Value;
    public static implicit operator FixedString32(string s) => new() { Value = s };
    
    public override string ToString()
    {
        unsafe
        {
            var span = MemoryMarshal.CreateReadOnlySpan(ref _buffer[0], 32);
            return Encoding.UTF8.GetString(span).TrimEnd('\0');
        }
    }
    
    public bool Equals(FixedString32 other)
    {
        unsafe
        {
            var a = MemoryMarshal.CreateReadOnlySpan(ref _buffer[0], 32);
            var b = MemoryMarshal.CreateReadOnlySpan(ref other._buffer[0], 32);
            return a.SequenceEqual(b);
        }
    }
    
    private void SetString(string? input)
    {
        unsafe
        {
            var destination = MemoryMarshal.CreateSpan(ref _buffer[0], 32);
            
            destination.Clear();

            if (!string.IsNullOrEmpty(input))
            {
                var sourceBytes = Encoding.UTF8.GetBytes(input);
                var length = Math.Min(sourceBytes.Length, 32);
                
                // Copy new data
                sourceBytes.AsSpan(0, length).CopyTo(destination);
            }
        }
    }
    
    public void Clear()
    {
        unsafe
        {
            var span = MemoryMarshal.CreateSpan(ref _buffer[0], 32);
            span.Clear();
        }
    }
    
    public override bool Equals(object? obj) => obj is FixedString32 other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
}