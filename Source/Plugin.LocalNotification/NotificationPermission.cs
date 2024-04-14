using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public class NotificationPermission
    {
        /// <summary>
        ///
        /// </summary>
        public bool AskPermission { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public AndroidNotificationPermission Android { get; set; } = new();

        /// <summary>
        ///
        /// </summary>
        public iOSNotificationPermission IOS { get; set; } = new();
    }
}