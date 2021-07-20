namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    /// the notification icon and application name will have the provided ARGB color
    /// </summary>
    public class AndroidColor
    {
        /// <summary>
        /// if set, will ignore ResourceName
        /// </summary>
        public int? Argb { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ResourceName { get; set; }
    }
}