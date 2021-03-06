using Android.App;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.LocalNotification.Platform.Droid
{
	/// <summary>
	/// Notification channel request builder
	/// </summary>
	public class NotificationChannelRequestBuilder
	{
		private NotificationImportance Importance = NotificationImportance.Default;
		private string ChannelId;
		private string Name;
		private string Description;
		private string Group;
		private Color LightColor;
		private string SoundFile;
		private long[] VibrationPattern;
		private NotificationVisibility LockscreenVisibility = NotificationVisibility.Secret;
		private bool ShowBadge = true;
		private bool EnableLights = true;
		private bool EnableVibration = true;
		private bool BypassDND = false;

		/// <summary>
		/// Initializes builder with the specified channelID.
		/// </summary>
		/// <param name="channelId">The channel id</param>
		public NotificationChannelRequestBuilder(string channelId)
		{
			ChannelId = channelId ?? NotificationCenter.DefaultChannelId;
		}

		/// <summary>
		/// Initializes builder with the default channel id.
		/// </summary>
		public NotificationChannelRequestBuilder()
		{
			ChannelId = NotificationCenter.DefaultChannelId;
		}

		/// <summary>
		/// Sets, the channel id.
		/// </summary>
		public NotificationChannelRequestBuilder WithChannelId(string channelId)
		{
			ChannelId = channelId ?? NotificationCenter.DefaultChannelId;
			return this;
		}

		/// <summary>
		/// Sets, the level of interruption of this notification channel.
		/// </summary>
		public NotificationChannelRequestBuilder WithImportance(NotificationImportance importance)
		{
			Importance = importance;
			return this;
		}

		/// <summary>
		/// Sets, the user visible name of this channel, default is General.
		/// </summary>
		public NotificationChannelRequestBuilder WithName(string name)
		{
			Name = name ?? "General";
			return this;
		}

		/// <summary>
		/// Sets, the user visible description of this channel.
		/// </summary>
		public NotificationChannelRequestBuilder WithDescription(string description)
		{
			Description = description;
			return this;
		}

		/// <summary>
		/// Sets, what group this channel belongs to.
		/// </summary>
		public NotificationChannelRequestBuilder WithGroup(string groupName)
		{
			Group = groupName;
			return this;
		}

		/// <summary>
		/// Sets, the notification light color for notifications posted to this channel,
		/// if the device supports that feature
		/// </summary>
		public NotificationChannelRequestBuilder WithLightColor(Color lightColor)
		{
			LightColor = lightColor;
			return this;
		}

		/// <summary>
		/// Sound file name for the notification.
		/// </summary>
		public NotificationChannelRequestBuilder WithCustomSound(string soundFilePath)
		{
			SoundFile = soundFilePath;
			return this;
		}

		/// <summary>
		/// Only modifiable before the channel is submitted.
		/// </summary>
		public NotificationChannelRequestBuilder WithCustomVibrationPattern(long[] vibrationPattern)
		{
			VibrationPattern = vibrationPattern;
			return this;
		}

		/// <summary>
		/// Sets, whether or not notifications posted to this channel are shown on the lockscreen in full or redacted form.
		/// </summary>
		public NotificationChannelRequestBuilder WithLockscreenVisibility(NotificationVisibility lockscreenVisibility)
		{
			LockscreenVisibility = lockscreenVisibility;
			return this;
		}

		/// <summary>
		/// Sets, Sets whether notifications posted to this channel can appear as application icon badges in a Launcher.
		/// </summary>
		public NotificationChannelRequestBuilder WithBadges(bool value)
		{
			ShowBadge = value;
			return this;
		}

		/// <summary>
		/// Sets, Sets whether notifications posted to this channel should display notification lights, on devices that support that feature.
		/// </summary>
		public NotificationChannelRequestBuilder WithLights(bool value)
		{
			EnableLights = value;
			return this;
		}

		/// <summary>
		/// Sets, Sets whether notification posted to this channel should vibrate. The vibration pattern can be set with VibrationPattern
		/// </summary>
		public NotificationChannelRequestBuilder WithVibration(bool value)
		{
			EnableVibration = value;
			return this;
		}

		/// <summary>
		/// Sets, Sets whether notification posted to this channel can bypass DND (Do Not Distrub) mode.
		/// </summary>
		public NotificationChannelRequestBuilder ShouldBypassDND(bool value)
		{
			BypassDND = value;
			return this;
		}

		/// <summary>
		/// Creates NotificationChannelRequest from this builder.
		/// </summary>
		/// <returns>The notification channel request</returns>
		public NotificationChannelRequest Build()
		{
			return new NotificationChannelRequest()
			{
				Id = ChannelId,
				Description = Description,
				EnableLights = EnableLights,
				EnableVibration = EnableVibration,
				Group = Group,
				Importance = Importance,
				LightColor = LightColor,
				LockscreenVisibility = LockscreenVisibility,
				Name = Name,
				ShowBadge = ShowBadge,
				Sound = SoundFile,
				VibrationPattern = VibrationPattern,
				CanBypassDND = BypassDND
			};
		}
	}
}
