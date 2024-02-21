namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAndroidLocalNotificationBuilder
    {
        /// <summary>
        /// A representation of settings that apply to a collection of similarly themed notifications.
        /// Create Notification Channel when API >= 26.
        /// </summary>
        /// <param name="channelRequest"></param>
        /// <returns></returns>
        IAndroidLocalNotificationBuilder AddChannel(NotificationChannelRequest channelRequest);

        /// <summary>
        /// A grouping of related notification channels. e.g., channels that all belong to a single account.
        /// Create Notification Channel Group when API >= 26.
        /// If you'd like to further organize the appearance of your channels in the settings UI, you can create channel groups.
        /// This is a good idea when your app supports multiple user accounts (such as for work profiles),
        /// so you can create a notification channel group for each account.
        /// This way, users can easily identify and control multiple notification channels that have identical names.
        /// </summary>
        /// <param name="groupChannelRequest"></param>
        /// <returns></returns>
        IAndroidLocalNotificationBuilder AddChannelGroup(NotificationChannelGroupRequest groupChannelRequest);
    }
}
