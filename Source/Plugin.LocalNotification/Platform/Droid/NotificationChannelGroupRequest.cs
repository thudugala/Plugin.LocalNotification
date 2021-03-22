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
        /// Default ctor
        /// </summary>
        /// <param name="group"></param>
        /// <param name="name"></param>
        public NotificationChannelGroupRequest(string group, string name)
        {
            Group = group;
            Name = name;
        }
    }
}