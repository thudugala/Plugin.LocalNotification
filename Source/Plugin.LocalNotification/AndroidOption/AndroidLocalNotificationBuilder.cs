namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    ///
    /// </summary>
    public class AndroidLocalNotificationBuilder : IAndroidLocalNotificationBuilder
    {
        /// <summary>
        ///
        /// </summary>
        public IList<NotificationChannelRequest> ChannelRequestList { get; }

        /// <summary>
        ///
        /// </summary>
        public IList<NotificationChannelGroupRequest> GroupChannelRequestList { get; }

        /// <summary>
        ///
        /// </summary>
        public AndroidLocalNotificationBuilder()
        {
            ChannelRequestList = new List<NotificationChannelRequest>();
            GroupChannelRequestList = new List<NotificationChannelGroupRequest>();
        }

        /// <inheritdoc/>
        public IAndroidLocalNotificationBuilder AddChannel(NotificationChannelRequest channelRequest)
        {
            ChannelRequestList.Add(channelRequest);
            return this;
        }

        /// <inheritdoc/>
        public IAndroidLocalNotificationBuilder AddChannelGroup(NotificationChannelGroupRequest groupChannelRequest)
        {
            GroupChannelRequestList.Add(groupChannelRequest);
            return this;
        }
    }
}