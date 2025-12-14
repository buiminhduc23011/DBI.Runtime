namespace DBI.Runtime.Functions.Convert;

/// <summary>
/// Type conversion functions for PLC operations.
/// Implements IEC 61131-3 type conversions.
/// </summary>
public static class ConvertFunctions
{
    #region BOOL Conversions

    /// <summary>
    /// Converts BOOL to INT (FALSE=0, TRUE=1).
    /// </summary>
    public static int BOOL_TO_INT(bool value) => value ? 1 : 0;

    /// <summary>
    /// Converts BOOL to BYTE.
    /// </summary>
    public static byte BOOL_TO_BYTE(bool value) => value ? (byte)1 : (byte)0;

    /// <summary>
    /// Converts BOOL to WORD.
    /// </summary>
    public static ushort BOOL_TO_WORD(bool value) => value ? (ushort)1 : (ushort)0;

    /// <summary>
    /// Converts BOOL to DWORD.
    /// </summary>
    public static uint BOOL_TO_DWORD(bool value) => value ? 1u : 0u;

    #endregion

    #region INT Conversions

    /// <summary>
    /// Converts INT to REAL.
    /// </summary>
    public static float INT_TO_REAL(short value) => value;

    /// <summary>
    /// Converts INT to DINT.
    /// </summary>
    public static int INT_TO_DINT(short value) => value;

    /// <summary>
    /// Converts INT to BOOL (0=FALSE, else TRUE).
    /// </summary>
    public static bool INT_TO_BOOL(short value) => value != 0;

    /// <summary>
    /// Converts INT to WORD (same bit pattern).
    /// </summary>
    public static ushort INT_TO_WORD(short value) => (ushort)value;

    /// <summary>
    /// Converts INT to BYTE (truncates to low byte).
    /// </summary>
    public static byte INT_TO_BYTE(short value) => (byte)(value & 0xFF);

    #endregion

    #region DINT Conversions

    /// <summary>
    /// Converts DINT to REAL.
    /// </summary>
    public static float DINT_TO_REAL(int value) => value;

    /// <summary>
    /// Converts DINT to INT (truncates).
    /// </summary>
    public static short DINT_TO_INT(int value) => (short)value;

    /// <summary>
    /// Converts DINT to BOOL.
    /// </summary>
    public static bool DINT_TO_BOOL(int value) => value != 0;

    /// <summary>
    /// Converts DINT to DWORD (same bit pattern).
    /// </summary>
    public static uint DINT_TO_DWORD(int value) => (uint)value;

    /// <summary>
    /// Converts DINT to WORD (truncates).
    /// </summary>
    public static ushort DINT_TO_WORD(int value) => (ushort)(value & 0xFFFF);

    #endregion

    #region REAL Conversions

    /// <summary>
    /// Converts REAL to INT (rounds towards zero).
    /// </summary>
    public static short REAL_TO_INT(float value) => (short)value;

    /// <summary>
    /// Converts REAL to DINT (rounds towards zero).
    /// </summary>
    public static int REAL_TO_DINT(float value) => (int)value;

    /// <summary>
    /// Converts REAL to DWORD (bit pattern, for raw access).
    /// </summary>
    public static uint REAL_TO_DWORD(float value) => (uint)BitConverter.SingleToInt32Bits(value);

    /// <summary>
    /// Rounds REAL to nearest integer.
    /// </summary>
    public static int ROUND(float value) => (int)System.Math.Round(value);

    /// <summary>
    /// Truncates REAL towards zero.
    /// </summary>
    public static int TRUNC(float value) => (int)System.Math.Truncate(value);

    /// <summary>
    /// Floor - rounds towards negative infinity.
    /// </summary>
    public static int FLOOR(float value) => (int)System.Math.Floor(value);

    /// <summary>
    /// Ceiling - rounds towards positive infinity.
    /// </summary>
    public static int CEIL(float value) => (int)System.Math.Ceiling(value);

    #endregion

    #region BYTE Conversions

    /// <summary>
    /// Converts BYTE to INT.
    /// </summary>
    public static short BYTE_TO_INT(byte value) => value;

    /// <summary>
    /// Converts BYTE to WORD.
    /// </summary>
    public static ushort BYTE_TO_WORD(byte value) => value;

    /// <summary>
    /// Converts BYTE to DWORD.
    /// </summary>
    public static uint BYTE_TO_DWORD(byte value) => value;

    /// <summary>
    /// Converts BYTE to BOOL (bit 0).
    /// </summary>
    public static bool BYTE_TO_BOOL(byte value) => (value & 1) != 0;

    #endregion

    #region WORD Conversions

