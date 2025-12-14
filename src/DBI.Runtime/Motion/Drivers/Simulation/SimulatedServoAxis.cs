using DBI.Runtime.Motion.Interfaces;

namespace DBI.Runtime.Motion.Drivers.Simulation;

/// <summary>
/// Simulated servo axis for testing without hardware.
/// </summary>
public class SimulatedServoAxis : IServoAxis
{
    private AxisConfig _config;
    private AxisState _state;
    private double _commandPosition;
    private double _actualPosition;
    private double _velocity;
    private double _targetPosition;
    private double _maxVelocity = 100000;
    private bool _isHomed;
    private int _errorCode;

    /// <summary>
    /// Creates a new simulated axis.
    /// </summary>
    public SimulatedServoAxis(int axisIndex)
    {
        AxisIndex = axisIndex;
        Name = $"SimAxis{axisIndex}";
        _config = new AxisConfig { AxisIndex = axisIndex, PulsesPerUnit = 1 };
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
    public bool IsEnabled => _state != AxisState.Disabled && _state != AxisState.NotInitialized;

    /// <inheritdoc/>
    public bool IsMoving => _state == AxisState.DiscreteMotion ||
                             _state == AxisState.ContinuousMotion ||
                             _state == AxisState.Homing;

    /// <inheritdoc/>
    public bool IsHomed => _isHomed;

    /// <inheritdoc/>
    public bool IsInPosition => !IsMoving && Math.Abs(_commandPosition - _actualPosition) < 0.001;

    /// <inheritdoc/>
    public bool HasError => _state == AxisState.ErrorStop;

    /// <inheritdoc/>
    public int ErrorCode => _errorCode;

    /// <inheritdoc/>
    public string ErrorMessage => _errorCode != 0 ? $"Simulated error {_errorCode}" : "No error";

    /// <inheritdoc/>
    public double ActualPosition => _actualPosition;

    /// <inheritdoc/>
    public double CommandPosition => _commandPosition;

    /// <inheritdoc/>
    public double ActualVelocity => _velocity;

    /// <inheritdoc/>
    public double TargetPosition => _targetPosition;

    /// <inheritdoc/>
    public double PositionError => _commandPosition - _actualPosition;

    /// <inheritdoc/>
    public bool PositiveLimitActive => _actualPosition >= _config.SoftLimitPositive;

    /// <inheritdoc/>
    public bool NegativeLimitActive => _actualPosition <= _config.SoftLimitNegative;

    /// <inheritdoc/>
    public bool HomeSwitchActive => Math.Abs(_actualPosition) < 1.0;

    /// <inheritdoc/>
    public bool AlarmActive => false;

    #endregion

    #region Control Methods

    /// <inheritdoc/>
    public void Enable()
    {
        _state = AxisState.Standstill;
    }

    /// <inheritdoc/>
    public void Disable()
    {
        _state = AxisState.Disabled;
        _velocity = 0;
    }

    /// <inheritdoc/>
    public void ResetError()
    {
        _errorCode = 0;
        if (_state == AxisState.ErrorStop)
            _state = IsEnabled ? AxisState.Standstill : AxisState.Disabled;
    }

    /// <inheritdoc/>
    public void SetZeroPosition()
    {
        _commandPosition = 0;
        _actualPosition = 0;
    }

    /// <inheritdoc/>
    public void SetPosition(double position)
    {
        _commandPosition = position;
        _actualPosition = position;
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

        _targetPosition = 0;
        _maxVelocity = velocity;
        _state = AxisState.Homing;
    }

    /// <inheritdoc/>
    public void MoveAbsolute(double position, MotionProfile profile)
    {
        if (!IsEnabled)
        {
            _errorCode = -100;
            return;
        }

        _targetPosition = position;
        _maxVelocity = profile.Velocity;
        _state = AxisState.DiscreteMotion;
    }

    /// <inheritdoc/>
    public void MoveRelative(double distance, MotionProfile profile)
    {
        if (!IsEnabled)
        {
            _errorCode = -100;
            return;
        }

        _targetPosition = _commandPosition + distance;
        _maxVelocity = profile.Velocity;
        _state = AxisState.DiscreteMotion;
    }

    /// <inheritdoc/>
    public void MoveVelocity(double velocity, double acceleration)
    {
        if (!IsEnabled)
        {
            _errorCode = -100;
            return;
        }

        _velocity = velocity;
        _state = AxisState.ContinuousMotion;
    }

    /// <inheritdoc/>
    public void Stop(double deceleration)
    {
        _velocity = 0;
        _targetPosition = _actualPosition;
        _state = AxisState.Stopping;
    }

    /// <inheritdoc/>
    public void EmergencyStop()
    {
        _velocity = 0;
        _targetPosition = _actualPosition;
        _state = AxisState.Standstill;
    }

    #endregion

    #region Configuration

    /// <inheritdoc/>
    public void Configure(AxisConfig config)
    {
        _config = config;
    }

    /// <inheritdoc/>
    public void SetSoftwareLimits(double negative, double positive)
    {
        _config.SoftLimitNegative = negative;
        _config.SoftLimitPositive = positive;
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
        // Simulate motion - move towards target
        if (_state == AxisState.DiscreteMotion || _state == AxisState.Homing)
        {
            double error = _targetPosition - _actualPosition;
            double step = Math.Min(Math.Abs(error), _maxVelocity / 1000.0); // Simple simulation

            if (Math.Abs(error) < 0.001)
            {
                _actualPosition = _targetPosition;
                _commandPosition = _targetPosition;
                _velocity = 0;

                if (_state == AxisState.Homing)
                {
                    _isHomed = true;
                    SetZeroPosition();
                }

                _state = AxisState.Standstill;
            }
            else
            {
                _actualPosition += Math.Sign(error) * step;
                _commandPosition = _actualPosition;
                _velocity = Math.Sign(error) * _maxVelocity;
            }
        }
        else if (_state == AxisState.ContinuousMotion)
        {
            _actualPosition += _velocity / 1000.0;
            _commandPosition = _actualPosition;
        }
        else if (_state == AxisState.Stopping)
        {
            _state = AxisState.Standstill;
        }
    }
}
