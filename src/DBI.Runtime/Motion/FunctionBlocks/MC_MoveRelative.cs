using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;
using DBI.Runtime.Motion.Interfaces;

namespace DBI.Runtime.Motion.FunctionBlocks;

/// <summary>
/// MC_MoveRelative - Move axis relative distance (PLCopen standard).
/// </summary>
public class MC_MoveRelative : FunctionBlockBase
{
    private bool _lastExecute;
    private bool _active;

    public MC_MoveRelative(ITimeSource timeSource) : base(timeSource) { }

    #region Inputs

    /// <summary>
    /// Reference to the axis.
    /// </summary>
    public IServoAxis? Axis { get; set; }

    /// <summary>
    /// Starts motion on rising edge.
    /// </summary>
    public bool ExecuteCmd { get; set; }

    /// <summary>
    /// Distance to move.
    /// </summary>
    public double Distance { get; set; }

    /// <summary>
    /// Maximum velocity.
    /// </summary>
    public double Velocity { get; set; }

    /// <summary>
    /// Acceleration.
    /// </summary>
    public double Acceleration { get; set; }

    /// <summary>
    /// Deceleration.
    /// </summary>
    public double Deceleration { get; set; }

    /// <summary>
    /// Jerk (0 = trapezoidal, >0 = S-curve).
    /// </summary>
    public double Jerk { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Motion complete.
    /// </summary>
    public bool Done { get; private set; }

    /// <summary>
    /// Function block is active.
    /// </summary>
    public bool Busy { get; private set; }

    /// <summary>
    /// Motion command is being executed.
    /// </summary>
    public bool Active { get; private set; }

    /// <summary>
    /// Command was aborted.
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
            var profile = new MotionProfile
            {
                Velocity = Velocity,
                Acceleration = Acceleration,
                Deceleration = Deceleration > 0 ? Deceleration : Acceleration,
                Jerk = Jerk
            };

            Axis.MoveRelative(Distance, profile);
            _active = true;
            Busy = true;
            Active = true;
            Done = false;
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
            else if (Axis.IsInPosition)
            {
                Done = true;
                _active = false;
                Busy = false;
                Active = false;
            }
            else if (Axis.State == AxisState.Standstill && !Axis.IsMoving)
            {
                CommandAborted = true;
                _active = false;
                Busy = false;
                Active = false;
            }
        }

        if (!ExecuteCmd)
        {
            Done = false;
            CommandAborted = false;
            Error = false;
        }
    }
}
