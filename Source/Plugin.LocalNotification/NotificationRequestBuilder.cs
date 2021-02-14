using System;

namespace Plugin.LocalNotification
{
	/// <summary>
	/// Notification Request Builder
	/// </summary>
	public class NotificationRequestBuilder
	{
		private string NotificationTitle = string.Empty;
		private string NotificationDescription = string.Empty;
		private DateTime? NotifyTime;
		private int NotificationId;
		private int BadgeNumberCount;
		private string ReturningData = string.Empty;
		private AndroidOptions AndroidOptions;
		private iOSOptions iOSOptions;
		private NotificationRepeat RepeatInterval = NotificationRepeat.No;
		private TimeSpanExt? RepeatSpan;
		private string NotificationSound = string.Empty;

		/// <summary>
		/// Initializes NotificationRequestBuilder with the specified notification Id.
		/// </summary>
		/// <param name="notificationId">A unique identifier for the request
		/// (if identifier is not unique, a new notification request object is not created).
		/// You can use this identifier later to cancel a request that is still pending.</param>
		public NotificationRequestBuilder(int notificationId)
		{
			NotificationId = notificationId;
			AndroidOptions = new AndroidOptions();
			iOSOptions = new iOSOptions();
		}

		/// <summary>
		/// Initializes NotificationRequestBuilder with default value.
		/// </summary>
		public NotificationRequestBuilder()
		{
			AndroidOptions = new AndroidOptions();
			iOSOptions = new iOSOptions();
		}

		/// <summary>
		/// A unique identifier for the request
		/// (if identifier is not unique, a new notification request object is not created).
		/// You can use this identifier later to cancel a request that is still pending.
		/// </summary>
		/// <param name="notificationId"></param>
		/// <returns></returns>
		public NotificationRequestBuilder WithNotificationId(int notificationId)
		{
			NotificationId = notificationId;
			return this;
		}

		/// <summary>
		/// Android specific properties.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public NotificationRequestBuilder WithAndroidOptions(AndroidOptions options)
		{
			AndroidOptions = options;
			return this;
		}

		/// <summary>
		/// iOS specific properties.
		/// </summary>
		public NotificationRequestBuilder WithiOSOptions(iOSOptions options)
		{
			iOSOptions = options;
			return this;
		}

		/// <summary>
		/// Title for the notification.
		/// </summary>
		public NotificationRequestBuilder WithTitle(string title)
		{
			NotificationTitle = title;
			return this;
		}

		/// <summary>
		/// Details for the notification.
		/// </summary>
		public NotificationRequestBuilder WithDescription(string description)
		{
			NotificationDescription = description;
			return this;
		}

		/// <summary>
		/// Time to show the notification.
		/// </summary>
		public NotificationRequestBuilder NotifyAt(DateTime? notificationTime)
		{
			NotifyTime = notificationTime;
			return this;
		}

		/// <summary>
		/// Number of the badge displays on the Home Screen.
		/// </summary>
		public NotificationRequestBuilder WithBadgeCount(int count)
		{
			BadgeNumberCount = count;
			return this;
		}

		/// <summary>
		/// Returning data when tapped or received notification.
		/// </summary>
		public NotificationRequestBuilder WithReturningData(string serilizedReturningData)
		{
			ReturningData = serilizedReturningData;
			return this;
		}

		/// <summary>
		/// Sound file name for the notification.
		/// In Android, Only used if Android Api below 26.
		/// Use NotificationCenter.CreateNotificationChannel when Android Api equal or above 26
		/// </summary>
		public NotificationRequestBuilder WithSound(string fileName)
		{
			NotificationSound = fileName;
			return this;
		}

		/// <summary>
		/// if Repeats = TimeInterval, then repeat again after specified amount of time elapses
		/// </summary>
		public NotificationRequestBuilder SetNotificationRepeatInterval(NotificationRepeat repeatInterval, TimeSpanExt? timeSpanExt)
		{
			RepeatInterval = repeatInterval;
			RepeatSpan = timeSpanExt;
			return this;
		}

		/// <summary>
		/// Creates the notification request
		/// </summary>
		/// <returns>The notification request</returns>
		public NotificationRequest Create() => new NotificationRequest()
		{
			Android = AndroidOptions,
			iOS = iOSOptions,
			BadgeNumber = BadgeNumberCount,
			Description = NotificationDescription,
			NotificationId = NotificationId,
			Repeats = RepeatInterval,
			NotifyRepeatInterval = RepeatSpan,
			NotifyTime = NotifyTime,
			ReturningData = ReturningData,
			Sound = NotificationSound,
			Title = NotificationTitle
		};
	}
}
