namespace DBI.Runtime.Motion.Interfaces;

/// <summary>
/// Represents the state of a servo axis.
/// </summary>
public enum AxisState
{
    /// <summary>Axis not initialized.</summary>
    NotInitialized,

    /// <summary>Axis is disabled (no power).</summary>
    Disabled,

    /// <summary>Axis is enabled and ready (standstill).</summary>
    Standstill,

    /// <summary>Axis is homing.</summary>
    Homing,

    /// <summary>Axis is moving (discrete motion).</summary>
    DiscreteMotion,

    /// <summary>Axis is moving (continuous motion).</summary>
    ContinuousMotion,

    /// <summary>Axis is synchronized/interpolating.</summary>
    SynchronizedMotion,

    /// <summary>Axis is stopping.</summary>
    Stopping,

    /// <summary>Axis has an error.</summary>
    ErrorStop
}

/// <summary>
/// Homing modes for servo axis.
/// </summary>
public enum HomingMode
{
    /// <summary>No homing.</summary>
    None = 0,

    /// <summary>Home to negative limit switch.</summary>
    NegativeLimit = 1,

    /// <summary>Home to positive limit switch.</summary>
    PositiveLimit = 2,

    /// <summary>Home to home/origin switch.</summary>
    HomeSwitch = 3,

    /// <summary>Home to encoder Z-pulse (index).</summary>
    EncoderIndex = 4,

    /// <summary>Home to current position.</summary>
    CurrentPosition = 5,

    /// <summary>Home to negative limit + Z-pulse.</summary>
    NegativeLimitAndIndex = 6,

    /// <summary>Home to positive limit + Z-pulse.</summary>
    PositiveLimitAndIndex = 7
}

/// <summary>
/// Pulse output mode for stepper/servo control.
/// </summary>
public enum PulseOutputMode
{
    /// <summary>Pulse + Direction mode.</summary>
    PulseDirection = 0,

    /// <summary>CW + CCW pulse mode.</summary>
    CwCcw = 1
}
