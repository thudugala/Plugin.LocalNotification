namespace Plugin.LocalNotification.AndroidOption;

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
    /// Gets or sets the allowed delay for scheduling notifications. If <c>NotifyTime</c> is earlier than <c>DateTime.Now</c> and this delay, notification will not be scheduled or shown. Default is 1 minute.
    /// </summary>
    public TimeSpan AllowedDelay { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Calculates the next notification time for a repeating notification request.
    /// </summary>
    /// <param name="notifyTime">The original notification time.</param>
    /// <param name="repeatType">The type of repeat interval.</param>
    /// <param name="notifyRepeatInterval">The custom repeat interval, if applicable.</param>
    /// <returns>The next notification time, or <c>null</c> if not applicable.</returns>
    internal DateTime? GetNextNotifyTimeForRepeat(DateTime? notifyTime, NotificationRepeat repeatType, TimeSpan? notifyRepeatInterval)
    {
        // NotifyTime does not change for Repeat request
        if (notifyTime is null)
        {
            return null;
        }

        var repeatInterval = GetNotifyRepeatInterval(repeatType, notifyRepeatInterval);
        if (repeatInterval == TimeSpan.Zero)
        {
            return null;
        }

        var newNotifyTime = notifyTime.Value.Add(repeatInterval);
        var nowTime = DateTime.Now.AddSeconds(10);
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
    internal TimeSpan GetNotifyRepeatInterval(NotificationRepeat repeatType, TimeSpan? notifyRepeatInterval)
    {
        var repeatInterval = TimeSpan.Zero;
        switch (repeatType)
        {
            case NotificationRepeat.Daily:
                // To be consistent with iOS, Schedule notification next day same time.
                repeatInterval = TimeSpan.FromDays(1);
                break;

            case NotificationRepeat.Weekly:
                // To be consistent with iOS, Schedule notification next week same day same time.
                repeatInterval = TimeSpan.FromDays(7);
                break;

            case NotificationRepeat.TimeInterval:
                if (notifyRepeatInterval.HasValue)
                {
                    repeatInterval = notifyRepeatInterval.Value;
                }
                break;

            case NotificationRepeat.No:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        return repeatInterval;
    }

    /// <summary>
    /// Determines if the notification time is valid for showing, consistent with iOS behavior.
    /// </summary>
    /// <param name="timeNow">The current time.</param>
    /// <param name="notifyTime">The notification time to validate.</param>
    /// <returns><c>true</c> if the notification time is valid; otherwise, <c>false</c>.</returns>
    internal bool IsValidNotifyTime(DateTime timeNow, DateTime? notifyTime)
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
    internal bool IsValidShowLaterTime(DateTime timeNow, DateTime? notifyTime)
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
    internal bool IsValidShowNowTime(DateTime timeNow, DateTime? notifyTime)
    {
        var (startTime, endTime) = GetNotifyTimeRange(timeNow);

        return startTime <= notifyTime && notifyTime <= endTime;
    }

    /// <summary>
    /// Gets the range of valid notification times based on the current time and allowed delay.
    /// </summary>
    /// <param name="timeNow">The current time.</param>
    /// <returns>A tuple containing the start and end times for valid notification display.</returns>
    private (DateTime StartTime, DateTime EndTime) GetNotifyTimeRange(DateTime timeNow)
    {
        var startTime = timeNow.Subtract(AllowedDelay);
        var endTime = timeNow.AddMinutes(1);

        return (startTime, endTime);
    }
}