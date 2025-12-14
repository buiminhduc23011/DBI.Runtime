using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.EdgeDetection;

/// <summary>
/// F_TRIG - Falling Edge Detection (IEC 61131-3).
/// Output Q is TRUE for exactly one scan cycle when CLK transitions from TRUE to FALSE.
/// </summary>
public class F_TRIG : FunctionBlockBase
{
    private bool _lastClk;
    private bool _initialized;

    /// <summary>
    /// Creates a new F_TRIG edge detector.
    /// </summary>
    public F_TRIG(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Clock input. Monitored for falling edge.
    /// </summary>
    public bool CLK { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Output. TRUE for one scan on falling edge of CLK.
    /// </summary>
    public bool Q { get; private set; }

    #endregion

    /// <inheritdoc/>
    public override void Execute()
    {
        if (!_initialized)
        {
            _lastClk = CLK;
            _initialized = true;
            Q = false;
            return;
        }

        // Falling edge: CLK is FALSE and was TRUE
        Q = !CLK && _lastClk;
        _lastClk = CLK;
    }

    /// <summary>
    /// Resets the edge detector.
    /// </summary>
    public void Reset()
    {
        _lastClk = false;
        _initialized = false;
        Q = false;
    }
}
