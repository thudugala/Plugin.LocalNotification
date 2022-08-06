namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAndroidLocalNotificationBuilder
    {
        /// <summary>
        /// A representation of settings that apply to a collection of similarly themed notifications
        /// </summary>
        /// <param name="channelRequest"></param>
        /// <returns></returns>
        IAndroidLocalNotificationBuilder AddChannel(NotificationChannelRequest channelRequest);

        /// <summary>
        /// A grouping of related notification channels. e.g., channels that all belong to a single account.
        /// </summary>
        /// <param name="groupChannelRequest"></param>
        /// <returns></returns>
        IAndroidLocalNotificationBuilder AddChannelGroup(NotificationChannelGroupRequest groupChannelRequest);
    }
}
