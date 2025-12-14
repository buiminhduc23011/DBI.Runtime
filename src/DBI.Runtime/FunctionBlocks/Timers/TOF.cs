using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Timers;

/// <summary>
/// TOF - Off-Delay Timer (IEC 61131-3).
/// Output Q becomes TRUE immediately when IN is TRUE, and stays TRUE for PT duration after IN becomes FALSE.
/// </summary>
/// <remarks>
/// Timing diagram:
/// IN:  |------|      |-----|
/// Q:   |------------|      |----------|
///              ^PT               ^PT
/// </remarks>
public class TOF : FunctionBlockBase
{
    private TimeSpan _startTime;
    private bool _timing;
    private bool _lastIn;

    /// <summary>
    /// Creates a new TOF timer.
    /// </summary>
    public TOF(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Timer input.
    /// </summary>
    public bool IN { get; set; }

    /// <summary>
    /// Preset Time. Duration to keep Q TRUE after IN goes FALSE.
    /// </summary>
    public TimeSpan PT { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Timer output. TRUE while IN is TRUE or during off-delay period.
    /// </summary>
    public bool Q { get; private set; }

    /// <summary>
    /// Elapsed Time. Current elapsed time during off-delay period.
    /// </summary>
    public TimeSpan ET { get; private set; }

    #endregion

    /// <inheritdoc/>
    public override void Execute()
    {
        // Detect falling edge of IN
        bool fallingEdge = _lastIn && !IN;

        if (IN)
        {
            // IN is TRUE - output is TRUE, no timing
            Q = true;
            ET = TimeSpan.Zero;
            _timing = false;
        }
        else if (fallingEdge)
        {
            // Falling edge - start off-delay timing
            _startTime = CurrentTime;
            _timing = true;
            Q = true;
        }
        else if (_timing)
        {
            // During off-delay period
            ET = CurrentTime - _startTime;

            if (ET >= PT)
            {
                // Time elapsed - turn off output
                ET = PT;
                Q = false;
                _timing = false;
            }
            else
            {
                Q = true;
            }
        }
        else
        {
            // Not timing and IN is FALSE
            Q = false;
            ET = TimeSpan.Zero;
        }

        _lastIn = IN;
    }

    /// <summary>
    /// Resets the timer to initial state.
    /// </summary>
    public void Reset()
    {
        _timing = false;
        _lastIn = false;
        Q = false;
        ET = TimeSpan.Zero;
    }
}
