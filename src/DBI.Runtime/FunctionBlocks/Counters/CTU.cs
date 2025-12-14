using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Counters;

/// <summary>
/// CTU - Count Up Counter (IEC 61131-3).
/// Increments CV on each rising edge of CU. Q is TRUE when CV >= PV.
/// </summary>
public class CTU : FunctionBlockBase
{
    private bool _lastCu;

    /// <summary>
    /// Creates a new CTU counter.
    /// </summary>
    public CTU(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Count Up input. CV increments on rising edge.
    /// </summary>
    public bool CU { get; set; }

    /// <summary>
    /// Reset input. When TRUE, CV is set to 0.
    /// </summary>
    public bool R { get; set; }

    /// <summary>
    /// Preset Value. Target count for Q to become TRUE.
    /// </summary>
    public int PV { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Counter output. TRUE when CV >= PV.
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
        // Reset has priority
        if (R)
        {
            CV = 0;
        }
        else
        {
            // Detect rising edge of CU
            bool risingEdge = CU && !_lastCu;

            if (risingEdge && CV < int.MaxValue)
            {
                CV++;
            }
        }

        // Update output
        Q = CV >= PV;
        _lastCu = CU;
    }

    /// <summary>
    /// Resets the counter to initial state.
    /// </summary>
    public void Reset()
    {
        _lastCu = false;
        CV = 0;
        Q = false;
    }
}
