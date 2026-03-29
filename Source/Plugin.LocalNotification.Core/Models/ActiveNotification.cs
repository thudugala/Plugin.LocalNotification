namespace Plugin.LocalNotification.Core.Models;

/// <summary>
/// Represents a notification that is currently displayed in the device's notification center.
/// </summary>
public class ActiveNotification
{
    /// <summary>
    /// Gets the unique identifier of the notification.
    /// </summary>
    public int NotificationId { get; init; }

    /// <summary>
    /// Gets the title of the notification.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets the body text of the notification.
    /// </summary>
    public string? Body { get; init; }

    /// <summary>
    /// Gets the notification channel identifier this notification was posted to (Android 8.0+ / API 26+ only).
    /// Always <see langword="null"/> on non-Android platforms.
    /// </summary>
    public string? ChannelId { get; init; }

    /// <summary>
    /// Gets the Android notification tag used when the notification was posted.
    /// Always <see langword="null"/> on non-Android platforms.
    /// </summary>
    public string? Tag { get; init; }

    /// <summary>
    /// Gets the group key this notification belongs to, used to group related notifications together.
    /// On Android this maps to <c>Notification.Group</c>;
    /// on iOS/macOS it maps to <c>UNNotificationContent.ThreadIdentifier</c>.
    /// </summary>
    public string? GroupKey { get; init; }

    /// <summary>
    /// Gets the expanded body text when the notification was posted with an Android
    /// <c>BigTextStyle</c>. Always <see langword="null"/> on non-Android platforms.
    /// </summary>
    public string? BigText { get; init; }

    /// <summary>
    /// Gets the custom payload associated with the notification.
    /// On iOS and macOS this is the <see cref="NotificationRequest.ReturningData"/> value
    /// stored in the notification's <c>UserInfo</c>.
    /// Always <see langword="null"/> on Android and Windows.
    /// </summary>
    public string? Payload { get; init; }
}
