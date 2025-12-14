namespace DBI.Runtime.Core.Types;

/// <summary>
/// Represents IEC 61131-3 TIME data type.
/// Stored internally as TimeSpan for precision.
/// </summary>
public readonly struct PlcTime : IEquatable<PlcTime>, IComparable<PlcTime>
{
    private readonly TimeSpan _value;

    public PlcTime(TimeSpan value) => _value = value;

    public static PlcTime Zero => new(TimeSpan.Zero);

    public static PlcTime FromMilliseconds(double ms) => new(TimeSpan.FromMilliseconds(ms));
    public static PlcTime FromSeconds(double seconds) => new(TimeSpan.FromSeconds(seconds));
    public static PlcTime FromMinutes(double minutes) => new(TimeSpan.FromMinutes(minutes));

    public TimeSpan ToTimeSpan() => _value;
    public double TotalMilliseconds => _value.TotalMilliseconds;
    public double TotalSeconds => _value.TotalSeconds;

    // Operators
    public static PlcTime operator +(PlcTime a, PlcTime b) => new(a._value + b._value);
    public static PlcTime operator -(PlcTime a, PlcTime b) => new(a._value - b._value);
    public static bool operator ==(PlcTime a, PlcTime b) => a._value == b._value;
    public static bool operator !=(PlcTime a, PlcTime b) => a._value != b._value;
    public static bool operator <(PlcTime a, PlcTime b) => a._value < b._value;
    public static bool operator >(PlcTime a, PlcTime b) => a._value > b._value;
    public static bool operator <=(PlcTime a, PlcTime b) => a._value <= b._value;
    public static bool operator >=(PlcTime a, PlcTime b) => a._value >= b._value;

    // Implicit conversions
    public static implicit operator PlcTime(TimeSpan ts) => new(ts);
    public static implicit operator TimeSpan(PlcTime pt) => pt._value;

    public bool Equals(PlcTime other) => _value.Equals(other._value);
    public override bool Equals(object? obj) => obj is PlcTime other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public int CompareTo(PlcTime other) => _value.CompareTo(other._value);
    public override string ToString() => $"T#{_value.TotalMilliseconds}ms";
}
