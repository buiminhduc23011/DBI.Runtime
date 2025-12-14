namespace DBI.Runtime.Functions.Math;

/// <summary>
/// Mathematical functions for PLC operations.
/// Implements IEC 61131-3 and Siemens standard math blocks.
/// </summary>
public static class MathFunctions
{
    #region Basic Arithmetic

    /// <summary>
    /// Adds two values.
    /// </summary>
    public static int ADD(int a, int b) => a + b;

    /// <summary>
    /// Adds two values.
    /// </summary>
    public static float ADD(float a, float b) => a + b;

    /// <summary>
    /// Adds two values.
    /// </summary>
    public static double ADD(double a, double b) => a + b;

    /// <summary>
    /// Subtracts b from a.
    /// </summary>
    public static int SUB(int a, int b) => a - b;

    /// <summary>
    /// Subtracts b from a.
    /// </summary>
    public static float SUB(float a, float b) => a - b;

    /// <summary>
    /// Subtracts b from a.
    /// </summary>
    public static double SUB(double a, double b) => a - b;

    /// <summary>
    /// Multiplies two values.
    /// </summary>
    public static int MUL(int a, int b) => a * b;

    /// <summary>
    /// Multiplies two values.
    /// </summary>
    public static float MUL(float a, float b) => a * b;

    /// <summary>
    /// Multiplies two values.
    /// </summary>
    public static double MUL(double a, double b) => a * b;

    /// <summary>
    /// Divides a by b. Returns 0 for division by zero.
    /// </summary>
    public static int DIV(int a, int b) => b != 0 ? a / b : 0;

    /// <summary>
    /// Divides a by b.
    /// </summary>
    public static float DIV(float a, float b) => b != 0 ? a / b : float.NaN;

    /// <summary>
    /// Divides a by b.
    /// </summary>
    public static double DIV(double a, double b) => b != 0 ? a / b : double.NaN;

    /// <summary>
    /// Modulo operation.
    /// </summary>
    public static int MOD(int a, int b) => b != 0 ? a % b : 0;

    #endregion

    #region Absolute and Square Root

    /// <summary>
    /// Returns absolute value.
    /// </summary>
    public static int ABS(int value) => System.Math.Abs(value);

    /// <summary>
    /// Returns absolute value.
    /// </summary>
    public static float ABS(float value) => System.Math.Abs(value);

    /// <summary>
    /// Returns absolute value.
    /// </summary>
    public static double ABS(double value) => System.Math.Abs(value);

    /// <summary>
    /// Returns square root.
    /// </summary>
    public static float SQRT(float value) => (float)System.Math.Sqrt(value);

    /// <summary>
    /// Returns square root.
    /// </summary>
    public static double SQRT(double value) => System.Math.Sqrt(value);

    /// <summary>
    /// Returns square (value * value).
    /// </summary>
    public static int SQR(int value) => value * value;

    /// <summary>
    /// Returns square.
    /// </summary>
    public static float SQR(float value) => value * value;

    /// <summary>
    /// Returns square.
    /// </summary>
    public static double SQR(double value) => value * value;

    #endregion

    #region Min/Max/Limit

    /// <summary>
    /// Returns the minimum of two values.
    /// </summary>
    public static int MIN(int a, int b) => System.Math.Min(a, b);

    /// <summary>
    /// Returns the minimum of two values.
    /// </summary>
    public static float MIN(float a, float b) => System.Math.Min(a, b);

    /// <summary>
    /// Returns the minimum of two values.
    /// </summary>
    public static double MIN(double a, double b) => System.Math.Min(a, b);

    /// <summary>
    /// Returns the maximum of two values.
    /// </summary>
    public static int MAX(int a, int b) => System.Math.Max(a, b);

    /// <summary>
    /// Returns the maximum of two values.
    /// </summary>
    public static float MAX(float a, float b) => System.Math.Max(a, b);

    /// <summary>
    /// Returns the maximum of two values.
    /// </summary>
    public static double MAX(double a, double b) => System.Math.Max(a, b);

    /// <summary>
    /// Limits value to range [min, max].
    /// </summary>
    public static int LIMIT(int min, int value, int max) => System.Math.Clamp(value, min, max);

    /// <summary>
    /// Limits value to range [min, max].
    /// </summary>
    public static float LIMIT(float min, float value, float max) => System.Math.Clamp(value, min, max);

    /// <summary>
    /// Limits value to range [min, max].
    /// </summary>
    public static double LIMIT(double min, double value, double max) => System.Math.Clamp(value, min, max);

    #endregion

    #region Trigonometric

    /// <summary>
    /// Sine in radians.
    /// </summary>
    public static float SIN(float angle) => (float)System.Math.Sin(angle);

    /// <summary>
    /// Cosine in radians.
    /// </summary>
    public static float COS(float angle) => (float)System.Math.Cos(angle);

    /// <summary>
    /// Tangent in radians.
    /// </summary>
    public static float TAN(float angle) => (float)System.Math.Tan(angle);

    /// <summary>
    /// Arc sine.
    /// </summary>
    public static float ASIN(float value) => (float)System.Math.Asin(value);

    /// <summary>
    /// Arc cosine.
    /// </summary>
    public static float ACOS(float value) => (float)System.Math.Acos(value);

    /// <summary>
    /// Arc tangent.
    /// </summary>
    public static float ATAN(float value) => (float)System.Math.Atan(value);

    #endregion

    #region Logarithmic and Exponential

    /// <summary>
    /// Natural logarithm.
    /// </summary>
    public static float LN(float value) => (float)System.Math.Log(value);

    /// <summary>
    /// Base-10 logarithm.
    /// </summary>
    public static float LOG(float value) => (float)System.Math.Log10(value);

    /// <summary>
    /// Exponential (e^value).
    /// </summary>
    public static float EXP(float value) => (float)System.Math.Exp(value);

    /// <summary>
    /// Power (base^exponent).
    /// </summary>
    public static float EXPT(float baseValue, float exponent) => (float)System.Math.Pow(baseValue, exponent);

    #endregion
}
