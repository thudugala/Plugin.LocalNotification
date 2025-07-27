namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Defines methods for building Android local notification options, including channels and channel groups.
/// </summary>
public interface IAndroidLocalNotificationBuilder
{
    /// <summary>
    /// Adds a notification channel with the specified settings. Creates the channel when API >= 26.
    /// </summary>
    /// <param name="channelRequest">The channel request containing channel settings.</param>
    /// <returns>The builder instance for chaining.</returns>
    IAndroidLocalNotificationBuilder AddChannel(NotificationChannelRequest channelRequest);

    /// <summary>
    /// A grouping of related notification channels. e.g., channels that all belong to a single account.
    /// Create Notification Channel Group when API >= 26.
    /// If you'd like to further organize the appearance of your channels in the settings UI, you can create channel groups.
    /// This is a good idea when your app supports multiple user accounts (such as for work profiles),
    /// so you can create a notification channel group for each account.
    /// This way, users can easily identify and control multiple notification channels that have identical names.
    /// </summary>
    /// <param name="groupChannelRequest">The channel group request containing group settings.</param>
    /// <returns>The builder instance for chaining.</returns>
    IAndroidLocalNotificationBuilder AddChannelGroup(NotificationChannelGroupRequest groupChannelRequest);
}