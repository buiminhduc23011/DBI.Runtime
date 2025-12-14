namespace DBI.Runtime.Core.Memory;

/// <summary>
/// Represents a single byte in PLC memory with bit-level access.
/// Provides %Mx.y style addressing.
/// </summary>
public class MemoryByte
{
    private byte _value;

    /// <summary>
    /// Gets or sets the byte value.
    /// </summary>
    public byte Value
    {
        get => _value;
        set => _value = value;
    }

    /// <summary>
    /// Gets or sets a specific bit (0-7).
    /// Usage: memoryByte[5] for bit 5.
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

    /// <summary>
    /// Sets all bits to 0.
    /// </summary>
    public void Clear() => _value = 0;

    private static void ValidateBitIndex(int bit)
    {
        if (bit < 0 || bit > 7)
            throw new ArgumentOutOfRangeException(nameof(bit), "Bit index must be 0-7");
    }
}
