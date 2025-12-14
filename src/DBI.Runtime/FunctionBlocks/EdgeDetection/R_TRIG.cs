using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.EdgeDetection;

/// <summary>
/// R_TRIG - Rising Edge Detection (IEC 61131-3).
/// Output Q is TRUE for exactly one scan cycle when CLK transitions from FALSE to TRUE.
/// </summary>
public class R_TRIG : FunctionBlockBase
{
    private bool _lastClk;

    /// <summary>
    /// Creates a new R_TRIG edge detector.
    /// </summary>
    public R_TRIG(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Clock input. Monitored for rising edge.
    /// </summary>
    public bool CLK { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Output. TRUE for one scan on rising edge of CLK.
    /// </summary>
    public bool Q { get; private set; }

    #endregion

    /// <inheritdoc/>
    public override void Execute()
    {
        // Rising edge: CLK is TRUE and was FALSE
        Q = CLK && !_lastClk;
        _lastClk = CLK;
    }

    /// <summary>
    /// Resets the edge detector.
    /// </summary>
    public void Reset()
    {
        _lastClk = false;
        Q = false;
    }
}
