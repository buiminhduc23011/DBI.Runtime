using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Timers;

/// <summary>
/// TONR - Retentive On-Delay Timer (IEC 61131-3).
/// Accumulates time when IN is TRUE. Does not reset when IN goes FALSE.
/// Must be explicitly reset via R input.
/// </summary>
/// <remarks>
/// Similar to TON but retains accumulated time across multiple IN pulses.
/// Used for applications like total run-time tracking.
/// </remarks>
public class TONR : FunctionBlockBase
{
    private TimeSpan _accumulatedTime;
    private TimeSpan _lastActiveTime;
    private bool _wasActive;

    /// <summary>
    /// Creates a new TONR timer.
    /// </summary>
    public TONR(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Timer input. While TRUE, timer accumulates time.
    /// </summary>
    public bool IN { get; set; }

    /// <summary>
    /// Reset input. When TRUE, resets accumulated time to zero.
    /// </summary>
    public bool R { get; set; }

    /// <summary>
    /// Preset Time. Target accumulated time for Q to become TRUE.
    /// </summary>
    public TimeSpan PT { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Timer output. TRUE when accumulated time >= PT.
    /// </summary>
    public bool Q { get; private set; }

    /// <summary>
    /// Elapsed Time. Total accumulated time.
    /// </summary>
    public TimeSpan ET { get; private set; }

    #endregion

    /// <inheritdoc/>
    public override void Execute()
    {
        // Handle reset first (reset has priority)
        if (R)
        {
            _accumulatedTime = TimeSpan.Zero;
            _wasActive = false;
            Q = false;
            ET = TimeSpan.Zero;
            return;
        }

        if (IN)
        {
            if (!_wasActive)
            {
                // Rising edge - record start time
                _lastActiveTime = CurrentTime;
                _wasActive = true;
            }
            else
            {
                // Still active - accumulate time since last cycle
                var currentTime = CurrentTime;
                var delta = currentTime - _lastActiveTime;
                _accumulatedTime += delta;
                _lastActiveTime = currentTime;
            }
        }
        else
        {
            // IN is FALSE - retain accumulated time but stop counting
            _wasActive = false;
        }

        // Clamp accumulated time to PT
        ET = _accumulatedTime > PT ? PT : _accumulatedTime;

        // Set output
        Q = _accumulatedTime >= PT;
    }

    /// <summary>
    /// Resets the timer to initial state.
    /// </summary>
    public void Reset()
    {
        _accumulatedTime = TimeSpan.Zero;
        _lastActiveTime = TimeSpan.Zero;
        _wasActive = false;
        Q = false;
        ET = TimeSpan.Zero;
    }
}
