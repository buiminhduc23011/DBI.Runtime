namespace DBI.Runtime.Core.Types;

/// <summary>
/// Represents IEC 61131-3 DWORD data type (32-bit unsigned).
/// Provides bit-level access and byte/word views.
/// </summary>
public struct PlcDWord : IEquatable<PlcDWord>
{
    private uint _value;

    public PlcDWord(uint value) => _value = value;

    public uint Value
    {
        get => _value;
        set => _value = value;
    }

    /// <summary>
    /// Gets or sets a specific bit (0-31).
    /// </summary>
    public bool this[int bit]
    {
        get
        {
            ValidateBitIndex(bit, 32);
            return (_value & (1u << bit)) != 0;
        }
        set
        {
            ValidateBitIndex(bit, 32);
            if (value)
                _value |= 1u << bit;
            else
                _value &= ~(1u << bit);
        }
    }

    /// <summary>
    /// Gets byte at specified position (0-3).
    /// </summary>
    public byte GetByte(int index)
    {
        ValidateByteIndex(index);
        return (byte)((_value >> (index * 8)) & 0xFF);
    }

    /// <summary>
    /// Sets byte at specified position (0-3).
    /// </summary>
    public void SetByte(int index, byte value)
    {
        ValidateByteIndex(index);
        uint mask = ~(0xFFu << (index * 8));
        _value = (_value & mask) | ((uint)value << (index * 8));
    }

    /// <summary>
    /// Gets the low word (bits 0-15).
    /// </summary>
    public PlcWord LowWord => new((ushort)(_value & 0xFFFF));

    /// <summary>
    /// Gets the high word (bits 16-31).
    /// </summary>
    public PlcWord HighWord => new((ushort)((_value >> 16) & 0xFFFF));

    /// <summary>
    /// Sets the low word.
    /// </summary>
    public void SetLowWord(ushort value) => _value = (_value & 0xFFFF0000) | value;

    /// <summary>
    /// Sets the high word.
    /// </summary>
    public void SetHighWord(ushort value) => _value = (_value & 0x0000FFFF) | ((uint)value << 16);

    // Operators
    public static PlcDWord operator &(PlcDWord a, PlcDWord b) => new(a._value & b._value);
    public static PlcDWord operator |(PlcDWord a, PlcDWord b) => new(a._value | b._value);
    public static PlcDWord operator ^(PlcDWord a, PlcDWord b) => new(a._value ^ b._value);
    public static PlcDWord operator ~(PlcDWord a) => new(~a._value);
    public static PlcDWord operator <<(PlcDWord a, int count) => new(a._value << count);
    public static PlcDWord operator >>(PlcDWord a, int count) => new(a._value >> count);

    public static bool operator ==(PlcDWord a, PlcDWord b) => a._value == b._value;
    public static bool operator !=(PlcDWord a, PlcDWord b) => a._value != b._value;

    // Implicit conversions
    public static implicit operator PlcDWord(uint value) => new(value);
    public static implicit operator uint(PlcDWord dword) => dword._value;

    public bool Equals(PlcDWord other) => _value == other._value;
    public override bool Equals(object? obj) => obj is PlcDWord other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => $"DW#16#{_value:X8}";

    private static void ValidateBitIndex(int bit, int maxBits)
    {
        if (bit < 0 || bit >= maxBits)
            throw new ArgumentOutOfRangeException(nameof(bit), $"Bit index must be 0-{maxBits - 1}");
    }

    private static void ValidateByteIndex(int index)
    {
        if (index < 0 || index > 3)
            throw new ArgumentOutOfRangeException(nameof(index), "Byte index must be 0-3");
    }
}
