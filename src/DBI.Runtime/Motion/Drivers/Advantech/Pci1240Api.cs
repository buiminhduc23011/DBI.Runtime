using System.Runtime.InteropServices;

namespace DBI.Runtime.Motion.Drivers.Advantech;

/// <summary>
/// Native API for Advantech PCI-1240U motion control card.
/// DLL: mPC1240.dll (32-bit) or mPC1240_x64.dll (64-bit)
/// </summary>
/// <remarks>
/// SDK Download: https://support.advantech.com
/// Manual: PCI-1240U User Manual / Common Motion API Manual
/// Install Path: C:\Program Files\Advantech\Motion\PCI-1240\
/// </remarks>
internal static class Pci1240Api
{
    private const string DLL_NAME = "mPC1240.dll";

    #region Device Management

    /// <summary>
    /// Open and initialize the PCI-1240 device.
    /// </summary>
    /// <param name="cardNo">Card number (0-based).</param>
    /// <returns>0 = success, others = error code.</returns>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotDevOpen(int cardNo);

    /// <summary>
    /// Close the device and release resources.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotDevClose(int cardNo);

    /// <summary>
    /// Reset the device.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotDevReset(int cardNo);

    /// <summary>
    /// Get device information.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotDevGetInfo(int cardNo, ref int version, ref int axisCount);

    #endregion

    #region Axis Configuration

    /// <summary>
    /// Set pulse output mode.
    /// </summary>
    /// <param name="cardNo">Card number.</param>
    /// <param name="axis">Axis number (0-3).</param>
    /// <param name="mode">0=Out/Dir, 1=CW/CCW.</param>
    /// <param name="logic">Output logic polarity.</param>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotSetPulseOut(int cardNo, int axis, int mode, int logic);

    /// <summary>
    /// Set encoder mode.
    /// </summary>
    /// <param name="cardNo">Card number.</param>
    /// <param name="axis">Axis number.</param>
    /// <param name="mode">0=1x, 1=2x, 2=4x.</param>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotSetEncMode(int cardNo, int axis, int mode);

    /// <summary>
    /// Enable/disable servo.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotSetServoOn(int cardNo, int axis, int enable);

    /// <summary>
    /// Get servo enable status.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotGetServoOn(int cardNo, int axis, ref int enable);

    /// <summary>
    /// Set software limit.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotSetSoftLimit(int cardNo, int axis, int enable, int posLimit, int negLimit);

    /// <summary>
    /// Clear alarm.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotClrAlarm(int cardNo, int axis);

    #endregion

    #region Position

    /// <summary>
    /// Set command position counter.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotSetCmdPos(int cardNo, int axis, int pos);

    /// <summary>
    /// Get command position.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotGetCmdPos(int cardNo, int axis, ref int pos);

    /// <summary>
    /// Set encoder position counter.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotSetActPos(int cardNo, int axis, int pos);

    /// <summary>
    /// Get encoder position.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotGetActPos(int cardNo, int axis, ref int pos);

    #endregion

    #region Velocity Profile

    /// <summary>
    /// Set velocity profile (trapezoidal).
    /// </summary>
    /// <param name="cardNo">Card number.</param>
    /// <param name="axis">Axis number.</param>
    /// <param name="strVel">Start velocity (Hz).</param>
    /// <param name="maxVel">Maximum velocity (Hz).</param>
    /// <param name="accTime">Acceleration time (sec).</param>
    /// <param name="decTime">Deceleration time (sec).</param>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotSetSpeed(int cardNo, int axis, double strVel, double maxVel, double accTime, double decTime);

    /// <summary>
    /// Set S-curve profile.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotSetSCurve(int cardNo, int axis, double strVel, double maxVel, double accTime, double decTime, double sRatio);

    #endregion

    #region Point-to-Point Motion

    /// <summary>
    /// Start relative move.
    /// </summary>
    /// <param name="cardNo">Card number.</param>
    /// <param name="axis">Axis number.</param>
    /// <param name="dist">Distance in pulses.</param>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotRelativeMove(int cardNo, int axis, int dist);

    /// <summary>
    /// Start absolute move.
    /// </summary>
    /// <param name="cardNo">Card number.</param>
    /// <param name="axis">Axis number.</param>
    /// <param name="pos">Target position in pulses.</param>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotAbsoluteMove(int cardNo, int axis, int pos);

