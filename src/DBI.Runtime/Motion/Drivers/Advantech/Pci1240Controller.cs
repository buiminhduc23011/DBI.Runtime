using DBI.Runtime.Motion.Interfaces;

namespace DBI.Runtime.Motion.Drivers.Advantech;

/// <summary>
/// Advantech PCI-1240U 4-axis PCIe motion control card driver.
/// </summary>
/// <remarks>
/// Product: PCI-1240U - 4-axis Stepper/Servo Motion Control Card
/// Interface: PCI / PCI Express
/// Features: Pulse/Direction, CW/CCW, Encoder feedback, Interpolation
/// SDK Download: https://support.advantech.com
/// Install Path: C:\Program Files\Advantech\Motion\PCI-1240\
/// </remarks>
public class Pci1240Controller : IMotionController
{
    private readonly Pci1240Axis[] _axes;
    private bool _initialized;
    private int _lastError;

    /// <summary>
    /// Creates a new PCI-1240 controller instance.
    /// </summary>
    public Pci1240Controller()
    {
        _axes = new Pci1240Axis[4]; // PCI-1240 supports 4 axes
    }

    #region IMotionController Properties

    /// <inheritdoc/>
    public string Name => "Advantech PCI-1240U";

    /// <inheritdoc/>
    public int CardNumber { get; private set; }

    /// <inheritdoc/>
    public bool IsConnected => _initialized;

    /// <inheritdoc/>
    public int AxisCount => 4;

    /// <inheritdoc/>
    public int LastErrorCode => _lastError;

    /// <inheritdoc/>
    public string LastErrorMessage => GetErrorMessage(_lastError);

    #endregion

    #region Initialization

    /// <inheritdoc/>
    public bool Initialize(MotionControllerConfig config)
    {
        if (_initialized)
        {
            Close();
        }

        CardNumber = config.CardNumber;

        // Open device
        int result = Pci1240Api.P1240MotDevOpen(CardNumber);
        if (result != 0)
        {
            _lastError = result;
            return false;
        }

        // Get device info
        int version = 0, axisCount = 0;
        Pci1240Api.P1240MotDevGetInfo(CardNumber, ref version, ref axisCount);

        // Create axis objects
        int axes = Math.Min(config.AxisCount, axisCount > 0 ? axisCount : 4);
        for (int i = 0; i < axes; i++)
        {
            _axes[i] = new Pci1240Axis(CardNumber, i);
        }

        _initialized = true;
        _lastError = 0;
        return true;
    }

    /// <inheritdoc/>
    public void Close()
    {
        if (_initialized)
        {
            // Disable all axes first
            DisableAll();

            // Close device
            Pci1240Api.P1240MotDevClose(CardNumber);
            _initialized = false;
        }
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
        for (int i = 0; i < AxisCount; i++)
        {
            if (_axes[i] != null)
            {
                Pci1240Api.P1240MotEmgStop(CardNumber, i);
            }
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
        if (axes.Length != positions.Length)
            throw new ArgumentException("Axes and positions arrays must have equal length");

        // Set interpolation speed
        double strVel = profile.Velocity * 1000 * 0.01;
        double maxVel = profile.Velocity * 1000;
        double acc = profile.Acceleration * 1000;
        double dec = profile.Deceleration > 0 ? profile.Deceleration * 1000 : acc;

        Pci1240Api.P1240MotSetIntplSpeed(CardNumber, strVel, maxVel, acc, dec);

        // Calculate distances
        int[] distances = new int[4];
        for (int i = 0; i < axes.Length && i < 4; i++)
        {
            double currentPos = _axes[axes[i]]?.CommandPosition ?? 0;
            distances[i] = (int)((positions[i] - currentPos) * 1000);
        }

        // Start interpolation
        switch (axes.Length)
        {
            case 2:
                Pci1240Api.P1240MotLineMove2(CardNumber, axes[0], axes[1], distances[0], distances[1]);
                break;
            case 3:
                Pci1240Api.P1240MotLineMove3(CardNumber, axes[0], axes[1], axes[2], distances[0], distances[1], distances[2]);
                break;
            case 4:
                Pci1240Api.P1240MotLineMove4(CardNumber, axes[0], axes[1], axes[2], axes[3], distances[0], distances[1], distances[2], distances[3]);
                break;
            default:
                throw new ArgumentException("Linear interpolation requires 2-4 axes");
        }
    }

    /// <inheritdoc/>
    public bool WaitForMotionComplete(int[] axes, int timeoutMs)
    {
        var startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
        {
            bool allDone = true;
            foreach (int axisIndex in axes)
            {
                int done = 0;
                Pci1240Api.P1240MotIsDone(CardNumber, axisIndex, ref done);
                if (done == 0)
                {
                    allDone = false;
                    break;
                }
            }

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

    private static string GetErrorMessage(int errorCode) => errorCode switch
    {
        0 => "No error",
        Pci1240Api.ERR_INVALID_CARD => "Invalid card number",
        Pci1240Api.ERR_INVALID_AXIS => "Invalid axis number",
        Pci1240Api.ERR_INVALID_PARAM => "Invalid parameter",
        Pci1240Api.ERR_NOT_READY => "Device not ready",
        Pci1240Api.ERR_MOTION_BUSY => "Motion busy",
        _ => $"Error code: {errorCode}"
    };
}
