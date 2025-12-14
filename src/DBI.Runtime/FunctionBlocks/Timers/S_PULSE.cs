using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Timers;

/// <summary>
/// S_PULSE - S5 Pulse Timer (Siemens S5/S7 compatible).
/// Generates a pulse of duration TV on rising edge of S.
/// </summary>
public class S_PULSE : FunctionBlockBase
{
    private TimeSpan _startTime;
    private bool _timing;
    private bool _lastS;

    /// <summary>
    /// Creates a new S_PULSE timer.
    /// </summary>
    public S_PULSE(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Start input. Rising edge triggers pulse.
    /// </summary>
    public bool S { get; set; }

    /// <summary>
    /// Timer Value (pulse duration).
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

        // Detect rising edge
        bool risingEdge = S && !_lastS;

        if (_timing)
        {
            // S must remain TRUE during pulse (unlike TP)
            if (!S)
            {
                // S went FALSE during pulse - stop
                _timing = false;
                Q = false;
                BI = TimeSpan.Zero;
            }
            else
            {
                var elapsed = CurrentTime - _startTime;
                var remaining = TV - elapsed;

                if (remaining <= TimeSpan.Zero)
                {
                    // Pulse complete
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
        }
        else if (risingEdge)
        {
            // Start pulse
            _startTime = CurrentTime;
            _timing = true;
            Q = true;
            BI = TV;
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
