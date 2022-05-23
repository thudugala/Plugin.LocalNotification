using Plugin.LocalNotification.AndroidOption;

namespace Plugin.LocalNotification.Platforms.Android
{
    /// <summary>
    /// A grouping of related notification channels
    /// </summary>
    public class NotificationChannelGroupRequest
    {
        /// <summary>
        /// The id of the group. Must be unique per package. the value may be truncated if it is too long
        /// </summary>
        public string Group { get; set; } = AndroidOptions.DefaultGroupId;

        /// <summary>
        /// The user visible name of the group, The recommended maximum length is 40 characters; the value may be truncated if it is too long.
        /// </summary>
        public string Name { get; set; } = AndroidOptions.DefaultGroupName;

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
        /// Default Constructor
        /// </summary>
        public NotificationChannelGroupRequest()
        {
        }
    }
}