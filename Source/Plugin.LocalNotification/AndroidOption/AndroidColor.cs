namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    /// the notification icon and application name will have the provided ARGB color
    /// </summary>
    public class AndroidColor
    {
        /// <summary>
        ///
        /// </summary>
        public AndroidColor()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="argb"></param>
        public AndroidColor(int argb) => Argb = argb;

        /// <summary>
        ///
        /// </summary>
        /// <param name="resourceName"></param>
        public AndroidColor(string resourceName) => ResourceName = resourceName;

        /// <summary>
        /// if set, will ignore ResourceName
        /// </summary>
        public int? Argb { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ResourceName { get; set; } = string.Empty;
    }
}