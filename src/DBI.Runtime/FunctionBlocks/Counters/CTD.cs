using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Counters;

/// <summary>
/// CTD - Count Down Counter (IEC 61131-3).
/// Decrements CV on each rising edge of CD. Q is TRUE when CV <= 0.
/// </summary>
public class CTD : FunctionBlockBase
{
    private bool _lastCd;

    /// <summary>
    /// Creates a new CTD counter.
    /// </summary>
    public CTD(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Count Down input. CV decrements on rising edge.
    /// </summary>
    public bool CD { get; set; }

    /// <summary>
    /// Load input. When TRUE, CV is loaded with PV.
    /// </summary>
    public bool LD { get; set; }

    /// <summary>
    /// Preset Value. Value to load into CV.
    /// </summary>
    public int PV { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Counter output. TRUE when CV <= 0.
    /// </summary>
    public bool Q { get; private set; }

    /// <summary>
    /// Current Value. Current count.
    /// </summary>
    public int CV { get; private set; }

    #endregion

    /// <inheritdoc/>
    public override void Execute()
    {
        // Load has priority
        if (LD)
        {
            CV = PV;
        }
        else
        {
            // Detect rising edge of CD
            bool risingEdge = CD && !_lastCd;

            if (risingEdge && CV > int.MinValue)
            {
                CV--;
            }
        }

        // Update output
        Q = CV <= 0;
        _lastCd = CD;
    }

    /// <summary>
    /// Resets the counter to initial state.
    /// </summary>
    public void Reset()
    {
        _lastCd = false;
        CV = 0;
        Q = true; // CV = 0 means Q = TRUE for CTD
    }
}
