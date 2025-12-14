using DBI.Runtime.Motion.Interfaces;

namespace DBI.Runtime.Motion.Drivers.Leadshine;

/// <summary>
/// Leadshine DMC2410 servo axis implementation.
/// </summary>
public class Dmc2410Axis : IServoAxis
{
    private readonly int _cardNo;
    private AxisConfig _config;
    private AxisState _state;
    private int _errorCode;
    private bool _isHomed;

    // Cached I/O status bits
    private const int IO_BIT_POS_LIMIT = 0;
    private const int IO_BIT_NEG_LIMIT = 1;
    private const int IO_BIT_ORIGIN = 2;
    private const int IO_BIT_ALARM = 3;
    private const int IO_BIT_INPOS = 4;

    /// <summary>
    /// Creates a new DMC2410 axis.
    /// </summary>
    public Dmc2410Axis(int cardNo, int axisIndex)
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
    public bool IsEnabled => Dmc2410Api.d2410_read_sevon_out(AxisIndex) == 1;

    /// <inheritdoc/>
    public bool IsMoving => Dmc2410Api.d2410_check_done(AxisIndex) == 1;

    /// <inheritdoc/>
    public bool IsHomed => _isHomed;

    /// <inheritdoc/>
    public bool IsInPosition => !IsMoving && ((ReadAxisIO() >> IO_BIT_INPOS) & 1) == 1;

    /// <inheritdoc/>
    public bool HasError => _errorCode != 0 || AlarmActive;

    /// <inheritdoc/>
    public int ErrorCode => _errorCode;

    /// <inheritdoc/>
    public string ErrorMessage => GetErrorMessage(_errorCode);

    /// <inheritdoc/>
    public double ActualPosition => Dmc2410Api.d2410_get_encoder(AxisIndex) / _config.PulsesPerUnit;

    /// <inheritdoc/>
    public double CommandPosition => Dmc2410Api.d2410_get_position(AxisIndex) / _config.PulsesPerUnit;

    /// <inheritdoc/>
    public double ActualVelocity => 0; // Not directly available, would need calculation

    /// <inheritdoc/>
    public double TargetPosition { get; private set; }

    /// <inheritdoc/>
    public double PositionError => CommandPosition - ActualPosition;

    /// <inheritdoc/>
    public bool PositiveLimitActive => ((ReadAxisIO() >> IO_BIT_POS_LIMIT) & 1) == 1;

    /// <inheritdoc/>
    public bool NegativeLimitActive => ((ReadAxisIO() >> IO_BIT_NEG_LIMIT) & 1) == 1;

    /// <inheritdoc/>
    public bool HomeSwitchActive => ((ReadAxisIO() >> IO_BIT_ORIGIN) & 1) == 1;

    /// <inheritdoc/>
    public bool AlarmActive => ((ReadAxisIO() >> IO_BIT_ALARM) & 1) == 1;

    #endregion

    #region Control Methods

    /// <inheritdoc/>
    public void Enable()
    {
        int result = Dmc2410Api.d2410_set_sevon_out(AxisIndex, 1);
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
        Dmc2410Api.d2410_set_sevon_out(AxisIndex, 0);
        _state = AxisState.Disabled;
    }

    /// <inheritdoc/>
    public void ResetError()
    {
        Dmc2410Api.d2410_set_alm_clr(AxisIndex);
        _errorCode = 0;
        if (_state == AxisState.ErrorStop)
        {
            _state = IsEnabled ? AxisState.Standstill : AxisState.Disabled;
        }
    }

    /// <inheritdoc/>
    public void SetZeroPosition()
    {
        Dmc2410Api.d2410_set_position(AxisIndex, 0);
        Dmc2410Api.d2410_set_encoder(AxisIndex, 0);
    }

