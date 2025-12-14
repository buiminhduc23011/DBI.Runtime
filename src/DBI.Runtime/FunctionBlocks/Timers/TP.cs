using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.FunctionBlocks.Base;

namespace DBI.Runtime.FunctionBlocks.Timers;

/// <summary>
/// TP - Pulse Timer (IEC 61131-3).
/// Generates a pulse of duration PT on rising edge of IN.
/// </summary>
/// <remarks>
/// Timing diagram:
/// IN:  |--|  |-----|  |--|
/// Q:   |-----|     |-----|
///        ^PT         ^PT
/// Once triggered, Q stays TRUE for PT duration, regardless of IN state.
/// Cannot be retriggered during pulse.
/// </remarks>
public class TP : FunctionBlockBase
{
    private TimeSpan _startTime;
    private bool _timing;
    private bool _lastIn;

    /// <summary>
    /// Creates a new TP timer.
    /// </summary>
    public TP(ITimeSource timeSource) : base(timeSource)
    {
    }

    #region Inputs

    /// <summary>
    /// Timer input. Rising edge triggers pulse.
    /// </summary>
    public bool IN { get; set; }

    /// <summary>
    /// Preset Time. Duration of the pulse.
    /// </summary>
    public TimeSpan PT { get; set; }

    #endregion

    #region Outputs

    /// <summary>
    /// Timer output. TRUE for PT duration after rising edge of IN.
    /// </summary>
    public bool Q { get; private set; }

    /// <summary>
    /// Elapsed Time. Current elapsed time during pulse.
    /// </summary>
    public TimeSpan ET { get; private set; }

    #endregion

    /// <inheritdoc/>
    public override void Execute()
    {
        // Detect rising edge of IN
        bool risingEdge = IN && !_lastIn;

        if (_timing)
        {
            // Currently generating pulse
            ET = CurrentTime - _startTime;

            if (ET >= PT)
            {
                // Pulse complete
                ET = PT;
                Q = false;
                _timing = false;
            }
            else
            {
                Q = true;
            }
        }
        else if (risingEdge)
        {
            // Start new pulse on rising edge
            _startTime = CurrentTime;
            _timing = true;
            ET = TimeSpan.Zero;
            Q = true;
        }
        else
        {
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
