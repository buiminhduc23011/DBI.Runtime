using System.Runtime.InteropServices;

namespace DBI.Runtime.Motion.Drivers.Leadshine;

/// <summary>
/// Native API for Leadshine DMC2410 motion control card.
/// DLL: dmc2410.dll (32-bit) or dmc2410_x64.dll (64-bit)
/// </summary>
/// <remarks>
/// SDK Download: https://www.leadshine.com/product/DMC2410.html
/// Manual: DMC2410 Software Manual v1.1
/// </remarks>
internal static class Dmc2410Api
{
    private const string DLL_NAME = "dmc2410.dll";

    #region Board Initialization

    /// <summary>
    /// Initialize the DMC2410 board.
    /// </summary>
    /// <param name="cardNo">Card number (0-7).</param>
    /// <returns>0 = success, others = error code.</returns>
    [DllImport(DLL_NAME)]
    public static extern int d2410_board_init(int cardNo);

    /// <summary>
    /// Close the DMC2410 board and release resources.
    /// </summary>
    /// <returns>0 = success.</returns>
    [DllImport(DLL_NAME)]
    public static extern int d2410_board_close();

    /// <summary>
    /// Get the number of installed cards.
    /// </summary>
    /// <returns>Number of cards.</returns>
    [DllImport(DLL_NAME)]
    public static extern int d2410_get_card_num();

    /// <summary>
    /// Get board version.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_get_board_version(int cardNo, ref int version);

    #endregion

    #region Axis Configuration

    /// <summary>
    /// Set pulse output mode.
    /// </summary>
    /// <param name="axis">Axis number (0-3).</param>
    /// <param name="outMode">0=Pulse/Dir, 1=CW/CCW.</param>
    /// <param name="posLogic">Pulse logic: 0=positive, 1=negative.</param>
    /// <param name="dirLogic">Direction logic: 0=positive, 1=negative.</param>
    [DllImport(DLL_NAME)]
    public static extern int d2410_set_pulse_outmode(int axis, int outMode, int posLogic, int dirLogic);

    /// <summary>
    /// Set encoder input mode.
    /// </summary>
    /// <param name="axis">Axis number.</param>
    /// <param name="encMode">0=1x, 1=2x, 2=4x multiplication.</param>
    /// <param name="encLogic">Encoder logic.</param>
    [DllImport(DLL_NAME)]
    public static extern int d2410_set_encoder_mode(int axis, int encMode, int encLogic);

    /// <summary>
    /// Set software limits.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_set_softlimit(int axis, int enable, int posLimit, int negLimit);

    /// <summary>
    /// Set servo enable output.
    /// </summary>
    /// <param name="axis">Axis number.</param>
    /// <param name="enable">1=enable, 0=disable.</param>
    [DllImport(DLL_NAME)]
    public static extern int d2410_set_sevon_out(int axis, int enable);

    /// <summary>
    /// Read servo enable status.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_read_sevon_out(int axis);

    /// <summary>
    /// Clear alarm output.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_set_alm_clr(int axis);

    #endregion

    #region Position Control

    /// <summary>
    /// Set current command position.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_set_position(int axis, int position);

    /// <summary>
    /// Get current command position.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_get_position(int axis);

    /// <summary>
    /// Set encoder position.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_set_encoder(int axis, int encoder);

    /// <summary>
    /// Get encoder position.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_get_encoder(int axis);

    #endregion

    #region Motion Profile

    /// <summary>
    /// Set velocity profile for trapezoidal motion.
    /// </summary>
    /// <param name="axis">Axis number.</param>
    /// <param name="startVel">Start velocity (pulse/s).</param>
    /// <param name="runVel">Running velocity (pulse/s).</param>
    /// <param name="accTime">Acceleration time (ms).</param>
    /// <param name="decTime">Deceleration time (ms).</param>
    [DllImport(DLL_NAME)]
    public static extern int d2410_set_profile(int axis, int startVel, int runVel, double accTime, double decTime);

    /// <summary>
    /// Set S-curve velocity profile.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_set_s_profile(int axis, int startVel, int runVel, double accTime, double decTime, double sTime);

    #endregion

    #region Point-to-Point Motion

