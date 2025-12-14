namespace DBI.Runtime.Motion.Interfaces;

/// <summary>
/// Interface for a motion control axis (servo/stepper).
/// </summary>
public interface IServoAxis
{
    /// <summary>
    /// Gets the axis index (0-based).
    /// </summary>
    int AxisIndex { get; }

    /// <summary>
    /// Gets or sets the axis name.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets the current axis state.
    /// </summary>
    AxisState State { get; }

    #region Status Properties

    /// <summary>
    /// Gets whether the axis is enabled (servo on).
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets whether the axis is currently moving.
    /// </summary>
    bool IsMoving { get; }

    /// <summary>
    /// Gets whether the axis has been homed.
    /// </summary>
    bool IsHomed { get; }

    /// <summary>
    /// Gets whether the axis is in position (motion complete).
    /// </summary>
    bool IsInPosition { get; }

    /// <summary>
    /// Gets whether the axis has an error.
    /// </summary>
    bool HasError { get; }

    /// <summary>
    /// Gets the current error code (0 = no error).
    /// </summary>
    int ErrorCode { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    string ErrorMessage { get; }

    #endregion

    #region Position/Velocity Properties

    /// <summary>
    /// Gets the current actual position (feedback).
    /// </summary>
    double ActualPosition { get; }

    /// <summary>
    /// Gets the current command position.
    /// </summary>
    double CommandPosition { get; }

    /// <summary>
    /// Gets the current actual velocity.
    /// </summary>
    double ActualVelocity { get; }

    /// <summary>
    /// Gets the target position of current motion.
    /// </summary>
    double TargetPosition { get; }

    /// <summary>
    /// Gets the position error (command - actual).
    /// </summary>
    double PositionError { get; }

    #endregion

    #region I/O Status

    /// <summary>
    /// Gets the positive limit switch status.
    /// </summary>
    bool PositiveLimitActive { get; }

    /// <summary>
    /// Gets the negative limit switch status.
    /// </summary>
    bool NegativeLimitActive { get; }

    /// <summary>
    /// Gets the home/origin switch status.
    /// </summary>
    bool HomeSwitchActive { get; }

    /// <summary>
    /// Gets the servo alarm status.
    /// </summary>
    bool AlarmActive { get; }

    #endregion

    #region Control Methods

    /// <summary>
    /// Enables the axis (servo on).
    /// </summary>
    void Enable();

    /// <summary>
    /// Disables the axis (servo off).
    /// </summary>
    void Disable();

    /// <summary>
    /// Resets the axis error.
    /// </summary>
    void ResetError();

    /// <summary>
    /// Sets the current position as zero (home position).
    /// </summary>
    void SetZeroPosition();

    /// <summary>
    /// Sets the current position to a specific value.
    /// </summary>
    void SetPosition(double position);

    #endregion

    #region Motion Methods

    /// <summary>
    /// Starts homing sequence.
    /// </summary>
    void StartHome(HomingMode mode, double velocity, double acceleration);

    /// <summary>
    /// Moves to absolute position.
    /// </summary>
    void MoveAbsolute(double position, MotionProfile profile);

    /// <summary>
    /// Moves relative distance from current position.
    /// </summary>
    void MoveRelative(double distance, MotionProfile profile);

    /// <summary>
    /// Starts continuous velocity motion.
    /// </summary>
    void MoveVelocity(double velocity, double acceleration);

    /// <summary>
    /// Stops motion with deceleration.
    /// </summary>
    void Stop(double deceleration);

    /// <summary>
    /// Emergency stop (immediate).
    /// </summary>
    void EmergencyStop();

    #endregion

    #region Configuration

    /// <summary>
    /// Configures the axis parameters.
    /// </summary>
    void Configure(AxisConfig config);

    /// <summary>
    /// Sets software limits.
    /// </summary>
    void SetSoftwareLimits(double negative, double positive);

    /// <summary>
    /// Sets velocity limit.
    /// </summary>
    void SetVelocityLimit(double maxVelocity);

    #endregion

    /// <summary>
    /// Updates the axis status (call each scan cycle).
    /// </summary>
    void UpdateStatus();
}
