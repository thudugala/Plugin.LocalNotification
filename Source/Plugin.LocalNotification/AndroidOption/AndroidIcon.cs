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
        /// <param name="name"></param>
        public AndroidIcon(string name)
        {
            Name = name;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public AndroidIcon(string name, string type)
        {
            Name = name;
            Type = type ?? DefaultType;
        }

        /// <summary>
        /// Default Group Name
        /// </summary>
        public static string DefaultType => "drawable";

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