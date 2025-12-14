using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;
using DBI.Runtime.Motion.Interfaces;

namespace DBI.Runtime.Motion.FunctionBlocks;

/// <summary>
/// MC_Stop - Stop axis motion with deceleration (PLCopen standard).
/// </summary>
public class MC_Stop : FunctionBlockBase
{
    private bool _active;

    public MC_Stop(ITimeSource timeSource) : base(timeSource) { }

    #region Inputs

    /// <summary>
    /// Reference to the axis.
    /// </summary>
    public IServoAxis? Axis { get; set; }

    /// <summary>
    /// Execute stop while TRUE.
    /// </summary>
    public bool ExecuteCmd { get; set; }

    /// <summary>
    /// Deceleration for stopping.
    /// </summary>
    public double Deceleration { get; set; } = 100000;

    #endregion

    #region Outputs

    /// <summary>
    /// Axis has stopped.
    /// </summary>
    public bool Done { get; private set; }

    /// <summary>
    /// Stop is being executed.
    /// </summary>
    public bool Busy { get; private set; }

    /// <summary>
    /// Error occurred.
    /// </summary>
    public bool Error { get; private set; }

    /// <summary>
    /// Error ID.
    /// </summary>
    public int ErrorID { get; private set; }

    #endregion

    public override void Execute()
    {
        if (Axis == null)
        {
            Error = true;
            ErrorID = -1;
            return;
        }

        if (ExecuteCmd && !_active)
        {
            Axis.Stop(Deceleration);
            _active = true;
            Busy = true;
            Done = false;
        }

        if (_active)
        {
            if (!Axis.IsMoving)
            {
                Done = true;
                Busy = false;

                if (!ExecuteCmd)
                {
                    _active = false;
                }
            }
        }

        if (!ExecuteCmd && !Axis.IsMoving)
        {
            Done = false;
            _active = false;
        }
    }
}

/// <summary>
/// MC_Reset - Reset axis errors (PLCopen standard).
/// </summary>
public class MC_Reset : FunctionBlockBase
{
    private bool _lastExecute;

    public MC_Reset(ITimeSource timeSource) : base(timeSource) { }

    #region Inputs

    /// <summary>
    /// Reference to the axis.
    /// </summary>
    public IServoAxis? Axis { get; set; }

    /// <summary>
    /// Execute reset on rising edge.
    /// </summary>
    public bool ExecuteCmd { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Reset complete.
    /// </summary>
    public bool Done { get; private set; }

    /// <summary>
    /// Reset in progress.
    /// </summary>
    public bool Busy { get; private set; }

    /// <summary>
    /// Error during reset.
    /// </summary>
    public bool Error { get; private set; }

    /// <summary>
    /// Error ID.
    /// </summary>
    public int ErrorID { get; private set; }

    #endregion

    public override void Execute()
    {
        if (Axis == null)
        {
            Error = true;
            ErrorID = -1;
            return;
        }

        bool risingEdge = ExecuteCmd && !_lastExecute;
        _lastExecute = ExecuteCmd;

        if (risingEdge)
        {
            Axis.ResetError();
            Busy = true;
        }

        if (Busy)
        {
            if (!Axis.HasError)
            {
                Done = true;
                Busy = false;
            }
        }

        if (!ExecuteCmd)
        {
            Done = false;
        }
    }
}

/// <summary>
/// MC_ReadActualPosition - Read actual axis position (PLCopen standard).
/// </summary>
public class MC_ReadActualPosition : FunctionBlockBase
{
    public MC_ReadActualPosition(ITimeSource timeSource) : base(timeSource) { }

    #region Inputs

    /// <summary>
    /// Reference to the axis.
    /// </summary>
    public IServoAxis? Axis { get; set; }

    /// <summary>
    /// Enable reading.
    /// </summary>
    public bool Enable { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Output is valid.
    /// </summary>
    public bool Valid { get; private set; }

    /// <summary>
    /// Error occurred.
    /// </summary>
    public bool Error { get; private set; }

    /// <summary>
    /// Error ID.
    /// </summary>
    public int ErrorID { get; private set; }

    /// <summary>
    /// Actual position value.
    /// </summary>
    public double Position { get; private set; }

    #endregion

    public override void Execute()
    {
        if (Axis == null)
        {
            Error = true;
            ErrorID = -1;
            Valid = false;
            return;
        }

        if (Enable)
        {
            Position = Axis.ActualPosition;
            Valid = true;
            Error = false;
        }
        else
        {
            Valid = false;
        }
    }
}

/// <summary>
/// MC_MoveVelocity - Continuous velocity motion (PLCopen standard).
/// </summary>
public class MC_MoveVelocity : FunctionBlockBase
{
    private bool _lastExecute;
    private bool _active;

    public MC_MoveVelocity(ITimeSource timeSource) : base(timeSource) { }

    #region Inputs

    /// <summary>
    /// Reference to the axis.
    /// </summary>
    public IServoAxis? Axis { get; set; }

    /// <summary>
    /// Execute on rising edge.
    /// </summary>
    public bool ExecuteCmd { get; set; }

    /// <summary>
    /// Target velocity (sign = direction).
    /// </summary>
    public double Velocity { get; set; }

    /// <summary>
    /// Acceleration.
    /// </summary>
    public double Acceleration { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// At target velocity.
    /// </summary>
    public bool InVelocity { get; private set; }

    /// <summary>
    /// Motion in progress.
    /// </summary>
    public bool Busy { get; private set; }

    /// <summary>
    /// Motion active.
    /// </summary>
    public bool Active { get; private set; }

    /// <summary>
    /// Command aborted.
    /// </summary>
    public bool CommandAborted { get; private set; }

    /// <summary>
    /// Error occurred.
    /// </summary>
    public bool Error { get; private set; }

    /// <summary>
    /// Error ID.
    /// </summary>
    public int ErrorID { get; private set; }

    #endregion

    public override void Execute()
    {
        if (Axis == null)
        {
            Error = true;
            ErrorID = -1;
            return;
        }

        bool risingEdge = ExecuteCmd && !_lastExecute;
        _lastExecute = ExecuteCmd;

        if (risingEdge && !_active)
        {
            Axis.MoveVelocity(Velocity, Acceleration);
            _active = true;
            Busy = true;
            Active = true;
            InVelocity = false;
            CommandAborted = false;
            Error = false;
        }

        if (_active)
        {
            if (Axis.HasError)
            {
                Error = true;
                ErrorID = Axis.ErrorCode;
                _active = false;
                Busy = false;
                Active = false;
            }
            else if (Axis.State == AxisState.ContinuousMotion)
            {
                InVelocity = Math.Abs(Axis.ActualVelocity - Velocity) < Math.Abs(Velocity * 0.01);
            }
            else if (!ExecuteCmd)
            {
                _active = false;
                Busy = false;
                Active = false;
                InVelocity = false;
            }
        }

        if (!ExecuteCmd)
        {
            CommandAborted = false;
            Error = false;
        }
    }
}
