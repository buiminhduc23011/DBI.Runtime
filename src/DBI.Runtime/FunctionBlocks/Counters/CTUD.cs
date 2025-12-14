using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Counters;

/// <summary>
/// CTUD - Up/Down Counter (IEC 61131-3).
/// Counts up on rising edge of CU, down on rising edge of CD.
/// </summary>
public class CTUD : FunctionBlockBase
{
    private bool _lastCu;
    private bool _lastCd;

    /// <summary>
    /// Creates a new CTUD counter.
    /// </summary>
    public CTUD(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Count Up input. CV increments on rising edge.
    /// </summary>
    public bool CU { get; set; }

    /// <summary>
    /// Count Down input. CV decrements on rising edge.
    /// </summary>
    public bool CD { get; set; }

    /// <summary>
    /// Reset input. When TRUE, CV is set to 0.
    /// </summary>
    public bool R { get; set; }

    /// <summary>
    /// Load input. When TRUE, CV is loaded with PV.
    /// </summary>
    public bool LD { get; set; }

    /// <summary>
    /// Preset Value.
    /// </summary>
    public int PV { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Up output. TRUE when CV >= PV.
    /// </summary>
    public bool QU { get; private set; }

    /// <summary>
    /// Down output. TRUE when CV <= 0.
    /// </summary>
    public bool QD { get; private set; }

    /// <summary>
    /// Current Value.
    /// </summary>
    public int CV { get; private set; }

    #endregion

    /// <inheritdoc/>
    public override void Execute()
    {
        // Reset and Load have priority (Reset > Load)
        if (R)
        {
            CV = 0;
        }
        else if (LD)
        {
            CV = PV;
        }
        else
        {
            // Detect rising edges
            bool cuRising = CU && !_lastCu;
            bool cdRising = CD && !_lastCd;

            // Count up
            if (cuRising && CV < int.MaxValue)
            {
                CV++;
            }

            // Count down
            if (cdRising && CV > int.MinValue)
            {
                CV--;
            }
        }

        // Update outputs
        QU = CV >= PV;
        QD = CV <= 0;

        _lastCu = CU;
        _lastCd = CD;
    }

    /// <summary>
    /// Resets the counter to initial state.
    /// </summary>
    public void Reset()
    {
        _lastCu = false;
        _lastCd = false;
        CV = 0;
        QU = false;
        QD = true;
    }
}
