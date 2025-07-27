namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Specifies the type of alarm used for scheduling Android notifications.
/// </summary>
public enum AndroidAlarmType
{
    /// <summary>
    /// Alarm time in System.currentTimeMillis() (wall clock time in UTC).
    /// This alarm does not wake the device up; if it goes off while the device is asleep, it will not be delivered until the next time the device wakes up.
    /// </summary>
    Rtc,

    /// <summary>
    /// Alarm time in System.currentTimeMillis() (wall clock time in UTC), which will wake up the device when it goes off.
    /// </summary>
    RtcWakeup,

    /// <summary>
    /// Alarm time in SystemClock.elapsedRealtime() (time since boot, including sleep).
    /// This alarm does not wake the device up; if it goes off while the device is asleep, it will not be delivered until the next time the device wakes up.
    /// </summary>
    ElapsedRealtime,

    /// <summary>
    /// Alarm time in SystemClock.elapsedRealtime() (time since boot, including sleep), which will wake up the device when it goes off.
    /// </summary>
    ElapsedRealtimeWakeup
}