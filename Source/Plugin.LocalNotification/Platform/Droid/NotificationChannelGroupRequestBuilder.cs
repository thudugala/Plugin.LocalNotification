namespace Plugin.LocalNotification.Platform.Droid
{
    /// <summary>
    ///
    /// </summary>
    public class NotificationChannelGroupRequestBuilder
    {
        private readonly NotificationChannelGroupRequest _groupRequest;

        /// <summary>
        ///
        /// </summary>
        public NotificationChannelGroupRequestBuilder()
        {
            _groupRequest = new NotificationChannelGroupRequest();
        }

        /// <summary>
        /// Builds the request to <see cref="NotificationChannelGroupRequest"/>
        /// </summary>
        /// <returns></returns>
        public NotificationChannelGroupRequest Build() => _groupRequest;

        /// <summary>
        /// Sets the Group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public NotificationChannelGroupRequestBuilder WithGroup(string group)
        {
            _groupRequest.Group = group ?? AndroidOptions.DefaultGroupId;
            return this;
        }

        /// <summary>
        /// Sets the Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public NotificationChannelGroupRequestBuilder WithName(string name)
        {
            _groupRequest.Name = name ?? AndroidOptions.DefaultGroupName;
            return this;
        }
    }
}