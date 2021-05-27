namespace Plugin.LocalNotification.Platform.Droid
{
    /// <summary>
    ///
    /// </summary>
    public class NotificationChannelGroupRequest
    {
        /// <summary>
        ///
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor to pass values directly
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        public NotificationChannelGroupRequest(string group, string name)
        {
            Group = group;
            Name = name;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public NotificationChannelGroupRequest()
        {

        }
    }
}