    /// <summary>
    /// Start point-to-point motion (relative).
    /// </summary>
    /// <param name="axis">Axis number.</param>
    /// <param name="dist">Distance in pulses.</param>
    [DllImport(DLL_NAME)]
    public static extern int d2410_pmove(int axis, int dist);

    /// <summary>
    /// Start point-to-point motion (absolute).
    /// </summary>
    /// <param name="axis">Axis number.</param>
    /// <param name="pos">Target position in pulses.</param>
    [DllImport(DLL_NAME)]
    public static extern int d2410_amove(int axis, int pos);

    #endregion

    #region Continuous Motion

    /// <summary>
    /// Start continuous motion (velocity mode).
    /// </summary>
    /// <param name="axis">Axis number.</param>
    /// <param name="startVel">Start velocity.</param>
    /// <param name="runVel">Running velocity (sign determines direction).</param>
    /// <param name="accTime">Acceleration time (ms).</param>
    [DllImport(DLL_NAME)]
    public static extern int d2410_vmove(int axis, int startVel, int runVel, double accTime);

    /// <summary>
    /// Change velocity during continuous motion.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_change_speed(int axis, int newVel, double accTime);

    #endregion

    #region Motion Control

    /// <summary>
    /// Stop motion with deceleration.
    /// </summary>
    /// <param name="axis">Axis number.</param>
    /// <param name="decTime">Deceleration time (ms).</param>
    [DllImport(DLL_NAME)]
    public static extern int d2410_decel_stop(int axis, double decTime);

    /// <summary>
    /// Emergency stop (immediate).
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_emg_stop(int axis);

    /// <summary>
    /// Check if axis is in motion.
    /// </summary>
    /// <returns>0=stopped, 1=moving.</returns>
    [DllImport(DLL_NAME)]
    public static extern int d2410_check_done(int axis);

    #endregion

    #region Homing

    /// <summary>
    /// Configure home mode.
    /// </summary>
    /// <param name="axis">Axis number.</param>
    /// <param name="mode">Home mode (0-7).</param>
    /// <param name="homeDir">Home direction: 0=negative, 1=positive.</param>
    [DllImport(DLL_NAME)]
    public static extern int d2410_config_home_mode(int axis, int mode, int homeDir);

    /// <summary>
    /// Start homing operation.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_home_start(int axis, int startVel, int runVel, double accTime);

    /// <summary>
    /// Check homing status.
    /// </summary>
    /// <returns>0=homing complete, 1=homing in progress.</returns>
    [DllImport(DLL_NAME)]
    public static extern int d2410_check_home(int axis);

    #endregion

    #region I/O Status

    /// <summary>
    /// Read axis I/O status.
    /// </summary>
    /// <returns>Bit field: 0=+Limit, 1=-Limit, 2=Origin, 3=Alarm, 4=Inpos.</returns>
    [DllImport(DLL_NAME)]
    public static extern int d2410_read_axis_io(int axis);

    /// <summary>
    /// Read general input status.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_read_input(int cardNo, int inputNo);

    /// <summary>
    /// Write general output.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_write_output(int cardNo, int outputNo, int value);

    #endregion

    #region Interpolation

    /// <summary>
    /// Set interpolation profile.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_set_vector_profile(int cardNo, int startVel, int runVel, double accTime, double decTime);

    /// <summary>
    /// Start 2-axis linear interpolation.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_line2(int axis1, int axis2, int dist1, int dist2);

    /// <summary>
    /// Start 3-axis linear interpolation.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_line3(int axis1, int axis2, int axis3, int dist1, int dist2, int dist3);

    /// <summary>
    /// Start 4-axis linear interpolation.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_line4(int axis1, int axis2, int axis3, int axis4, int dist1, int dist2, int dist3, int dist4);

    /// <summary>
    /// Start 2-axis arc interpolation.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int d2410_arc2(int axis1, int axis2, int endX, int endY, int centerX, int centerY, int dir);

    #endregion

    #region Error Codes

    public const int ERR_SUCCESS = 0;
    public const int ERR_CARD_NOT_FOUND = -1;
    public const int ERR_INVALID_AXIS = -2;
    public const int ERR_INVALID_PARAMETER = -3;
    public const int ERR_MOTION_NOT_COMPLETE = -4;
    public const int ERR_LIMIT_ACTIVE = -5;
    public const int ERR_ALARM_ACTIVE = -6;

    #endregion
}
