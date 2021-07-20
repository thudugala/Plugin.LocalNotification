namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    ///
    /// </summary>
    public class AndroidIcon
    {
        /// <summary>
        ///
        /// </summary>
        public AndroidIcon()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resourceName"></param>
        public AndroidIcon(string resourceName)
        {
            ResourceName = resourceName;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="type"></param>
        public AndroidIcon(string resourceName, string type)
        {
            ResourceName = resourceName;
            Type = type ?? DefaultType;
        }

        /// <summary>
        /// Default Group Name
        /// </summary>
        public static string DefaultType => "drawable";

        /// <summary>
        /// The name of the desired resource
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// Optional default resource type to find, if "type/" is
        /// not included in the name.  Can be null to require an
        /// explicit type.
        /// </summary>
        public string Type { get; set; } = DefaultType;
    }
}