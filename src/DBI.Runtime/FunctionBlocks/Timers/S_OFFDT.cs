using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Timers;

/// <summary>
/// S_OFFDT - S5 Off-Delay Timer (Siemens S5/S7 compatible).
/// Similar to TOF but with S5 timer format support.
/// </summary>
public class S_OFFDT : FunctionBlockBase
{
    private TimeSpan _startTime;
    private bool _timing;
    private bool _lastS;

    /// <summary>
    /// Creates a new S_OFFDT timer.
    /// </summary>
    public S_OFFDT(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Start input.
    /// </summary>
    public bool S { get; set; }

    /// <summary>
    /// Timer Value (preset time).
    /// </summary>
    public TimeSpan TV { get; set; }

    /// <summary>
    /// Reset input.
    /// </summary>
    public bool R { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Timer output.
    /// </summary>
    public bool Q { get; private set; }

    /// <summary>
    /// Remaining time.
    /// </summary>
    public TimeSpan BI { get; private set; }

    #endregion

    /// <inheritdoc/>
    public override void Execute()
    {
        // Reset has priority
        if (R)
        {
            _timing = false;
            Q = false;
            BI = TimeSpan.Zero;
            _lastS = S;
            return;
        }

        // Detect falling edge
        bool fallingEdge = _lastS && !S;

        if (S)
        {
            // S is TRUE - output is TRUE, no timing
            Q = true;
            BI = TV;
            _timing = false;
        }
        else if (fallingEdge)
        {
            // Falling edge - start off-delay
            _startTime = CurrentTime;
            _timing = true;
            Q = true;
            BI = TV;
        }
        else if (_timing)
        {
            var elapsed = CurrentTime - _startTime;
            var remaining = TV - elapsed;

            if (remaining <= TimeSpan.Zero)
            {
                // Timer elapsed
                Q = false;
                BI = TimeSpan.Zero;
                _timing = false;
            }
            else
            {
                Q = true;
                BI = remaining;
            }
        }
        else
        {
            Q = false;
            BI = TimeSpan.Zero;
        }

        _lastS = S;
    }

    /// <summary>
    /// Resets the timer.
    /// </summary>
    public void Reset()
    {
        _timing = false;
        _lastS = false;
        Q = false;
        BI = TimeSpan.Zero;
    }
}
