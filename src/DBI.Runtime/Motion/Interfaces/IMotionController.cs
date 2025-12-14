namespace DBI.Runtime.Motion.Interfaces;

/// <summary>
/// Interface for motion control card/controller.
/// </summary>
public interface IMotionController : IDisposable
{
    /// <summary>
    /// Gets the controller name/type.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the card/board number.
    /// </summary>
    int CardNumber { get; }

    /// <summary>
    /// Gets whether the controller is connected/initialized.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets the number of available axes.
    /// </summary>
    int AxisCount { get; }

    /// <summary>
    /// Gets the last error code.
    /// </summary>
    int LastErrorCode { get; }

    /// <summary>
    /// Gets the last error message.
    /// </summary>
    string LastErrorMessage { get; }

    /// <summary>
    /// Initializes the motion controller.
    /// </summary>
    /// <param name="config">Configuration parameters.</param>
    /// <returns>True if successful.</returns>
    bool Initialize(MotionControllerConfig config);

    /// <summary>
    /// Closes the motion controller and releases resources.
    /// </summary>
    void Close();

    /// <summary>
    /// Gets a specific axis by index.
    /// </summary>
    /// <param name="axisIndex">Axis index (0-based).</param>
    /// <returns>The servo axis interface.</returns>
    IServoAxis GetAxis(int axisIndex);

    /// <summary>
    /// Gets all available axes.
    /// </summary>
    IServoAxis[] GetAllAxes();

    /// <summary>
    /// Emergency stops all axes immediately.
    /// </summary>
    void EmergencyStopAll();

    /// <summary>
    /// Resets all axis errors.
    /// </summary>
    void ResetAllErrors();

    /// <summary>
    /// Enables all axes.
    /// </summary>
    void EnableAll();

    /// <summary>
    /// Disables all axes.
    /// </summary>
    void DisableAll();

    /// <summary>
    /// Updates status for all axes (call each scan cycle).
    /// </summary>
    void UpdateAllStatus();

    /// <summary>
    /// Starts linear interpolation motion on multiple axes.
    /// </summary>
    /// <param name="axes">Array of axis indices.</param>
    /// <param name="positions">Array of target positions.</param>
    /// <param name="profile">Motion profile.</param>
    void StartLinearInterpolation(int[] axes, double[] positions, MotionProfile profile);

    /// <summary>
    /// Waits for all specified axes to complete motion.
    /// </summary>
    /// <param name="axes">Array of axis indices.</param>
    /// <param name="timeoutMs">Timeout in milliseconds.</param>
    /// <returns>True if all axes completed, false if timeout.</returns>
    bool WaitForMotionComplete(int[] axes, int timeoutMs);
}
