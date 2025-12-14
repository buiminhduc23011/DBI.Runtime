using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;
using DBI.Runtime.Motion.Interfaces;

namespace DBI.Runtime.Motion.FunctionBlocks;

/// <summary>
/// MC_Power - Enable/disable servo axis power (PLCopen standard).
/// </summary>
public class MC_Power : FunctionBlockBase
{
    private bool _lastEnable;

    public MC_Power(ITimeSource timeSource) : base(timeSource) { }

    #region Inputs

    /// <summary>
    /// Reference to the axis.
    /// </summary>
    public IServoAxis? Axis { get; set; }

    /// <summary>
    /// Enable power. TRUE = servo on.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Enable positive direction movement.
    /// </summary>
    public bool EnablePositive { get; set; } = true;

    /// <summary>
    /// Enable negative direction movement.
    /// </summary>
    public bool EnableNegative { get; set; } = true;

    #endregion

    #region Outputs

    /// <summary>
    /// Current power status.
    /// </summary>
    public bool Status { get; private set; }

    /// <summary>
    /// Axis is ready for motion.
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

        // Detect rising/falling edge of Enable
        if (Enable && !_lastEnable)
        {
            // Rising edge - enable axis
            Axis.Enable();
        }
        else if (!Enable && _lastEnable)
        {
            // Falling edge - disable axis
            Axis.Disable();
        }

        _lastEnable = Enable;

        // Update outputs
        Status = Axis.IsEnabled;
        Valid = Axis.IsEnabled && !Axis.HasError;
        Error = Axis.HasError;
        ErrorID = Axis.ErrorCode;
    }
}
