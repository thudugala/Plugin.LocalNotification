namespace Plugin.LocalNotification.Core.Models;

/// <summary>
/// Set id Notification should repeat
/// </summary>
public enum NotificationRepeat
{
    /// <summary>
    /// Notification should not repeat
    /// </summary>
    No,

    /// <summary>
    /// Notification should repeat next day at same time
    /// </summary>
    Daily,

    /// <summary>
    /// Notification should repeat next week at same day, same time
    /// </summary>
    Weekly,

    /// <summary>
    /// Notification to be delivered after the specified amount of time elapses
    /// </summary>
    TimeInterval,

    /// <summary>
    /// Notification should repeat on the same day of the month at the same time.
    /// For example, if first scheduled for the 15th at 09:00, it will next fire on the 15th of the following month at 09:00.
    /// </summary>
    Monthly
}