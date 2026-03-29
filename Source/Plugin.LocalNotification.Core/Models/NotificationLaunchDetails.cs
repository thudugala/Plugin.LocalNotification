namespace Plugin.LocalNotification.Core.Models;

/// <summary>
/// Contains details about whether the application was launched by tapping on a notification
/// created by this plugin.
/// </summary>
public class NotificationLaunchDetails
{
    /// <summary>
    /// Gets a value indicating whether the application was launched by tapping on a notification.
    /// </summary>
    public bool DidNotificationLaunchApp { get; init; }

    /// <summary>
    /// Gets the notification request associated with the launch notification, or <c>null</c>
    /// if the app was not launched from a notification.
    /// </summary>
    public NotificationRequest? Request { get; init; }

    /// <summary>
    /// Gets the action identifier that was tapped, or <c>null</c> if the app was not launched
    /// from a notification. A value equal to <c>NotificationActionEventArgs.TapActionId</c>
    /// indicates the notification body itself was tapped.
    /// </summary>
    public int? ActionId { get; init; }
}
