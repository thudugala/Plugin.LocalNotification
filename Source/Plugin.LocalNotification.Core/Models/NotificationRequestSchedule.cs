using Plugin.LocalNotification.Core.Models.AndroidOption;

namespace Plugin.LocalNotification.Core.Models;

/// <summary>
/// Schedule notification
/// </summary>
public class NotificationRequestSchedule
{
    /// <summary>
    /// Android specific properties.
    /// </summary>
    public AndroidScheduleOptions Android { get; set; } = new();

    /// <summary>
    /// Time to cancel the notification automatically.
    /// </summary>
    public DateTimeOffset? NotifyAutoCancelTime { get; set; }

    /// <summary>
    /// if Repeats = TimeInterval, then repeat again after specified amount of time elapses
    /// </summary>
    public TimeSpan? NotifyRepeatInterval { get; set; }

    /// <summary>
    /// Time to show the notification.
    /// </summary>
    public DateTimeOffset? NotifyTime { get; set; }

    /// <summary>
    /// If true, will repeat again at the time specifies in NotifyTime or NotifyRepeatInterval
    /// </summary>
    public NotificationRepeat RepeatType { get; set; } = NotificationRepeat.No;
}