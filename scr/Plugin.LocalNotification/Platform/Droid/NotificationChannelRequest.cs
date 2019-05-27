using Android.App;
using Android.Graphics;

namespace Plugin.LocalNotification.Platform.Droid
{
    /// <summary>
    /// Notification Channel Request
    /// </summary>
    public class NotificationChannelRequest
    {
        /// <summary>
        /// Sets or gets, the level of interruption of this notification channel. 
        /// </summary>
        public NotificationImportance Importance { get; set; } = NotificationImportance.Default;

        /// <summary>
        /// Sets or gets, the user visible name of this channel, default is General.
        /// </summary>
        public string Name { get; set; } = "General";

        /// <summary>
        /// Sets or gets, the user visible description of this channel. 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Sets or gets, what group this channel belongs to.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Sets or gets, the notification light color for notifications posted to this channel,
        /// if the device supports that feature
        /// </summary>
        public Color LightColor { get; set; }


        /// <summary>
        /// Sound file name for the notification.
        /// </summary>
        public string Sound { get; set; }

        /// <summary>
        /// Sets or gets, whether or not notifications posted to this channel are shown on the lockscreen in full or redacted form. 
        /// </summary>
        public NotificationVisibility LockscreenVisibility { get; set; } = NotificationVisibility.Secret;
    }
}