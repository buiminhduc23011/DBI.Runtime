using System.Diagnostics;
using DBI.Runtime.Core.Interfaces;

namespace DBI.Runtime.Core.Runtime;

/// <summary>
/// Monotonic time source using high-resolution timer.
/// Provides deterministic time behavior for PLC timers.
/// </summary>
public class MonotonicTimeSource : ITimeSource
{
    private readonly Stopwatch _stopwatch;
    private TimeSpan _simulatedTime;
    private readonly bool _realTime;

    /// <summary>
    /// Creates a real-time time source using system clock.
    /// </summary>
    public MonotonicTimeSource() : this(realTime: true)
    {
    }

    /// <summary>
    /// Creates a time source.
    /// </summary>
    /// <param name="realTime">If true, uses system clock. If false, uses simulated time.</param>
    public MonotonicTimeSource(bool realTime)
    {
        _realTime = realTime;
        _stopwatch = new Stopwatch();
        _simulatedTime = TimeSpan.Zero;

        if (_realTime)
            _stopwatch.Start();
    }

    /// <inheritdoc/>
    public TimeSpan Now => _realTime ? _stopwatch.Elapsed : _simulatedTime;

    /// <inheritdoc/>
    public void Tick(TimeSpan elapsed)
    {
        if (!_realTime)
        {
            _simulatedTime += elapsed;
        }
    }

    /// <inheritdoc/>
    public void Reset()
    {
        if (_realTime)
        {
            _stopwatch.Restart();
        }
        else
        {
            _simulatedTime = TimeSpan.Zero;
        }
    }

    /// <summary>
    /// Creates a simulated time source for testing.
    /// </summary>
    public static MonotonicTimeSource CreateSimulated() => new(realTime: false);

    /// <summary>
    /// Creates a real-time time source.
    /// </summary>
    public static MonotonicTimeSource CreateRealTime() => new(realTime: true);
}