    /// <inheritdoc/>
    public void SetPosition(double position)
    {
        int pulses = (int)(position * _config.PulsesPerUnit);
        Dmc2410Api.d2410_set_position(AxisIndex, pulses);
        Dmc2410Api.d2410_set_encoder(AxisIndex, pulses);
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
        int dmcMode = HomingModeToDmc(mode);

        Dmc2410Api.d2410_config_home_mode(AxisIndex, dmcMode, homeDir);

        int velPulse = (int)(velocity * _config.PulsesPerUnit);
        double accTime = velocity / acceleration * 1000; // ms

        int result = Dmc2410Api.d2410_home_start(AxisIndex, velPulse / 10, velPulse, accTime);
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
        TargetPosition = position;

        int posPulse = (int)(position * _config.PulsesPerUnit);
        int result = Dmc2410Api.d2410_amove(AxisIndex, posPulse);

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
        TargetPosition = CommandPosition + distance;

        int distPulse = (int)(distance * _config.PulsesPerUnit);
        int result = Dmc2410Api.d2410_pmove(AxisIndex, distPulse);

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

        int velPulse = (int)(velocity * _config.PulsesPerUnit);
        double accTime = Math.Abs(velocity / acceleration) * 1000; // ms

        int result = Dmc2410Api.d2410_vmove(AxisIndex, Math.Abs(velPulse) / 10, velPulse, accTime);

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
        double decTime = ActualVelocity != 0 ? Math.Abs(ActualVelocity / deceleration) * 1000 : 100;
        Dmc2410Api.d2410_decel_stop(AxisIndex, decTime);
        _state = AxisState.Stopping;
    }

    /// <inheritdoc/>
    public void EmergencyStop()
    {
        Dmc2410Api.d2410_emg_stop(AxisIndex);
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
        int dirLogic = config.InvertDirection ? 1 : 0;
        Dmc2410Api.d2410_set_pulse_outmode(AxisIndex, pulseMode, 0, dirLogic);

        // Set software limits
        if (config.SoftLimitPositive != double.MaxValue && config.SoftLimitNegative != double.MinValue)
        {
            int posLimit = (int)(config.SoftLimitPositive * config.PulsesPerUnit);
            int negLimit = (int)(config.SoftLimitNegative * config.PulsesPerUnit);
            Dmc2410Api.d2410_set_softlimit(AxisIndex, 1, posLimit, negLimit);
        }
    }

    /// <inheritdoc/>
    public void SetSoftwareLimits(double negative, double positive)
    {
        _config.SoftLimitNegative = negative;
        _config.SoftLimitPositive = positive;

        int posLimit = (int)(positive * _config.PulsesPerUnit);
        int negLimit = (int)(negative * _config.PulsesPerUnit);
        Dmc2410Api.d2410_set_softlimit(AxisIndex, 1, posLimit, negLimit);
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
        // Update state based on motion status
        if (Dmc2410Api.d2410_check_home(AxisIndex) == 0 && _state == AxisState.Homing)
        {
            _isHomed = true;
            _state = AxisState.Standstill;
            SetZeroPosition();
        }
        else if (!IsMoving && (_state == AxisState.DiscreteMotion || _state == AxisState.Stopping))
        {
            _state = AxisState.Standstill;
        }

        // Check for errors
        if (AlarmActive)
        {
            _errorCode = Dmc2410Api.ERR_ALARM_ACTIVE;
            _state = AxisState.ErrorStop;
        }
    }

    #region Private Methods

    private int ReadAxisIO() => Dmc2410Api.d2410_read_axis_io(AxisIndex);

    private void SetProfile(MotionProfile profile)
    {
        int startVel = (int)(profile.Velocity * _config.PulsesPerUnit * 0.01); // 1% of run vel
        int runVel = (int)(profile.Velocity * _config.PulsesPerUnit);
        double accTime = profile.Velocity / profile.Acceleration * 1000; // ms
        double decTime = profile.Velocity / (profile.Deceleration > 0 ? profile.Deceleration : profile.Acceleration) * 1000;

        if (profile.Jerk > 0)
        {
            double sTime = profile.Acceleration / profile.Jerk * 1000;
            Dmc2410Api.d2410_set_s_profile(AxisIndex, startVel, runVel, accTime, decTime, sTime);
        }
        else
        {
            Dmc2410Api.d2410_set_profile(AxisIndex, startVel, runVel, accTime, decTime);
        }
    }

    private static int HomingModeToDmc(HomingMode mode) => mode switch
    {
        HomingMode.NegativeLimit => 0,
        HomingMode.PositiveLimit => 1,
        HomingMode.HomeSwitch => 2,
        HomingMode.EncoderIndex => 3,
        HomingMode.NegativeLimitAndIndex => 4,
        HomingMode.PositiveLimitAndIndex => 5,
        _ => 0
    };

    private static string GetErrorMessage(int errorCode) => errorCode switch
    {
        0 => "No error",
        -1 => "Card not found",
        -2 => "Invalid axis number",
        -3 => "Invalid parameter",
        -4 => "Motion not complete",
        -5 => "Limit switch active",
        -6 => "Servo alarm active",
        -100 => "Axis not enabled",
        _ => $"Unknown error ({errorCode})"
    };

    #endregion
}
