namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public class NotificationImage
    {
        /// <summary>
        /// Must be less than 90Kb
        /// </summary>
        public byte[] Binary { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool HasValue => string.IsNullOrWhiteSpace(ResourceName) == false ||
                                string.IsNullOrWhiteSpace(FilePath) == false ||
                                (Binary != null && Binary.Length > 0);

        /// <summary>
        ///
        /// </summary>
        public string ResourceName { get; set; }
    }
}