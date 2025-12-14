using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Timers;

/// <summary>
/// TON - On-Delay Timer (IEC 61131-3).
/// Output Q becomes TRUE when input IN has been TRUE for at least PT duration.
/// </summary>
/// <remarks>
/// Timing diagram:
/// IN:  |------|      |------------|
/// Q:   |      |------|            |-------|
///           ^PT              ^PT
/// ET counts from 0 to PT while IN is TRUE. Resets when IN goes FALSE.
/// </remarks>
public class TON : FunctionBlockBase
{
    private TimeSpan _startTime;
    private bool _timing;
    private bool _lastIn;

    /// <summary>
    /// Creates a new TON timer.
    /// </summary>
    public TON(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Timer input. When TRUE, timer starts counting.
    /// </summary>
    public bool IN { get; set; }

    /// <summary>
    /// Preset Time. Duration before Q becomes TRUE.
    /// </summary>
    public TimeSpan PT { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Timer output. Becomes TRUE when IN has been TRUE for >= PT.
    /// </summary>
    public bool Q { get; private set; }

    /// <summary>
    /// Elapsed Time. Current elapsed time while timing.
    /// </summary>
    public TimeSpan ET { get; private set; }

    #endregion

    /// <inheritdoc/>
    public override void Execute()
    {
        if (IN)
        {
            if (!_timing)
            {
                // Rising edge - start timing
                _startTime = CurrentTime;
                _timing = true;
            }

            // Calculate elapsed time
            ET = CurrentTime - _startTime;

            // Clamp ET to PT
            if (ET > PT)
                ET = PT;

            // Set output when time elapsed
            Q = ET >= PT;
        }
        else
        {
            // IN is FALSE - reset timer
            _timing = false;
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
