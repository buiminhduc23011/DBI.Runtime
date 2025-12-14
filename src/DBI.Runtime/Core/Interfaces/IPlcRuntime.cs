using DBI.Runtime.Core.Memory;

namespace DBI.Runtime.Core.Interfaces;

/// <summary>
/// PLC Runtime interface providing scan cycle execution.
/// </summary>
public interface IPlcRuntime : IDisposable
{
    /// <summary>
    /// Gets or sets the target cycle time.
    /// </summary>
    TimeSpan CycleTime { get; set; }

    /// <summary>
    /// Gets the actual duration of the last scan cycle.
    /// </summary>
    TimeSpan LastCycleDuration { get; }

    /// <summary>
    /// Gets the current scan cycle count since start.
    /// </summary>
    long CycleCount { get; }

    /// <summary>
    /// Gets the PLC memory (I/Q/M areas).
    /// </summary>
    PlcMemory Memory { get; }

    /// <summary>
    /// Gets the time source used by the runtime.
    /// </summary>
    ITimeSource TimeSource { get; }

    /// <summary>
    /// Gets whether the runtime is currently running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Registers a function block to be executed each scan cycle.
    /// </summary>
    void RegisterFunctionBlock(IFunctionBlock block);

    /// <summary>
    /// Unregisters a function block.
    /// </summary>
    void UnregisterFunctionBlock(IFunctionBlock block);

    /// <summary>
    /// Starts the PLC runtime.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the PLC runtime.
    /// </summary>
    void Stop();

    /// <summary>
    /// Executes a single scan cycle manually.
    /// </summary>
    void ExecuteSingleCycle();

    /// <summary>
    /// Event raised when a scan cycle completes.
    /// </summary>
    event EventHandler<ScanCycleEventArgs>? OnScanCycleComplete;
}

/// <summary>
/// Event arguments for scan cycle completion.
/// </summary>
public class ScanCycleEventArgs : EventArgs
{
    /// <summary>
    /// Gets the cycle number.
    /// </summary>
    public long CycleNumber { get; init; }

    /// <summary>
    /// Gets the actual duration of the cycle.
    /// </summary>
    public TimeSpan CycleDuration { get; init; }

    /// <summary>
    /// Gets the current runtime time.
    /// </summary>
    public TimeSpan RuntimeTime { get; init; }
}
