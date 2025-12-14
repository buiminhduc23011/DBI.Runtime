using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;
using DBI.Runtime.Motion.Interfaces;

namespace DBI.Runtime.Motion.FunctionBlocks;

/// <summary>
/// MC_MoveAbsolute - Move axis to absolute position (PLCopen standard).
/// </summary>
public class MC_MoveAbsolute : FunctionBlockBase
{
    private bool _lastExecute;
    private bool _active;

    public MC_MoveAbsolute(ITimeSource timeSource) : base(timeSource) { }

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
    /// Target position.
    /// </summary>
    public double Position { get; set; }

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
    /// Motion complete, position reached.
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
    /// Command was aborted by another command.
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

        // Detect rising edge
        bool risingEdge = ExecuteCmd && !_lastExecute;
        _lastExecute = ExecuteCmd;

        if (risingEdge && !_active)
        {
            // Start motion
            var profile = new MotionProfile
            {
                Velocity = Velocity,
                Acceleration = Acceleration,
                Deceleration = Deceleration > 0 ? Deceleration : Acceleration,
                Jerk = Jerk
            };

            Axis.MoveAbsolute(Position, profile);
            _active = true;
            Busy = true;
            Active = true;
            Done = false;
            CommandAborted = false;
            Error = false;
            ErrorID = 0;
        }

        if (_active)
        {
            // Check status
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
                // Motion was aborted by another command
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
