namespace DBI.Runtime.Functions.Compare;

/// <summary>
/// Comparison functions for PLC operations.
/// Implements IEC 61131-3 comparison operators.
/// </summary>
public static class CompareFunctions
{
    #region Equality

    /// <summary>
    /// Equal comparison.
    /// </summary>
    public static bool EQ(int a, int b) => a == b;

    /// <summary>
    /// Equal comparison.
    /// </summary>
    public static bool EQ(float a, float b) => System.Math.Abs(a - b) < float.Epsilon;

    /// <summary>
    /// Equal comparison.
    /// </summary>
    public static bool EQ(double a, double b) => System.Math.Abs(a - b) < double.Epsilon;

    /// <summary>
    /// Not equal comparison.
    /// </summary>
    public static bool NE(int a, int b) => a != b;

    /// <summary>
    /// Not equal comparison.
    /// </summary>
    public static bool NE(float a, float b) => System.Math.Abs(a - b) >= float.Epsilon;

    /// <summary>
    /// Not equal comparison.
    /// </summary>
    public static bool NE(double a, double b) => System.Math.Abs(a - b) >= double.Epsilon;

    #endregion

    #region Relational

    /// <summary>
    /// Greater than comparison.
    /// </summary>
    public static bool GT(int a, int b) => a > b;

    /// <summary>
    /// Greater than comparison.
    /// </summary>
    public static bool GT(float a, float b) => a > b;

    /// <summary>
    /// Greater than comparison.
    /// </summary>
    public static bool GT(double a, double b) => a > b;

    /// <summary>
    /// Greater than or equal comparison.
    /// </summary>
    public static bool GE(int a, int b) => a >= b;

    /// <summary>
    /// Greater than or equal comparison.
    /// </summary>
    public static bool GE(float a, float b) => a >= b;

    /// <summary>
    /// Greater than or equal comparison.
    /// </summary>
    public static bool GE(double a, double b) => a >= b;

    /// <summary>
    /// Less than comparison.
    /// </summary>
    public static bool LT(int a, int b) => a < b;

    /// <summary>
    /// Less than comparison.
    /// </summary>
    public static bool LT(float a, float b) => a < b;

    /// <summary>
    /// Less than comparison.
    /// </summary>
    public static bool LT(double a, double b) => a < b;

    /// <summary>
    /// Less than or equal comparison.
    /// </summary>
    public static bool LE(int a, int b) => a <= b;

    /// <summary>
    /// Less than or equal comparison.
    /// </summary>
    public static bool LE(float a, float b) => a <= b;

    /// <summary>
    /// Less than or equal comparison.
    /// </summary>
    public static bool LE(double a, double b) => a <= b;

    #endregion

    #region Range Check

    /// <summary>
    /// Checks if value is within range [min, max] (inclusive).
    /// </summary>
    public static bool IN_RANGE(int value, int min, int max) => value >= min && value <= max;

    /// <summary>
    /// Checks if value is within range [min, max] (inclusive).
    /// </summary>
    public static bool IN_RANGE(float value, float min, float max) => value >= min && value <= max;

    /// <summary>
    /// Checks if value is within range [min, max] (inclusive).
    /// </summary>
    public static bool IN_RANGE(double value, double min, double max) => value >= min && value <= max;

    /// <summary>
    /// Checks if value is outside range [min, max].
    /// </summary>
    public static bool OUT_RANGE(int value, int min, int max) => value < min || value > max;

    /// <summary>
    /// Checks if value is outside range [min, max].
    /// </summary>
    public static bool OUT_RANGE(float value, float min, float max) => value < min || value > max;

    /// <summary>
    /// Checks if value is outside range [min, max].
    /// </summary>
    public static bool OUT_RANGE(double value, double min, double max) => value < min || value > max;

    #endregion
}
