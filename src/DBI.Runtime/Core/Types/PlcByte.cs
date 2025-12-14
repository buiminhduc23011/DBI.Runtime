namespace DBI.Runtime.Core.Types;

/// <summary>
/// Represents IEC 61131-3 BYTE data type (8-bit unsigned).
/// Provides bit-level access.
/// </summary>
public struct PlcByte : IEquatable<PlcByte>
{
    private byte _value;

    public PlcByte(byte value) => _value = value;

    public byte Value
    {
        get => _value;
        set => _value = value;
    }

    /// <summary>
    /// Gets or sets a specific bit (0-7).
    /// </summary>
    public bool this[int bit]
    {
        get
        {
            ValidateBitIndex(bit);
            return (_value & (1 << bit)) != 0;
        }
        set
        {
            ValidateBitIndex(bit);
            if (value)
                _value |= (byte)(1 << bit);
            else
                _value &= (byte)~(1 << bit);
        }
    }

    // Operators
    public static PlcByte operator &(PlcByte a, PlcByte b) => new((byte)(a._value & b._value));
    public static PlcByte operator |(PlcByte a, PlcByte b) => new((byte)(a._value | b._value));
    public static PlcByte operator ^(PlcByte a, PlcByte b) => new((byte)(a._value ^ b._value));
    public static PlcByte operator ~(PlcByte a) => new((byte)~a._value);
    public static PlcByte operator <<(PlcByte a, int count) => new((byte)(a._value << count));
    public static PlcByte operator >>(PlcByte a, int count) => new((byte)(a._value >> count));

    public static bool operator ==(PlcByte a, PlcByte b) => a._value == b._value;
    public static bool operator !=(PlcByte a, PlcByte b) => a._value != b._value;

    // Implicit conversions
    public static implicit operator PlcByte(byte value) => new(value);
    public static implicit operator byte(PlcByte plcByte) => plcByte._value;

    public bool Equals(PlcByte other) => _value == other._value;
    public override bool Equals(object? obj) => obj is PlcByte other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => $"B#16#{_value:X2}";

    private static void ValidateBitIndex(int bit)
    {
        if (bit < 0 || bit > 7)
            throw new ArgumentOutOfRangeException(nameof(bit), "Bit index must be 0-7");
    }
}
