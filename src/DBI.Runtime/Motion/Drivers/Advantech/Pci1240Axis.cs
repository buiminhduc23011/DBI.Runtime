using DBI.Runtime.Motion.Interfaces;

namespace DBI.Runtime.Motion.Drivers.Advantech;

/// <summary>
/// Advantech PCI-1240U servo axis implementation.
/// </summary>
public class Pci1240Axis : IServoAxis
{
    private readonly int _cardNo;
    private AxisConfig _config;
    private AxisState _state;
    private int _errorCode;
    private bool _isHomed;
    private double _targetPosition;

    /// <summary>
    /// Creates a new PCI-1240 axis.
    /// </summary>
    public Pci1240Axis(int cardNo, int axisIndex)
    {
        _cardNo = cardNo;
        AxisIndex = axisIndex;
        Name = $"Axis{axisIndex}";
        _config = new AxisConfig { AxisIndex = axisIndex };
        _state = AxisState.Disabled;
    }

    #region IServoAxis Properties

    /// <inheritdoc/>
    public int AxisIndex { get; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public AxisState State => _state;

    /// <inheritdoc/>
    public bool IsEnabled
    {
        get
        {
            int enabled = 0;
            Pci1240Api.P1240MotGetServoOn(_cardNo, AxisIndex, ref enabled);
            return enabled == 1;
        }
    }

    /// <inheritdoc/>
    public bool IsMoving
    {
        get
        {
            int status = 0;
            Pci1240Api.P1240MotGetStatus(_cardNo, AxisIndex, ref status);
            return (status & Pci1240Api.STS_BUSY) != 0;
        }
    }

    /// <inheritdoc/>
    public bool IsHomed => _isHomed;

    /// <inheritdoc/>
    public bool IsInPosition
    {
        get
        {
            int inpos = 0;
            Pci1240Api.P1240MotGetInPos(_cardNo, AxisIndex, ref inpos);
            return inpos == 1;
        }
    }

    /// <inheritdoc/>
    public bool HasError => _errorCode != 0 || AlarmActive;

    /// <inheritdoc/>
    public int ErrorCode => _errorCode;

    /// <inheritdoc/>
    public string ErrorMessage => GetErrorMessage(_errorCode);

    /// <inheritdoc/>
    public double ActualPosition
    {
        get
        {
            int pos = 0;
            Pci1240Api.P1240MotGetActPos(_cardNo, AxisIndex, ref pos);
            return pos / _config.PulsesPerUnit;
        }
    }

    /// <inheritdoc/>
    public double CommandPosition
    {
        get
        {
            int pos = 0;
            Pci1240Api.P1240MotGetCmdPos(_cardNo, AxisIndex, ref pos);
            return pos / _config.PulsesPerUnit;
        }
    }

    /// <inheritdoc/>
    public double ActualVelocity => 0; // Would need velocity calculation

    /// <inheritdoc/>
    public double TargetPosition => _targetPosition;

    /// <inheritdoc/>
    public double PositionError => CommandPosition - ActualPosition;

    /// <inheritdoc/>
    public bool PositiveLimitActive
    {
        get
        {
            int pos = 0, neg = 0;
            Pci1240Api.P1240MotGetLimitStatus(_cardNo, AxisIndex, ref pos, ref neg);
            return pos == 1;
        }
    }

    /// <inheritdoc/>
    public bool NegativeLimitActive
    {
        get
        {
            int pos = 0, neg = 0;
            Pci1240Api.P1240MotGetLimitStatus(_cardNo, AxisIndex, ref pos, ref neg);
            return neg == 1;
        }
    }

    /// <inheritdoc/>
    public bool HomeSwitchActive
    {
        get
        {
            int status = 0;
            Pci1240Api.P1240MotGetHomeSwitch(_cardNo, AxisIndex, ref status);
            return status == 1;
        }
    }

    /// <inheritdoc/>
    public bool AlarmActive
    {
        get
        {
            int alarm = 0;
            Pci1240Api.P1240MotGetAlarm(_cardNo, AxisIndex, ref alarm);
            return alarm == 1;
        }
    }

    #endregion

    #region Control Methods

    /// <inheritdoc/>
    public void Enable()
    {
        int result = Pci1240Api.P1240MotSetServoOn(_cardNo, AxisIndex, 1);
        if (result != 0)
        {
            _errorCode = result;
            _state = AxisState.ErrorStop;
        }
        else
        {
            _state = AxisState.Standstill;
        }
    }

    /// <inheritdoc/>
    public void Disable()
    {
        Pci1240Api.P1240MotSetServoOn(_cardNo, AxisIndex, 0);
        _state = AxisState.Disabled;
    }

    /// <inheritdoc/>
    public void ResetError()
    {
        Pci1240Api.P1240MotClrAlarm(_cardNo, AxisIndex);
        _errorCode = 0;
        if (_state == AxisState.ErrorStop)
        {
            _state = IsEnabled ? AxisState.Standstill : AxisState.Disabled;
        }
    }

    /// <inheritdoc/>
    public void SetZeroPosition()
    {
        Pci1240Api.P1240MotSetCmdPos(_cardNo, AxisIndex, 0);
        Pci1240Api.P1240MotSetActPos(_cardNo, AxisIndex, 0);
    }

    /// <inheritdoc/>
    public void SetPosition(double position)
    {
        int pulses = (int)(position * _config.PulsesPerUnit);
        Pci1240Api.P1240MotSetCmdPos(_cardNo, AxisIndex, pulses);
        Pci1240Api.P1240MotSetActPos(_cardNo, AxisIndex, pulses);
    }

    #endregion

    #region Motion Methods

    /// <inheritdoc/>
    public void StartHome(HomingMode mode, double velocity, double acceleration)
    {
        if (!IsEnabled)
        {
            _errorCode = -100;
            return;
        }

        int homeDir = mode == HomingMode.PositiveLimit || mode == HomingMode.PositiveLimitAndIndex ? 1 : 0;
        int homeMode = HomingModeToPci(mode);

        double speed1 = velocity * _config.PulsesPerUnit;
        double speed2 = speed1 * 0.1; // Slow speed for final approach
        double acc = acceleration * _config.PulsesPerUnit;

        Pci1240Api.P1240MotSetHomeConfig(_cardNo, AxisIndex, homeMode, homeDir, speed1, speed2, acc);

        int result = Pci1240Api.P1240MotStartHome(_cardNo, AxisIndex);
        if (result == 0)
        {
            _state = AxisState.Homing;
        }
        else
        {
            _errorCode = result;
            _state = AxisState.ErrorStop;
        }
    }

    /// <inheritdoc/>
    public void MoveAbsolute(double position, MotionProfile profile)
    {
        if (!IsEnabled)
        {
            _errorCode = -100;
            return;
        }

        SetProfile(profile);
        _targetPosition = position;

        int posPulse = (int)(position * _config.PulsesPerUnit);
        int result = Pci1240Api.P1240MotAbsoluteMove(_cardNo, AxisIndex, posPulse);

        if (result == 0)
        {
            _state = AxisState.DiscreteMotion;
        }
        else
        {
            _errorCode = result;
            _state = AxisState.ErrorStop;
        }
    }

    /// <inheritdoc/>
    public void MoveRelative(double distance, MotionProfile profile)
    {
        if (!IsEnabled)
        {
            _errorCode = -100;
            return;
        }

        SetProfile(profile);
        _targetPosition = CommandPosition + distance;

        int distPulse = (int)(distance * _config.PulsesPerUnit);
        int result = Pci1240Api.P1240MotRelativeMove(_cardNo, AxisIndex, distPulse);

        if (result == 0)
        {
            _state = AxisState.DiscreteMotion;
        }
        else
        {
            _errorCode = result;
            _state = AxisState.ErrorStop;
        }
    }

    /// <inheritdoc/>
    public void MoveVelocity(double velocity, double acceleration)
    {
        if (!IsEnabled)
        {
            _errorCode = -100;
            return;
        }

        double velPulse = Math.Abs(velocity * _config.PulsesPerUnit);
        double accTime = Math.Abs(velocity / acceleration);

        // Set speed profile
        Pci1240Api.P1240MotSetSpeed(_cardNo, AxisIndex, velPulse * 0.01, velPulse, accTime, accTime);

        int dir = velocity >= 0 ? 1 : 0;
        int result = Pci1240Api.P1240MotContMove(_cardNo, AxisIndex, dir);

        if (result == 0)
        {
            _state = AxisState.ContinuousMotion;
        }
        else
        {
            _errorCode = result;
            _state = AxisState.ErrorStop;
        }
    }

    /// <inheritdoc/>
    public void Stop(double deceleration)
    {
        Pci1240Api.P1240MotDecelStop(_cardNo, AxisIndex);
        _state = AxisState.Stopping;
    }

    /// <inheritdoc/>
    public void EmergencyStop()
    {
        Pci1240Api.P1240MotEmgStop(_cardNo, AxisIndex);
        _state = AxisState.Standstill;
    }

    #endregion

    #region Configuration

    /// <inheritdoc/>
    public void Configure(AxisConfig config)
    {
        _config = config;

        // Set pulse output mode
        int pulseMode = config.PulseMode == PulseOutputMode.CwCcw ? 1 : 0;
        int logic = config.InvertDirection ? 1 : 0;
        Pci1240Api.P1240MotSetPulseOut(_cardNo, AxisIndex, pulseMode, logic);

        // Set software limits
        if (config.SoftLimitPositive != double.MaxValue && config.SoftLimitNegative != double.MinValue)
        {
            int posLimit = (int)(config.SoftLimitPositive * config.PulsesPerUnit);
            int negLimit = (int)(config.SoftLimitNegative * config.PulsesPerUnit);
            Pci1240Api.P1240MotSetSoftLimit(_cardNo, AxisIndex, 1, posLimit, negLimit);
        }
    }

    /// <inheritdoc/>
    public void SetSoftwareLimits(double negative, double positive)
    {
        _config.SoftLimitNegative = negative;
        _config.SoftLimitPositive = positive;

        int posLimit = (int)(positive * _config.PulsesPerUnit);
        int negLimit = (int)(negative * _config.PulsesPerUnit);
        Pci1240Api.P1240MotSetSoftLimit(_cardNo, AxisIndex, 1, posLimit, negLimit);
    }

    /// <inheritdoc/>
    public void SetVelocityLimit(double maxVelocity)
    {
        _config.MaxVelocity = maxVelocity;
    }

    #endregion

    /// <inheritdoc/>
    public void UpdateStatus()
    {
        // Check homing status
        if (_state == AxisState.Homing)
        {
            int homeStatus = 0;
            Pci1240Api.P1240MotGetHomeStatus(_cardNo, AxisIndex, ref homeStatus);
            if (homeStatus == 0) // Homing complete
            {
                _isHomed = true;
                _state = AxisState.Standstill;
                SetZeroPosition();
            }
        }
        else if (!IsMoving && (_state == AxisState.DiscreteMotion || _state == AxisState.Stopping))
        {
            _state = AxisState.Standstill;
        }

        // Check for errors
        if (AlarmActive)
        {
            _errorCode = -200;
            _state = AxisState.ErrorStop;
        }
    }

    #region Private Methods

    private void SetProfile(MotionProfile profile)
    {
        double strVel = profile.Velocity * _config.PulsesPerUnit * 0.01;
        double maxVel = profile.Velocity * _config.PulsesPerUnit;
        double accTime = profile.Velocity / profile.Acceleration;
        double decTime = profile.Velocity / (profile.Deceleration > 0 ? profile.Deceleration : profile.Acceleration);

        if (profile.Jerk > 0)
        {
            double sRatio = 0.5; // S-curve ratio
            Pci1240Api.P1240MotSetSCurve(_cardNo, AxisIndex, strVel, maxVel, accTime, decTime, sRatio);
        }
        else
        {
            Pci1240Api.P1240MotSetSpeed(_cardNo, AxisIndex, strVel, maxVel, accTime, decTime);
        }
    }

    private static int HomingModeToPci(HomingMode mode) => mode switch
    {
        HomingMode.NegativeLimit => 0,
        HomingMode.PositiveLimit => 1,
        HomingMode.HomeSwitch => 2,
        HomingMode.EncoderIndex => 3,
        HomingMode.NegativeLimitAndIndex => 4,
        HomingMode.PositiveLimitAndIndex => 5,
        _ => 2 // Default to home switch
    };

    private static string GetErrorMessage(int errorCode) => errorCode switch
    {
        0 => "No error",
        -1 => "Invalid card number",
        -2 => "Invalid axis number",
        -3 => "Invalid parameter",
        -4 => "Device not ready",
        -5 => "Motion busy",
        -100 => "Axis not enabled",
        -200 => "Servo alarm active",
        _ => $"Unknown error ({errorCode})"
    };

    #endregion
}
