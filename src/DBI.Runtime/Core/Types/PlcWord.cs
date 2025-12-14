namespace DBI.Runtime.Core.Types;

/// <summary>
/// Represents IEC 61131-3 WORD data type (16-bit unsigned).
/// Provides bit-level access.
/// </summary>
public struct PlcWord : IEquatable<PlcWord>
{
    private ushort _value;

    public PlcWord(ushort value) => _value = value;

    public ushort Value
    {
        get => _value;
        set => _value = value;
    }

    /// <summary>
    /// Gets or sets a specific bit (0-15).
    /// </summary>
    public bool this[int bit]
    {
        get
        {
            ValidateBitIndex(bit, 16);
            return (_value & (1 << bit)) != 0;
        }
        set
        {
            ValidateBitIndex(bit, 16);
            if (value)
                _value |= (ushort)(1 << bit);
            else
                _value &= (ushort)~(1 << bit);
        }
    }

    /// <summary>
    /// Gets the low byte.
    /// </summary>
    public byte LowByte => (byte)(_value & 0xFF);

    /// <summary>
    /// Gets the high byte.
    /// </summary>
    public byte HighByte => (byte)((_value >> 8) & 0xFF);

    /// <summary>
    /// Sets the low byte.
    /// </summary>
    public void SetLowByte(byte value) => _value = (ushort)((_value & 0xFF00) | value);

    /// <summary>
    /// Sets the high byte.
    /// </summary>
    public void SetHighByte(byte value) => _value = (ushort)((_value & 0x00FF) | (value << 8));

    // Operators
    public static PlcWord operator &(PlcWord a, PlcWord b) => new((ushort)(a._value & b._value));
    public static PlcWord operator |(PlcWord a, PlcWord b) => new((ushort)(a._value | b._value));
    public static PlcWord operator ^(PlcWord a, PlcWord b) => new((ushort)(a._value ^ b._value));
    public static PlcWord operator ~(PlcWord a) => new((ushort)~a._value);
    public static PlcWord operator <<(PlcWord a, int count) => new((ushort)(a._value << count));
    public static PlcWord operator >>(PlcWord a, int count) => new((ushort)(a._value >> count));

    public static bool operator ==(PlcWord a, PlcWord b) => a._value == b._value;
    public static bool operator !=(PlcWord a, PlcWord b) => a._value != b._value;

    // Implicit conversions
    public static implicit operator PlcWord(ushort value) => new(value);
    public static implicit operator ushort(PlcWord word) => word._value;

    public bool Equals(PlcWord other) => _value == other._value;
    public override bool Equals(object? obj) => obj is PlcWord other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => $"W#16#{_value:X4}";

    private static void ValidateBitIndex(int bit, int maxBits)
    {
        if (bit < 0 || bit >= maxBits)
            throw new ArgumentOutOfRangeException(nameof(bit), $"Bit index must be 0-{maxBits - 1}");
    }
}
