namespace DBI.Runtime.Core.Interfaces;

/// <summary>
/// Abstraction for PLC time source.
/// Provides monotonic time for deterministic timer behavior.
/// </summary>
public interface ITimeSource
{
    /// <summary>
    /// Gets the current monotonic time since runtime start.
    /// </summary>
    TimeSpan Now { get; }

    /// <summary>
    /// Advances the time by the specified elapsed duration.
    /// Used for simulated/test environments.
    /// </summary>
    /// <param name="elapsed">Time to advance.</param>
    void Tick(TimeSpan elapsed);

    /// <summary>
    /// Resets the time source to zero.
    /// </summary>
    void Reset();
}
