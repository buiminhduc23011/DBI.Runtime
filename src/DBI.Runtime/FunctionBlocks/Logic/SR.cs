using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Logic;

/// <summary>
/// SR - Set/Reset Flip-Flop (Set Dominant).
/// When both SET1 and RESET are TRUE, output Q remains TRUE (SET has priority).
/// </summary>
public class SR : FunctionBlockBase
{
    /// <summary>
    /// Creates a new SR flip-flop.
    /// </summary>
    public SR(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Set input (dominant). Sets Q to TRUE.
    /// </summary>
    public bool SET1 { get; set; }

    /// <summary>
    /// Reset input. Resets Q to FALSE (only if SET1 is FALSE).
    /// </summary>
    public bool RESET { get; set; }

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
        // SET dominant: Q = SET1 OR (Q AND NOT RESET)
        Q1 = SET1 || (Q1 && !RESET);
    }

    /// <summary>
    /// Resets to initial state.
    /// </summary>
    public void Reset()
    {
        Q1 = false;
    }
}
