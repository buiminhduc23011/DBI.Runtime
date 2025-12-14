using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;
using DBI.Runtime.Motion.Interfaces;

namespace DBI.Runtime.Motion.FunctionBlocks;

/// <summary>
/// MC_Home - Execute homing sequence (PLCopen standard).
/// </summary>
public class MC_Home : FunctionBlockBase
{
    private bool _lastExecute;
    private bool _active;

    public MC_Home(ITimeSource timeSource) : base(timeSource) { }

    #region Inputs

    /// <summary>
    /// Reference to the axis.
    /// </summary>
    public IServoAxis? Axis { get; set; }

    /// <summary>
    /// Starts homing on rising edge.
    /// </summary>
    public bool ExecuteCmd { get; set; }

    /// <summary>
    /// Homing mode.
    /// </summary>
    public HomingMode Mode { get; set; } = HomingMode.HomeSwitch;

    /// <summary>
    /// Homing velocity.
    /// </summary>
    public double Velocity { get; set; } = 10000;

    /// <summary>
    /// Homing acceleration.
    /// </summary>
    public double Acceleration { get; set; } = 50000;

    /// <summary>
    /// Position value after homing complete.
    /// </summary>
    public double Position { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Homing complete.
    /// </summary>
    public bool Done { get; private set; }

    /// <summary>
    /// Function block is busy.
    /// </summary>
    public bool Busy { get; private set; }

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
            Axis.StartHome(Mode, Velocity, Acceleration);
            _active = true;
            Busy = true;
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
            }
            else if (Axis.IsHomed && Axis.State == AxisState.Standstill)
            {
                Done = true;
                _active = false;
                Busy = false;
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
