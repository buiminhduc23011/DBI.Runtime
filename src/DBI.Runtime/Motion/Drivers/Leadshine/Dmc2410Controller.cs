using DBI.Runtime.Motion.Interfaces;

namespace DBI.Runtime.Motion.Drivers.Leadshine;

/// <summary>
/// Leadshine DMC2410 4-axis PCIe motion control card driver.
/// </summary>
/// <remarks>
/// Product: DMC2410 - 4-axis Stepper/Servo Motion Control Card
/// Interface: PCIe
/// Features: Pulse/Direction, CW/CCW, Encoder feedback, Interpolation
/// SDK Download: https://www.leadshine.com/download
/// </remarks>
public class Dmc2410Controller : IMotionController
{
    private readonly Dmc2410Axis[] _axes;
    private bool _initialized;
    private int _lastError;

    /// <summary>
    /// Creates a new DMC2410 controller instance.
    /// </summary>
    public Dmc2410Controller()
    {
        _axes = new Dmc2410Axis[4]; // DMC2410 supports 4 axes
    }

    #region IMotionController Properties

    /// <inheritdoc/>
    public string Name => "Leadshine DMC2410";

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

        // Check if card exists
        int cardCount = Dmc2410Api.d2410_get_card_num();
        if (cardCount <= CardNumber)
        {
            _lastError = Dmc2410Api.ERR_CARD_NOT_FOUND;
            return false;
        }

        // Initialize board
        int result = Dmc2410Api.d2410_board_init(CardNumber);
        if (result != 0)
        {
            _lastError = result;
            return false;
        }

        // Create axis objects
        int axisCount = Math.Min(config.AxisCount, 4);
        for (int i = 0; i < axisCount; i++)
        {
            _axes[i] = new Dmc2410Axis(CardNumber, i);
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

            // Close board
            Dmc2410Api.d2410_board_close();
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
                Dmc2410Api.d2410_emg_stop(i);
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

        // Set vector profile
        var firstAxis = _axes[axes[0]];
        int startVel = (int)(profile.Velocity * 1000 * 0.01);
        int runVel = (int)(profile.Velocity * 1000);
        double accTime = profile.Velocity / profile.Acceleration * 1000;
        double decTime = profile.Velocity / (profile.Deceleration > 0 ? profile.Deceleration : profile.Acceleration) * 1000;

        Dmc2410Api.d2410_set_vector_profile(CardNumber, startVel, runVel, accTime, decTime);

        // Calculate distances (relative motion for interpolation)
        int[] distances = new int[4];
        for (int i = 0; i < axes.Length && i < 4; i++)
        {
            double currentPos = _axes[axes[i]]?.CommandPosition ?? 0;
            distances[i] = (int)((positions[i] - currentPos) * 1000);
        }

        // Start interpolation based on number of axes
        switch (axes.Length)
        {
            case 2:
                Dmc2410Api.d2410_line2(axes[0], axes[1], distances[0], distances[1]);
                break;
            case 3:
                Dmc2410Api.d2410_line3(axes[0], axes[1], axes[2], distances[0], distances[1], distances[2]);
                break;
            case 4:
                Dmc2410Api.d2410_line4(axes[0], axes[1], axes[2], axes[3], distances[0], distances[1], distances[2], distances[3]);
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
                if (Dmc2410Api.d2410_check_done(axisIndex) != 0)
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
        Dmc2410Api.ERR_CARD_NOT_FOUND => "Motion control card not found",
        Dmc2410Api.ERR_INVALID_AXIS => "Invalid axis number",
        Dmc2410Api.ERR_INVALID_PARAMETER => "Invalid parameter",
        _ => $"Error code: {errorCode}"
    };
}
