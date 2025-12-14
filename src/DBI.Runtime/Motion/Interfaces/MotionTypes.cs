namespace DBI.Runtime.Motion.Interfaces;

/// <summary>
/// Motion profile parameters for servo motion.
/// </summary>
public class MotionProfile
{
    /// <summary>
    /// Maximum velocity in units/second.
    /// </summary>
    public double Velocity { get; set; }

    /// <summary>
    /// Acceleration in units/second².
    /// </summary>
    public double Acceleration { get; set; }

    /// <summary>
    /// Deceleration in units/second². If 0, uses Acceleration value.
    /// </summary>
    public double Deceleration { get; set; }

    /// <summary>
    /// Jerk in units/second³. 0 = trapezoidal profile, >0 = S-curve.
    /// </summary>
    public double Jerk { get; set; }

    /// <summary>
    /// Creates a simple trapezoidal motion profile.
    /// </summary>
    public static MotionProfile Trapezoidal(double velocity, double acceleration)
    {
        return new MotionProfile
        {
            Velocity = velocity,
            Acceleration = acceleration,
            Deceleration = acceleration,
            Jerk = 0
        };
    }

    /// <summary>
    /// Creates an S-curve motion profile.
    /// </summary>
    public static MotionProfile SCurve(double velocity, double acceleration, double jerk)
    {
        return new MotionProfile
        {
            Velocity = velocity,
            Acceleration = acceleration,
            Deceleration = acceleration,
            Jerk = jerk
        };
    }
}

/// <summary>
/// Configuration for motion controller initialization.
/// </summary>
public class MotionControllerConfig
{
    /// <summary>
    /// Card/board number (0-based).
    /// </summary>
    public int CardNumber { get; set; }

    /// <summary>
    /// Number of axes to initialize.
    /// </summary>
    public int AxisCount { get; set; } = 4;

    /// <summary>
    /// Pulse per unit (encoder counts per unit).
    /// </summary>
    public double PulsePerUnit { get; set; } = 1000;

    /// <summary>
    /// Connection string (for network-based controllers).
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Enable simulation mode (no hardware).
    /// </summary>
    public bool SimulationMode { get; set; }
}

/// <summary>
/// Axis configuration parameters.
/// </summary>
public class AxisConfig
{
    /// <summary>
    /// Axis index (0-based).
    /// </summary>
    public int AxisIndex { get; set; }

    /// <summary>
    /// Pulse output mode.
    /// </summary>
    public PulseOutputMode PulseMode { get; set; } = PulseOutputMode.PulseDirection;

    /// <summary>
    /// Pulses per unit of movement.
    /// </summary>
    public double PulsesPerUnit { get; set; } = 1000;

    /// <summary>
    /// Maximum velocity limit.
    /// </summary>
    public double MaxVelocity { get; set; } = 100000;

    /// <summary>
    /// Maximum acceleration limit.
    /// </summary>
    public double MaxAcceleration { get; set; } = 500000;

    /// <summary>
    /// Software positive limit.
    /// </summary>
    public double SoftLimitPositive { get; set; } = double.MaxValue;

    /// <summary>
    /// Software negative limit.
    /// </summary>
    public double SoftLimitNegative { get; set; } = double.MinValue;

    /// <summary>
    /// Invert direction.
    /// </summary>
    public bool InvertDirection { get; set; }
}
