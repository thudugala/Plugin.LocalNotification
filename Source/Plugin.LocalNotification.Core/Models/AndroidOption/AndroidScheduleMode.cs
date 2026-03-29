namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Controls which <c>AlarmManager</c> scheduling API is used when delivering a scheduled notification on Android.
/// Choosing the right mode balances battery impact, delivery reliability, and permission requirements.
/// </summary>
public enum AndroidScheduleMode
{
    /// <summary>
    /// <para>Default behavior introduced in earlier versions of this plugin.</para>
    /// Uses <c>AlarmManager.setExactAndAllowWhileIdle()</c> when the app holds the
    /// <c>SCHEDULE_EXACT_ALARM</c> permission (API 31+), or when running below API 31.
    /// Falls back to <c>AlarmManager.setAndAllowWhileIdle()</c> (API 23+) or
    /// <c>AlarmManager.set()</c> otherwise.
    /// </summary>
    Default,

    /// <summary>
    /// <para>Always uses <c>AlarmManager.set()</c> (below API 23) or
    /// <c>AlarmManager.setAndAllowWhileIdle()</c> (API 23+).</para>
    /// The system may batch delivery with other alarms, delaying it by several minutes.
    /// Best for non-time-critical reminders where battery efficiency matters most.
    /// </summary>
    Inexact,

    /// <summary>
    /// <para>Always uses <c>AlarmManager.setAndAllowWhileIdle()</c> (API 23+) or
    /// <c>AlarmManager.set()</c> on older devices.</para>
    /// Inexact but will fire even when the device is in Doze mode.
    /// </summary>
    InexactAllowWhileIdle,

    /// <summary>
    /// <para>Uses <c>AlarmManager.setExact()</c> (API 19+).</para>
    /// Delivers at the requested time precisely.
    /// On API 31+ requires the <c>SCHEDULE_EXACT_ALARM</c> permission or the
    /// <c>USE_EXACT_ALARM</c> permission; if not granted the notification will not fire.
    /// </summary>
    Exact,

    /// <summary>
    /// <para>Uses <c>AlarmManager.setExactAndAllowWhileIdle()</c> (API 23+) or
    /// <c>AlarmManager.setExact()</c> on older devices.</para>
    /// Delivers exactly at the requested time even when the device is in Doze mode.
    /// On API 31+ requires <c>SCHEDULE_EXACT_ALARM</c> or <c>USE_EXACT_ALARM</c>.
    /// </summary>
    ExactAllowWhileIdle,

    /// <summary>
    /// <para>Uses <c>AlarmManager.setAlarmClock()</c> (API 21+).</para>
    /// The notification is shown in the system alarm clock UI (clock icon in status bar).
    /// This is the most reliable scheduling path and does NOT require
    /// <c>SCHEDULE_EXACT_ALARM</c> even on API 31+.
    /// Use for alarms the user has explicitly set.
    /// </summary>
    AlarmClock,
}