    /// <summary>
    /// Converts WORD to INT (same bit pattern).
    /// </summary>
    public static short WORD_TO_INT(ushort value) => (short)value;

    /// <summary>
    /// Converts WORD to DINT.
    /// </summary>
    public static int WORD_TO_DINT(ushort value) => value;

    /// <summary>
    /// Converts WORD to DWORD.
    /// </summary>
    public static uint WORD_TO_DWORD(ushort value) => value;

    /// <summary>
    /// Converts WORD to BYTE (low byte).
    /// </summary>
    public static byte WORD_TO_BYTE(ushort value) => (byte)(value & 0xFF);

    /// <summary>
    /// Converts WORD to BOOL (bit 0).
    /// </summary>
    public static bool WORD_TO_BOOL(ushort value) => (value & 1) != 0;

    #endregion

    #region DWORD Conversions

    /// <summary>
    /// Converts DWORD to DINT (same bit pattern).
    /// </summary>
    public static int DWORD_TO_DINT(uint value) => (int)value;

    /// <summary>
    /// Converts DWORD to REAL (bit pattern).
    /// </summary>
    public static float DWORD_TO_REAL(uint value) => BitConverter.Int32BitsToSingle((int)value);

    /// <summary>
    /// Converts DWORD to WORD (low word).
    /// </summary>
    public static ushort DWORD_TO_WORD(uint value) => (ushort)(value & 0xFFFF);

    /// <summary>
    /// Converts DWORD to BYTE (low byte).
    /// </summary>
    public static byte DWORD_TO_BYTE(uint value) => (byte)(value & 0xFF);

    /// <summary>
    /// Converts DWORD to BOOL (bit 0).
    /// </summary>
    public static bool DWORD_TO_BOOL(uint value) => (value & 1) != 0;

    #endregion

    #region TIME Conversions

    /// <summary>
    /// Converts TIME (milliseconds as int) to TimeSpan.
    /// </summary>
    public static TimeSpan TIME_TO_TIMESPAN(int milliseconds) => TimeSpan.FromMilliseconds(milliseconds);

    /// <summary>
    /// Converts TimeSpan to TIME (milliseconds as int).
    /// </summary>
    public static int TIMESPAN_TO_TIME(TimeSpan value) => (int)value.TotalMilliseconds;

    /// <summary>
    /// Converts DINT milliseconds to TimeSpan.
    /// </summary>
    public static TimeSpan DINT_TO_TIME(int milliseconds) => TimeSpan.FromMilliseconds(milliseconds);

    /// <summary>
    /// Converts TimeSpan to DINT milliseconds.
    /// </summary>
    public static int TIME_TO_DINT(TimeSpan value) => (int)value.TotalMilliseconds;

    #endregion

    #region String Conversions (basic)

    /// <summary>
    /// Converts INT to string.
    /// </summary>
    public static string INT_TO_STRING(short value) => value.ToString();

    /// <summary>
    /// Converts DINT to string.
    /// </summary>
    public static string DINT_TO_STRING(int value) => value.ToString();

    /// <summary>
    /// Converts REAL to string.
    /// </summary>
    public static string REAL_TO_STRING(float value) => value.ToString("G");

    /// <summary>
    /// Parses string to INT.
    /// </summary>
    public static short STRING_TO_INT(string value) => short.TryParse(value, out var result) ? result : (short)0;

    /// <summary>
    /// Parses string to DINT.
    /// </summary>
    public static int STRING_TO_DINT(string value) => int.TryParse(value, out var result) ? result : 0;

    /// <summary>
    /// Parses string to REAL.
    /// </summary>
    public static float STRING_TO_REAL(string value) => float.TryParse(value, out var result) ? result : 0f;

    #endregion

    #region BCD Conversions (Siemens-specific)

    /// <summary>
    /// Converts BCD-encoded word to integer.
    /// </summary>
    public static int BCD_TO_INT(ushort bcdValue)
    {
        int result = 0;
        int multiplier = 1;

        for (int i = 0; i < 4; i++)
        {
            int digit = (bcdValue >> (i * 4)) & 0x0F;
            if (digit > 9) return -1; // Invalid BCD
            result += digit * multiplier;
            multiplier *= 10;
        }

        return result;
    }

    /// <summary>
    /// Converts integer to BCD-encoded word.
    /// </summary>
    public static ushort INT_TO_BCD(int value)
    {
        if (value < 0 || value > 9999) return 0;

        ushort result = 0;
        for (int i = 0; i < 4; i++)
        {
            result |= (ushort)((value % 10) << (i * 4));
            value /= 10;
        }

        return result;
    }

    #endregion
}
