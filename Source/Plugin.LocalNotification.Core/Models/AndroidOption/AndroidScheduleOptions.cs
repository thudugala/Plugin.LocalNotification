namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Represents scheduling options for Android notifications, including alarm type and allowed delay.
/// </summary>
public class AndroidScheduleOptions
{
    /// <summary>
    /// Gets or sets the alarm type for scheduling notifications. Default is <see cref="AndroidAlarmType.RtcWakeup"/>.
    /// </summary>
    public AndroidAlarmType AlarmType { get; set; } = AndroidAlarmType.RtcWakeup;

    /// <summary>
    /// Gets or sets the scheduling mode that controls which <c>AlarmManager</c> API is used.
    /// <list type="bullet">
    ///   <item><description><see cref="AndroidScheduleMode.Default"/> — exact when the app has the required permission, inexact otherwise.</description></item>
    ///   <item><description><see cref="AndroidScheduleMode.Inexact"/> — always uses <c>AlarmManager.set()</c>; most battery-friendly.</description></item>
    ///   <item><description><see cref="AndroidScheduleMode.InexactAllowWhileIdle"/> — <c>AlarmManager.setAndAllowWhileIdle()</c>; fires in Doze mode.</description></item>
    ///   <item><description><see cref="AndroidScheduleMode.Exact"/> — <c>AlarmManager.setExact()</c>; requires <c>SCHEDULE_EXACT_ALARM</c> on API 31+.</description></item>
    ///   <item><description><see cref="AndroidScheduleMode.ExactAllowWhileIdle"/> — <c>AlarmManager.setExactAndAllowWhileIdle()</c>; fires in Doze mode.</description></item>
    ///   <item><description><see cref="AndroidScheduleMode.AlarmClock"/> — <c>AlarmManager.setAlarmClock()</c>; user-visible alarm icon, most reliable for user-facing alarms.</description></item>
    /// </list>
    /// Default is <see cref="AndroidScheduleMode.Default"/>.
    /// </summary>
    public AndroidScheduleMode ScheduleMode { get; set; } = AndroidScheduleMode.Default;

    /// <summary>
    /// Gets or sets the allowed delay for scheduling notifications. If <c>NotifyTime</c> is earlier than <c>DateTimeOffset.Now</c> and this delay, notification will not be scheduled or shown. Default is 1 minute.
    /// </summary>
    public TimeSpan AllowedDelay { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Calculates the next notification time for a repeating notification request.
    /// </summary>
    /// <param name="notifyTime">The original notification time.</param>
    /// <param name="repeatType">The type of repeat interval.</param>
    /// <param name="notifyRepeatInterval">The custom repeat interval, if applicable.</param>
    /// <returns>The next notification time, or <c>null</c> if not applicable.</returns>
    public DateTimeOffset? GetNextNotifyTimeForRepeat(DateTimeOffset? notifyTime, NotificationRepeat repeatType, TimeSpan? notifyRepeatInterval)
    {
        // NotifyTime does not change for Repeat request
        if (notifyTime is null)
        {
            return null;
        }

        // Monthly uses calendar arithmetic (months vary in length)
        if (repeatType == NotificationRepeat.Monthly)
        {
            var monthlyNextTime = notifyTime.Value.AddMonths(1);
            var monthlyNow = DateTimeOffset.Now.AddSeconds(10);
            while (monthlyNextTime <= monthlyNow)
            {
                monthlyNextTime = monthlyNextTime.AddMonths(1);
            }
            return monthlyNextTime;
        }

        var repeatInterval = GetNotifyRepeatInterval(repeatType, notifyRepeatInterval);
        if (repeatInterval == TimeSpan.Zero)
        {
            return null;
        }

        var newNotifyTime = notifyTime.Value.Add(repeatInterval);
        var nowTime = DateTimeOffset.Now.AddSeconds(10);
        while (newNotifyTime <= nowTime)
        {
            newNotifyTime = newNotifyTime.Add(repeatInterval);
        }
        return newNotifyTime;
    }

    /// <summary>
    /// Gets the repeat interval for a notification based on the repeat type and custom interval.
    /// </summary>
    /// <param name="repeatType">The type of repeat interval.</param>
    /// <param name="notifyRepeatInterval">The custom repeat interval, if applicable.</param>
    /// <returns>The repeat interval as a <see cref="TimeSpan"/>.</returns>
    public TimeSpan GetNotifyRepeatInterval(NotificationRepeat repeatType, TimeSpan? notifyRepeatInterval)
    {
        var repeatInterval = TimeSpan.Zero;
        switch (repeatType)
        {
            case NotificationRepeat.Daily:
                // To be consistent with IOS, Schedule notification next day same time.
                repeatInterval = TimeSpan.FromDays(1);
                break;

            case NotificationRepeat.Weekly:
                // To be consistent with IOS, Schedule notification next week same day same time.
                repeatInterval = TimeSpan.FromDays(7);
                break;

            case NotificationRepeat.TimeInterval:
                if (notifyRepeatInterval.HasValue)
                {
                    repeatInterval = notifyRepeatInterval.Value;
                }
                break;

            case NotificationRepeat.Monthly:
                // Monthly is handled separately in GetNextNotifyTimeForRepeat via calendar arithmetic.
                // Returning Zero here means TimeInterval-style callers see no interval (safe fallback).
                break;

            case NotificationRepeat.No:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        return repeatInterval;
    }

    /// <summary>
    /// Determines if the notification time is valid for showing, consistent with IOS behavior.
    /// </summary>
    /// <param name="timeNow">The current time.</param>
    /// <param name="notifyTime">The notification time to validate.</param>
    /// <returns><c>true</c> if the notification time is valid; otherwise, <c>false</c>.</returns>
    public bool IsValidNotifyTime(DateTimeOffset timeNow, DateTimeOffset? notifyTime)
    {
        var (startTime, _) = GetNotifyTimeRange(timeNow);

        return startTime <= notifyTime;
    }

    /// <summary>
    /// Determines if the notification time is valid for showing later.
    /// </summary>
    /// <param name="timeNow">The current time.</param>
    /// <param name="notifyTime">The notification time to validate.</param>
    /// <returns><c>true</c> if the notification time is valid for showing later; otherwise, <c>false</c>.</returns>
    public bool IsValidShowLaterTime(DateTimeOffset timeNow, DateTimeOffset? notifyTime)
    {
        var (_, endTime) = GetNotifyTimeRange(timeNow);

        return notifyTime > endTime;
    }

    /// <summary>
    /// Determines if the notification time is valid for showing now.
    /// </summary>
    /// <param name="timeNow">The current time.</param>
    /// <param name="notifyTime">The notification time to validate.</param>
    /// <returns><c>true</c> if the notification time is valid for showing now; otherwise, <c>false</c>.</returns>
    public bool IsValidShowNowTime(DateTimeOffset timeNow, DateTimeOffset? notifyTime)
    {
        var (startTime, endTime) = GetNotifyTimeRange(timeNow);

        return startTime <= notifyTime && notifyTime <= endTime;
    }

    /// <summary>
    /// Gets the range of valid notification times based on the current time and allowed delay.
    /// </summary>
    /// <param name="timeNow">The current time.</param>
    /// <returns>A tuple containing the start and end times for valid notification display.</returns>
    private (DateTimeOffset StartTime, DateTimeOffset EndTime) GetNotifyTimeRange(DateTimeOffset timeNow)
    {
        var startTime = timeNow.Subtract(AllowedDelay);
        var endTime = timeNow.AddMinutes(1);

        return (startTime, endTime);
    }
}