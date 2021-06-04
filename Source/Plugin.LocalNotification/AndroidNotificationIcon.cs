namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public class AndroidNotificationIcon
    {
        /// <summary>
        /// Default Group Name
        /// </summary>
        public static string DefaultType => "drawable";

        /// <summary>
        /// 
        /// </summary>
        public AndroidNotificationIcon()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public AndroidNotificationIcon(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public AndroidNotificationIcon(string name, string type)
        {
            Name = name;
            Type = type ?? DefaultType;
        }

        /// <summary>
        /// The name of the desired resource
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional default resource type to find, if "type/" is
        /// not included in the name.  Can be null to require an
        /// explicit type.
        /// </summary>
        public string Type { get; set; } = DefaultType;
    }
}