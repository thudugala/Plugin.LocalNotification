using Android.App;
using Android.Graphics;
using Plugin.LocalNotification.AndroidOption;

namespace Plugin.LocalNotification.Platform.Droid
{
    /// <summary>
    /// Notification channel request builder
    /// </summary>
    public class NotificationChannelRequestBuilder
    {
        private readonly NotificationChannelRequest _channelRequest;

        /// <summary>
        /// Initializes builder with the default channel id.
        /// </summary>
        public NotificationChannelRequestBuilder()
        {
            _channelRequest = new NotificationChannelRequest();
        }

        /// <summary>
        /// Initializes builder with the specified channelID.
        /// </summary>
        /// <param name="channelId">The channel id</param>
        public NotificationChannelRequestBuilder(string channelId) : this()
        {
            _channelRequest.Id = channelId ?? AndroidOptions.DefaultChannelId;
        }

        /// <summary>
        /// Sets, the channel id.
        /// </summary>
        public NotificationChannelRequestBuilder WithChannelId(string channelId)
        {
            _channelRequest.Id = channelId ?? AndroidOptions.DefaultChannelId;
            return this;
        }

        /// <summary>
        /// Sets, the level of interruption of this notification channel.
        /// </summary>
        public NotificationChannelRequestBuilder WithImportance(NotificationImportance importance)
        {
            _channelRequest.Importance = importance;
            return this;
        }

        /// <summary>
        /// Sets, the user visible name of this channel, default is General.
        /// </summary>
        public NotificationChannelRequestBuilder WithName(string name)
        {
            _channelRequest.Name = name ?? AndroidOptions.DefaultChannelName;
            return this;
        }

        /// <summary>
        /// Sets, the user visible description of this channel.
        /// </summary>
        public NotificationChannelRequestBuilder WithDescription(string description)
        {
            _channelRequest.Description = description;
            return this;
        }

        /// <summary>
        /// Sets, what group this channel belongs to.
        /// </summary>
        public NotificationChannelRequestBuilder WithGroup(string groupName)
        {
            _channelRequest.Group = groupName;
            return this;
        }

        /// <summary>
        /// Sets, the notification light color for notifications posted to this channel,
        /// if the device supports that feature
        /// </summary>
        public NotificationChannelRequestBuilder WithLightColor(Color lightColor)
        {
            _channelRequest.LightColor = lightColor;
            return this;
        }

        /// <summary>
        /// Sound file name for the notification.
        /// </summary>
        public NotificationChannelRequestBuilder WithSound(string filePath)
        {
            _channelRequest.Sound = filePath;
            return this;
        }

        /// <summary>
        /// Only modifiable before the channel is submitted.
        /// </summary>
        public NotificationChannelRequestBuilder WithVibrationPattern(long[] vibrationPattern)
        {
            _channelRequest.VibrationPattern = vibrationPattern;
            return this;
        }

        /// <summary>
        /// Sets, whether or not notifications posted to this channel are shown on the lock Screen in full or redacted form.
        /// </summary>
        public NotificationChannelRequestBuilder WithLockScreenVisibility(NotificationVisibility lockScreenVisibility)
        {
            _channelRequest.LockScreenVisibility = lockScreenVisibility;
            return this;
        }

        /// <summary>
        /// Sets, Sets whether notifications posted to this channel can appear as application icon badges in a Launcher.
        /// </summary>
        public NotificationChannelRequestBuilder WithBadges(bool value)
        {
            _channelRequest.ShowBadge = value;
            return this;
        }

        /// <summary>
        /// Sets, Sets whether notifications posted to this channel should display notification lights, on devices that support that feature.
        /// </summary>
        public NotificationChannelRequestBuilder WithLights(bool value)
        {
            _channelRequest.EnableLights = value;
            return this;
        }

        /// <summary>
        /// Sets, Sets whether notification posted to this channel should vibrate. The vibration pattern can be set with VibrationPattern
        /// </summary>
        public NotificationChannelRequestBuilder WithVibration(bool value)
        {
            _channelRequest.EnableVibration = value;
            return this;
        }

        /// <summary>
        /// Sets, Sets whether notification posted to this channel can bypass DND (Do Not Disturb) mode.
        /// </summary>
        public NotificationChannelRequestBuilder ShouldBypassDnd(bool value)
        {
            _channelRequest.CanBypassDnd = value;
            return this;
        }

        /// <summary>
        /// Creates NotificationChannelRequest from this builder.
        /// </summary>
        /// <returns>The notification channel request</returns>
        public NotificationChannelRequest Build() => _channelRequest;
    }
}