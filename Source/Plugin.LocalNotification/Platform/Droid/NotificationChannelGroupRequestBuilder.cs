namespace Plugin.LocalNotification.Platform.Droid
{
    /// <summary>
    ///
    /// </summary>
    public class NotificationChannelGroupRequestBuilder
    {
        private string _group;
        private string _name;

        internal NotificationChannelGroupRequestBuilder()
        {
            _group = AndroidOptions.DefaultGroupId;
            _name = AndroidOptions.DefaultGroupName;
        }

        /// <summary>
        /// Builds the request to <see cref="NotificationChannelGroupRequest"/>
        /// </summary>
        /// <returns></returns>
        public NotificationChannelGroupRequest Build() => new NotificationChannelGroupRequest()
        {
            Group = _group,
            Name = _name
        };

        /// <summary>
        /// Sets the Group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public NotificationChannelGroupRequestBuilder WithGroup(string group)
        {
            _group = group ?? AndroidOptions.DefaultGroupId;
            return this;
        }

        /// <summary>
        /// Sets the Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public NotificationChannelGroupRequestBuilder WithName(string name)
        {
            _name = name ?? AndroidOptions.DefaultGroupName;
            return this;
        }
    }
}