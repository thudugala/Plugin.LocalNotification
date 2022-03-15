using System;

namespace Plugin.LocalNotification.iOSOption
{
    /// <summary>
    /// NotificationRequest for iOS
    /// </summary>
    public class iOSOptions
    {
        /// <summary>
        /// Setting this flag will prevent iOS from displaying the default banner when a Notification is received in foreground
        /// Default is false
        /// </summary>
        public bool HideForegroundAlert { get; set; }

        /// <summary>
        /// Setting this flag will enable iOS to play the default notification sound even if the app is in foreground
        /// Default is false
        /// </summary>
        public bool PlayForegroundSound { get; set; }

        /// <summary>
        /// Setting this will enable iOS to deliver acitve, passive. time-sensentive (high priority) and critical require entitlements
        /// Default is active
        /// </summary>
        public iOSNotificationPriority Priority { get; set; } = iOSNotificationPriority.Active;

        /// <summary>
        /// The system uses the relevanceScore, a value between 0 and 1, to sort the notifications from your app. The highest score gets featured in the notification summary.
        /// </summary>
        public double RelevanceScore { get; set; }

        /// <summary>
        /// An identifier that you use to group related notifications together.
        /// </summary>
        public string ThreadIdentifier { get; set; }
    }
}