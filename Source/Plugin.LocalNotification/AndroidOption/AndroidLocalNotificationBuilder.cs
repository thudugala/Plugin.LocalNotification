namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    ///
    /// </summary>
    public class AndroidLocalNotificationBuilder : IAndroidLocalNotificationBuilder
    {        
        internal IList<NotificationChannelRequest> ChannelRequestList { get; } = new List<NotificationChannelRequest>();
               
        internal IList<NotificationChannelGroupRequest> GroupChannelRequestList { get; } = new List<NotificationChannelGroupRequest>();
               
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