    #endregion

    #region Continuous Motion

    /// <summary>
    /// Start continuous (JOG) motion.
    /// </summary>
    /// <param name="cardNo">Card number.</param>
    /// <param name="axis">Axis number.</param>
    /// <param name="dir">Direction: 0=negative, 1=positive.</param>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotContMove(int cardNo, int axis, int dir);

    /// <summary>
    /// Change speed during motion.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotChangeSpeed(int cardNo, int axis, double newVel);

    #endregion

    #region Motion Control

    /// <summary>
    /// Stop with deceleration.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotDecelStop(int cardNo, int axis);

    /// <summary>
    /// Emergency stop (immediate).
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotEmgStop(int cardNo, int axis);

    /// <summary>
    /// Check motion status.
    /// </summary>
    /// <returns>Motion status bits.</returns>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotGetStatus(int cardNo, int axis, ref int status);

    /// <summary>
    /// Check if motion is done.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotIsDone(int cardNo, int axis, ref int done);

    #endregion

    #region Homing

    /// <summary>
    /// Configure homing parameters.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotSetHomeConfig(int cardNo, int axis, int mode, int dir, double speed1, double speed2, double acc);

    /// <summary>
    /// Start homing.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotStartHome(int cardNo, int axis);

    /// <summary>
    /// Get homing status.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotGetHomeStatus(int cardNo, int axis, ref int status);

    #endregion

    #region I/O

    /// <summary>
    /// Get limit switch status.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotGetLimitStatus(int cardNo, int axis, ref int posLimit, ref int negLimit);

    /// <summary>
    /// Get home switch status.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotGetHomeSwitch(int cardNo, int axis, ref int status);

    /// <summary>
    /// Get alarm status.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotGetAlarm(int cardNo, int axis, ref int alarm);

    /// <summary>
    /// Get in-position status.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotGetInPos(int cardNo, int axis, ref int inpos);

    /// <summary>
    /// Read digital input.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240DioReadInput(int cardNo, int portNo, ref int value);

    /// <summary>
    /// Write digital output.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240DioWriteOutput(int cardNo, int portNo, int value);

    #endregion

    #region Interpolation

    /// <summary>
    /// Set interpolation speed.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotSetIntplSpeed(int cardNo, double strVel, double maxVel, double acc, double dec);

    /// <summary>
    /// 2-axis linear interpolation.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotLineMove2(int cardNo, int axis1, int axis2, int dist1, int dist2);

    /// <summary>
    /// 3-axis linear interpolation.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotLineMove3(int cardNo, int axis1, int axis2, int axis3, int dist1, int dist2, int dist3);

    /// <summary>
    /// 4-axis linear interpolation.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotLineMove4(int cardNo, int axis1, int axis2, int axis3, int axis4, int dist1, int dist2, int dist3, int dist4);

    /// <summary>
    /// 2-axis arc interpolation.
    /// </summary>
    [DllImport(DLL_NAME)]
    public static extern int P1240MotArcMove2(int cardNo, int axis1, int axis2, int endX, int endY, int centerX, int centerY, int dir);

    #endregion

    #region Status Bit Definitions

    /// <summary>Motion status: busy.</summary>
    public const int STS_BUSY = 0x01;
    /// <summary>Motion status: in position.</summary>
    public const int STS_INPOS = 0x02;
    /// <summary>Motion status: positive limit.</summary>
    public const int STS_POSLIMIT = 0x04;
    /// <summary>Motion status: negative limit.</summary>
    public const int STS_NEGLIMIT = 0x08;
    /// <summary>Motion status: home switch.</summary>
    public const int STS_HOME = 0x10;
    /// <summary>Motion status: alarm.</summary>
    public const int STS_ALARM = 0x20;
    /// <summary>Motion status: servo on.</summary>
    public const int STS_SERVOON = 0x40;

    #endregion

    #region Error Codes

    public const int ERR_SUCCESS = 0;
    public const int ERR_INVALID_CARD = -1;
    public const int ERR_INVALID_AXIS = -2;
    public const int ERR_INVALID_PARAM = -3;
    public const int ERR_NOT_READY = -4;
    public const int ERR_MOTION_BUSY = -5;

    #endregion
}
