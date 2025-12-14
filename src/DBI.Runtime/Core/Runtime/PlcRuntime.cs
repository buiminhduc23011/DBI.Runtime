using System.Diagnostics;
using DBI.Runtime.Core.Interfaces;
using DBI.Runtime.Core.Memory;

namespace DBI.Runtime.Core.Runtime;

/// <summary>
/// PLC Runtime implementation providing scan cycle execution.
/// </summary>
public class PlcRuntime : IPlcRuntime
{
    private readonly List<IFunctionBlock> _functionBlocks = [];
    private readonly object _lock = new();
    private readonly Stopwatch _cycleStopwatch = new();
    private CancellationTokenSource? _cts;
    private Task? _runTask;
    private bool _disposed;

    /// <summary>
    /// Creates a new PLC runtime with default settings.
    /// </summary>
    public PlcRuntime() : this(new MonotonicTimeSource())
    {
    }

    /// <summary>
    /// Creates a new PLC runtime with the specified time source.
    /// </summary>
    public PlcRuntime(ITimeSource timeSource)
    {
        TimeSource = timeSource ?? throw new ArgumentNullException(nameof(timeSource));
        Memory = new PlcMemory();
        CycleTime = TimeSpan.FromMilliseconds(100); // Default 100ms cycle
    }

    /// <inheritdoc/>
    public TimeSpan CycleTime { get; set; }

    /// <inheritdoc/>
    public TimeSpan LastCycleDuration { get; private set; }

    /// <inheritdoc/>
    public long CycleCount { get; private set; }

    /// <inheritdoc/>
    public PlcMemory Memory { get; }

    /// <inheritdoc/>
    public ITimeSource TimeSource { get; }

    /// <inheritdoc/>
    public bool IsRunning => _runTask != null && !_runTask.IsCompleted;

    /// <inheritdoc/>
    public event EventHandler<ScanCycleEventArgs>? OnScanCycleComplete;

    /// <inheritdoc/>
    public void RegisterFunctionBlock(IFunctionBlock block)
    {
        ArgumentNullException.ThrowIfNull(block);
        lock (_lock)
        {
            if (!_functionBlocks.Contains(block))
                _functionBlocks.Add(block);
        }
    }

    /// <inheritdoc/>
    public void UnregisterFunctionBlock(IFunctionBlock block)
    {
        lock (_lock)
        {
            _functionBlocks.Remove(block);
        }
    }

    /// <inheritdoc/>
    public void Start()
    {
        if (IsRunning)
            return;

        _cts = new CancellationTokenSource();
        _runTask = Task.Run(() => RunLoop(_cts.Token), _cts.Token);
    }

    /// <inheritdoc/>
    public void Stop()
    {
        _cts?.Cancel();
        try
        {
            _runTask?.Wait(TimeSpan.FromSeconds(5));
        }
        catch (AggregateException)
        {
            // Expected when canceling
        }
        _cts?.Dispose();
        _cts = null;
        _runTask = null;
    }

    /// <inheritdoc/>
    public void ExecuteSingleCycle()
    {
        _cycleStopwatch.Restart();

        // 1. Update time source (for simulated time)
        if (TimeSource is MonotonicTimeSource mts)
        {
            mts.Tick(CycleTime);
        }

        // 2. Execute all registered function blocks
        IFunctionBlock[] blocks;
        lock (_lock)
        {
            blocks = [.. _functionBlocks];
        }

        foreach (var block in blocks)
        {
            try
            {
                block.Execute();
            }
            catch (Exception ex)
            {
                // Log error but continue execution (PLC-like behavior)
                System.Diagnostics.Debug.WriteLine($"FB execution error: {ex.Message}");
            }
        }

        _cycleStopwatch.Stop();
        LastCycleDuration = _cycleStopwatch.Elapsed;
        CycleCount++;

        // Raise event
        OnScanCycleComplete?.Invoke(this, new ScanCycleEventArgs
        {
            CycleNumber = CycleCount,
            CycleDuration = LastCycleDuration,
            RuntimeTime = TimeSource.Now
        });
    }

    private async Task RunLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var cycleStart = Stopwatch.GetTimestamp();

            ExecuteSingleCycle();

            // Wait for remainder of cycle time
            var elapsed = Stopwatch.GetElapsedTime(cycleStart);
            var remaining = CycleTime - elapsed;

            if (remaining > TimeSpan.Zero)
            {
                try
                {
                    await Task.Delay(remaining, ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        Stop();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
