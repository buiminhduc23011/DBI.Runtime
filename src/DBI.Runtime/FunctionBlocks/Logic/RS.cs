using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Logic;

/// <summary>
/// RS - Reset/Set Flip-Flop (Reset Dominant).
/// When both SET and RESET1 are TRUE, output Q becomes FALSE (RESET has priority).
/// </summary>
public class RS : FunctionBlockBase
{
    /// <summary>
    /// Creates a new RS flip-flop.
    /// </summary>
    public RS(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Set input. Sets Q to TRUE (only if RESET1 is FALSE).
    /// </summary>
    public bool SET { get; set; }

    /// <summary>
    /// Reset input (dominant). Resets Q to FALSE.
    /// </summary>
    public bool RESET1 { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Output state.
    /// </summary>
    public bool Q1 { get; private set; }

    #endregion

    /// <inheritdoc/>
    public override void Execute()
    {
        // RESET dominant: Q = NOT RESET1 AND (SET OR Q)
        Q1 = !RESET1 && (SET || Q1);
    }

    /// <summary>
    /// Resets to initial state.
    /// </summary>
    public void Reset()
    {
        Q1 = false;
    }
}
