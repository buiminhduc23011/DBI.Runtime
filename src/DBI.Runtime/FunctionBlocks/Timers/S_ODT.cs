using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Timers;

/// <summary>
/// S_ODT - S5 On-Delay Timer (Siemens S5/S7 compatible).
/// Similar to TON but with additional S5 timer format support.
/// </summary>
/// <remarks>
/// Provides compatibility with legacy S5 timer behavior.
/// Timer starts when S input is TRUE, Q becomes TRUE after TV elapsed.
/// </remarks>
public class S_ODT : FunctionBlockBase
{
    private TimeSpan _startTime;
    private bool _timing;
    private bool _lastS;

    /// <summary>
    /// Creates a new S_ODT timer.
    /// </summary>
    public S_ODT(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Start input. Starts timer on rising edge, continues while TRUE.
    /// </summary>
    public bool S { get; set; }

    /// <summary>
    /// Timer Value (preset time).
    /// </summary>
    public TimeSpan TV { get; set; }

    /// <summary>
    /// Reset input. When TRUE, resets the timer.
    /// </summary>
    public bool R { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Timer output. TRUE when timer has elapsed.
    /// </summary>
    public bool Q { get; private set; }

    /// <summary>
    /// Binary output - remaining time as TimeSpan.
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

        if (S)
        {
            if (!_lastS)
            {
                // Rising edge of S - start timer
                _startTime = CurrentTime;
                _timing = true;
            }

            if (_timing)
            {
                var elapsed = CurrentTime - _startTime;
                var remaining = TV - elapsed;

                if (remaining <= TimeSpan.Zero)
                {
                    // Timer elapsed
                    Q = true;
                    BI = TimeSpan.Zero;
                    _timing = false;
                }
                else
                {
                    Q = false;
                    BI = remaining;
                }
            }
        }
        else
        {
            // S is FALSE - reset timer
            _timing = false;
            Q = false;
            BI = TimeSpan.Zero;
        }

        _lastS = S;
    }

    /// <summary>
    /// Resets the timer to initial state.
    /// </summary>
    public void Reset()
    {
        _timing = false;
        _lastS = false;
        Q = false;
        BI = TimeSpan.Zero;
    }
}
