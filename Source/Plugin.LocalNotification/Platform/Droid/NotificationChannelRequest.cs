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
        public string Id { get; set; } = AndroidOptions.DefaultChannelId;

        /// <summary>
        /// Sets or gets, the user visible name of this channel, default is General.
        /// </summary>
        public string Name { get; set; } = AndroidOptions.DefaultChannelName;

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
        /// Only modifiable before the channel is submitted.
        /// </summary>
        public long[] VibrationPattern { get; set; }

        /// <summary>
        /// Sets or gets, whether or not notifications posted to this channel are shown on the lock Screen in full or redacted form.
        /// </summary>
        public NotificationVisibility LockScreenVisibility { get; set; } = NotificationVisibility.Secret;

        /// <summary>
        /// Sets or gets, Sets whether notifications posted to this channel can appear as application icon badges in a Launcher.
        /// </summary>
        public bool ShowBadge { get; set; } = true;

        /// <summary>
        /// Sets or gets, Sets whether notifications posted to this channel should display notification lights, on devices that support that feature.
        /// </summary>
        public bool EnableLights { get; set; } = true;

        /// <summary>
        /// Sets or gets, Sets whether notification posted to this channel should vibrate. The vibration pattern can be set with VibrationPattern
        /// </summary>
        public bool EnableVibration { get; set; } = true;

        /// <summary>
        /// Sets or gets, Sets whether notification posted to this channel can bypass DND (Do Not Disturb) mode.
        /// </summary>
        public bool CanBypassDnd { get; set; } = false;

        /// <summary>
        /// Creates a ChannelRequestBuilder with default values.
        /// </summary>
        /// <returns></returns>
        public static NotificationChannelRequestBuilder CreateBuilder() => new NotificationChannelRequestBuilder();

        /// <summary>
        /// Creates a ChannelRequestBuilder with specified channelId.
        /// </summary>
        /// <returns></returns>
        public static NotificationChannelRequestBuilder CreateBuilder(string channelId) => new NotificationChannelRequestBuilder(channelId);
    }
}