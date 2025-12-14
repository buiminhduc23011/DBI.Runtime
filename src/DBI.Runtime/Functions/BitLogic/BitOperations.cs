namespace DBI.Runtime.Functions.BitLogic;

/// <summary>
/// Bit-level operations for PLC logic.
/// Implements IEC 61131-3 and Siemens bit operations.
/// </summary>
public static class PlcBitLogic
{
    #region Logical Operations - Byte

    /// <summary>
    /// Bitwise AND.
    /// </summary>
    public static byte AND(byte a, byte b) => (byte)(a & b);

    /// <summary>
    /// Bitwise OR.
    /// </summary>
    public static byte OR(byte a, byte b) => (byte)(a | b);

    /// <summary>
    /// Bitwise XOR.
    /// </summary>
    public static byte XOR(byte a, byte b) => (byte)(a ^ b);

    /// <summary>
    /// Bitwise NOT.
    /// </summary>
    public static byte NOT(byte a) => (byte)~a;

    #endregion

    #region Logical Operations - Word (ushort)

    /// <summary>
    /// Bitwise AND.
    /// </summary>
    public static ushort AND(ushort a, ushort b) => (ushort)(a & b);

    /// <summary>
    /// Bitwise OR.
    /// </summary>
    public static ushort OR(ushort a, ushort b) => (ushort)(a | b);

    /// <summary>
    /// Bitwise XOR.
    /// </summary>
    public static ushort XOR(ushort a, ushort b) => (ushort)(a ^ b);

    /// <summary>
    /// Bitwise NOT.
    /// </summary>
    public static ushort NOT(ushort a) => (ushort)~a;

    #endregion

    #region Logical Operations - DWord (uint)

    /// <summary>
    /// Bitwise AND.
    /// </summary>
    public static uint AND(uint a, uint b) => a & b;

    /// <summary>
    /// Bitwise OR.
    /// </summary>
    public static uint OR(uint a, uint b) => a | b;

    /// <summary>
    /// Bitwise XOR.
    /// </summary>
    public static uint XOR(uint a, uint b) => a ^ b;

    /// <summary>
    /// Bitwise NOT.
    /// </summary>
    public static uint NOT(uint a) => ~a;

    #endregion

    #region Shift Operations - Byte

    /// <summary>
    /// Shift left.
    /// </summary>
    public static byte SHL(byte value, int count) => (byte)(value << count);

    /// <summary>
    /// Shift right.
    /// </summary>
    public static byte SHR(byte value, int count) => (byte)(value >> count);

    /// <summary>
    /// Rotate left.
    /// </summary>
    public static byte ROL(byte value, int count)
    {
        count &= 7; // Modulo 8
        return (byte)((value << count) | (value >> (8 - count)));
    }

    /// <summary>
    /// Rotate right.
    /// </summary>
    public static byte ROR(byte value, int count)
    {
        count &= 7;
        return (byte)((value >> count) | (value << (8 - count)));
    }

    #endregion

    #region Shift Operations - Word

    /// <summary>
    /// Shift left.
    /// </summary>
    public static ushort SHL(ushort value, int count) => (ushort)(value << count);

    /// <summary>
    /// Shift right.
    /// </summary>
    public static ushort SHR(ushort value, int count) => (ushort)(value >> count);

    /// <summary>
    /// Rotate left.
    /// </summary>
    public static ushort ROL(ushort value, int count)
    {
        count &= 15;
        return (ushort)((value << count) | (value >> (16 - count)));
    }

    /// <summary>
    /// Rotate right.
    /// </summary>
    public static ushort ROR(ushort value, int count)
    {
        count &= 15;
        return (ushort)((value >> count) | (value << (16 - count)));
    }

    #endregion

    #region Shift Operations - DWord

    /// <summary>
    /// Shift left.
    /// </summary>
    public static uint SHL(uint value, int count) => value << count;

    /// <summary>
    /// Shift right.
    /// </summary>
    public static uint SHR(uint value, int count) => value >> count;

    /// <summary>
    /// Rotate left.
    /// </summary>
    public static uint ROL(uint value, int count)
    {
        count &= 31;
        return (value << count) | (value >> (32 - count));
    }

    /// <summary>
    /// Rotate right.
    /// </summary>
    public static uint ROR(uint value, int count)
    {
        count &= 31;
        return (value >> count) | (value << (32 - count));
    }

