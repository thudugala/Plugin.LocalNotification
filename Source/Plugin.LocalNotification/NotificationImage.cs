using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public class NotificationImage
    {
        /// <summary>
        ///
        /// </summary>
        public byte[] Binary { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool HasValue => string.IsNullOrWhiteSpace(ResourceName) ||
                                string.IsNullOrWhiteSpace(FilePath) ||
                                (Binary != null && Binary.Length > 0);

        /// <summary>
        ///
        /// </summary>
        public string ResourceName { get; set; }
    }
}