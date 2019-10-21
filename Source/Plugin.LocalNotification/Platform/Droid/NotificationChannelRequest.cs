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
        /// Sets or gets, The id of the channel. Must be unique per package. The value may be truncated if it is too lon
        /// Also, NotificationRequest.Android.ChannelId must be set to the same Id to target this channel.
        /// </summary>
        public string Id { get; set; } = NotificationCenter.DefaultChannelId;

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

#pragma warning disable CA1819 // Properties should not return arrays
        /// <summary>
        /// Only modifiable before the channel is submitted.
        /// </summary>
        public long[] VibrationPattern { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Sets or gets, whether or not notifications posted to this channel are shown on the lockscreen in full or redacted form.
        /// </summary>
        public NotificationVisibility LockscreenVisibility { get; set; } = NotificationVisibility.Secret;
    }
}