    #endregion

    #region Bit Manipulation

    /// <summary>
    /// Tests if a specific bit is set.
    /// </summary>
    public static bool TEST_BIT(byte value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 8);
        return (value & (1 << bitNumber)) != 0;
    }

    /// <summary>
    /// Tests if a specific bit is set.
    /// </summary>
    public static bool TEST_BIT(ushort value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 16);
        return (value & (1 << bitNumber)) != 0;
    }

    /// <summary>
    /// Tests if a specific bit is set.
    /// </summary>
    public static bool TEST_BIT(uint value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 32);
        return (value & (1u << bitNumber)) != 0;
    }

    /// <summary>
    /// Sets a specific bit to 1.
    /// </summary>
    public static byte SET_BIT(byte value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 8);
        return (byte)(value | (1 << bitNumber));
    }

    /// <summary>
    /// Sets a specific bit to 1.
    /// </summary>
    public static ushort SET_BIT(ushort value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 16);
        return (ushort)(value | (1 << bitNumber));
    }

    /// <summary>
    /// Sets a specific bit to 1.
    /// </summary>
    public static uint SET_BIT(uint value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 32);
        return value | (1u << bitNumber);
    }

    /// <summary>
    /// Resets (clears) a specific bit to 0.
    /// </summary>
    public static byte RESET_BIT(byte value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 8);
        return (byte)(value & ~(1 << bitNumber));
    }

    /// <summary>
    /// Resets (clears) a specific bit to 0.
    /// </summary>
    public static ushort RESET_BIT(ushort value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 16);
        return (ushort)(value & ~(1 << bitNumber));
    }

    /// <summary>
    /// Resets (clears) a specific bit to 0.
    /// </summary>
    public static uint RESET_BIT(uint value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 32);
        return value & ~(1u << bitNumber);
    }

    /// <summary>
    /// Toggles a specific bit.
    /// </summary>
    public static byte TOGGLE_BIT(byte value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 8);
        return (byte)(value ^ (1 << bitNumber));
    }

    /// <summary>
    /// Toggles a specific bit.
    /// </summary>
    public static ushort TOGGLE_BIT(ushort value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 16);
        return (ushort)(value ^ (1 << bitNumber));
    }

    /// <summary>
    /// Toggles a specific bit.
    /// </summary>
    public static uint TOGGLE_BIT(uint value, int bitNumber)
    {
        ValidateBitNumber(bitNumber, 32);
        return value ^ (1u << bitNumber);
    }

    /// <summary>
    /// Sets a bit to a specific value.
    /// </summary>
    public static byte WRITE_BIT(byte value, int bitNumber, bool bitValue)
    {
        return bitValue ? SET_BIT(value, bitNumber) : RESET_BIT(value, bitNumber);
    }

    /// <summary>
    /// Sets a bit to a specific value.
    /// </summary>
    public static ushort WRITE_BIT(ushort value, int bitNumber, bool bitValue)
    {
        return bitValue ? SET_BIT(value, bitNumber) : RESET_BIT(value, bitNumber);
    }

    /// <summary>
    /// Sets a bit to a specific value.
    /// </summary>
    public static uint WRITE_BIT(uint value, int bitNumber, bool bitValue)
    {
        return bitValue ? SET_BIT(value, bitNumber) : RESET_BIT(value, bitNumber);
    }

    #endregion

    #region Byte Swapping

    /// <summary>
    /// Swaps bytes in a word (big-endian to little-endian or vice versa).
    /// </summary>
    public static ushort SWAP(ushort value)
    {
        return (ushort)((value << 8) | (value >> 8));
    }

    /// <summary>
    /// Swaps bytes in a dword.
    /// </summary>
    public static uint SWAP(uint value)
    {
        return ((value & 0x000000FF) << 24) |
               ((value & 0x0000FF00) << 8) |
               ((value & 0x00FF0000) >> 8) |
               ((value & 0xFF000000) >> 24);
    }

    #endregion

    private static void ValidateBitNumber(int bitNumber, int maxBits)
    {
        if (bitNumber < 0 || bitNumber >= maxBits)
            throw new ArgumentOutOfRangeException(nameof(bitNumber), $"Bit number must be 0-{maxBits - 1}");
    }
}
