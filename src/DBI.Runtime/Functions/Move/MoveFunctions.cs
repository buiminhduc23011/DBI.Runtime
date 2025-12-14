namespace DBI.Runtime.Functions.Move;

/// <summary>
/// Move and copy functions for PLC operations.
/// </summary>
public static class MoveFunctions
{
    /// <summary>
    /// Moves (copies) a value. Returns the input value unchanged.
    /// </summary>
    public static T MOVE<T>(T value) => value;

    /// <summary>
    /// Fills an array with a value.
    /// </summary>
    public static void FILL<T>(T[] array, T value)
    {
        Array.Fill(array, value);
    }

    /// <summary>
    /// Fills a portion of an array with a value.
    /// </summary>
    public static void FILL<T>(T[] array, T value, int startIndex, int count)
    {
        Array.Fill(array, value, startIndex, count);
    }

    /// <summary>
    /// Copies bytes from source to destination.
    /// </summary>
    public static void BLKMOV(byte[] source, int sourceIndex, byte[] destination, int destIndex, int count)
    {
        Array.Copy(source, sourceIndex, destination, destIndex, count);
    }

    /// <summary>
    /// Swaps two values.
    /// </summary>
    public static void SWAP<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }
}
