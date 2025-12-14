using DBI.Runtime.Motion.Interfaces;

namespace DBI.Runtime.Motion.Drivers.Simulation;

/// <summary>
/// Simulated motion controller for testing without hardware.
/// </summary>
public class SimulatedMotionController : IMotionController
{
    private readonly SimulatedServoAxis[] _axes;
    private bool _initialized;

    /// <summary>
    /// Creates a new simulated controller.
    /// </summary>
    /// <param name="axisCount">Number of simulated axes (default 4).</param>
    public SimulatedMotionController(int axisCount = 4)
    {
        _axes = new SimulatedServoAxis[axisCount];
    }

    #region IMotionController Properties

    /// <inheritdoc/>
    public string Name => "Simulated Motion Controller";

    /// <inheritdoc/>
    public int CardNumber => 0;

    /// <inheritdoc/>
    public bool IsConnected => _initialized;

    /// <inheritdoc/>
    public int AxisCount => _axes.Length;

    /// <inheritdoc/>
    public int LastErrorCode => 0;

    /// <inheritdoc/>
    public string LastErrorMessage => "No error";

    #endregion

    #region Initialization

    /// <inheritdoc/>
    public bool Initialize(MotionControllerConfig config)
    {
        int count = Math.Min(config.AxisCount, _axes.Length);
        for (int i = 0; i < count; i++)
        {
            _axes[i] = new SimulatedServoAxis(i);
        }

        _initialized = true;
        return true;
    }

    /// <inheritdoc/>
    public void Close()
    {
        DisableAll();
        _initialized = false;
    }

    #endregion

    #region Axis Access

    /// <inheritdoc/>
    public IServoAxis GetAxis(int axisIndex)
    {
        if (axisIndex < 0 || axisIndex >= AxisCount)
            throw new ArgumentOutOfRangeException(nameof(axisIndex));

        return _axes[axisIndex] ?? throw new InvalidOperationException("Axis not initialized");
    }

    /// <inheritdoc/>
    public IServoAxis[] GetAllAxes()
    {
        return _axes.Where(a => a != null).ToArray<IServoAxis>();
    }

    #endregion

    #region Multi-Axis Control

    /// <inheritdoc/>
    public void EmergencyStopAll()
    {
        foreach (var axis in _axes)
        {
            axis?.EmergencyStop();
        }
    }

    /// <inheritdoc/>
    public void ResetAllErrors()
    {
        foreach (var axis in _axes)
        {
            axis?.ResetError();
        }
    }

    /// <inheritdoc/>
    public void EnableAll()
    {
        foreach (var axis in _axes)
        {
            axis?.Enable();
        }
    }

    /// <inheritdoc/>
    public void DisableAll()
    {
        foreach (var axis in _axes)
        {
            axis?.Disable();
        }
    }

    /// <inheritdoc/>
    public void UpdateAllStatus()
    {
        foreach (var axis in _axes)
        {
            axis?.UpdateStatus();
        }
    }

    #endregion

    #region Interpolation

    /// <inheritdoc/>
    public void StartLinearInterpolation(int[] axes, double[] positions, MotionProfile profile)
    {
        // Simple simulation: just move each axis independently
        for (int i = 0; i < axes.Length; i++)
        {
            _axes[axes[i]]?.MoveAbsolute(positions[i], profile);
        }
    }

    /// <inheritdoc/>
    public bool WaitForMotionComplete(int[] axes, int timeoutMs)
    {
        var startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
        {
            UpdateAllStatus();

            bool allDone = axes.All(i => _axes[i]?.IsInPosition ?? true);
            if (allDone)
                return true;

            Thread.Sleep(1);
        }

        return false;
    }

    #endregion

    /// <inheritdoc/>
    public void Dispose()
    {
        Close();
        GC.SuppressFinalize(this);
    }
}
