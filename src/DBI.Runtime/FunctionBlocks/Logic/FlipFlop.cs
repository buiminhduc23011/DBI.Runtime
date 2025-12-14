using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Logic;

/// <summary>
/// FlipFlop - Toggle Flip-Flop.
/// Output Q toggles on each rising edge of CLK.
/// </summary>
public class FlipFlop : FunctionBlockBase
{
    private bool _lastClk;

    /// <summary>
    /// Creates a new FlipFlop.
    /// </summary>
    public FlipFlop(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Clock input. Q toggles on rising edge.
    /// </summary>
    public bool CLK { get; set; }

    /// <summary>
    /// Reset input. When TRUE, Q is set to FALSE.
    /// </summary>
    public bool RST { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Output state.
    /// </summary>
    public bool Q { get; private set; }

    #endregion

    /// <inheritdoc/>
    public override void Execute()
    {
        if (RST)
        {
            Q = false;
        }
        else
        {
            // Toggle on rising edge
            bool risingEdge = CLK && !_lastClk;
            if (risingEdge)
            {
                Q = !Q;
            }
        }

        _lastClk = CLK;
    }

    /// <summary>
    /// Resets to initial state.
    /// </summary>
    public void Reset()
    {
        _lastClk = false;
        Q = false;
    }
}
