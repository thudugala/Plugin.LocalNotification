using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Notification Request Builder
    /// </summary>
    public class NotificationRequestBuilder
    {
        private readonly NotificationRequest _request;

        /// <summary>
        /// Initializes NotificationRequestBuilder with the specified notification Id.
        /// </summary>
        /// <param name="notificationId">A unique identifier for the request
        /// (if identifier is not unique, a new notification request object is not created).
        /// You can use this identifier later to cancel a request that is still pending.</param>
        public NotificationRequestBuilder(int notificationId) : this()
        {
            _request.NotificationId = notificationId;
        }

        /// <summary>
        /// Initializes NotificationRequestBuilder with default value.
        /// </summary>
        public NotificationRequestBuilder()
        {
            _request = new NotificationRequest();
        }

        /// <summary>
        /// Creates the notification request
        /// </summary>
        /// <returns>The notification request</returns>
        public NotificationRequest Create() => _request;

        /// <summary>
        /// Android specific properties builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public NotificationRequestBuilder WithAndroidOptions(Func<AndroidOptionsBuilder, AndroidOptions> builder)
        {
            _request.Android = builder.Invoke(new AndroidOptionsBuilder());
            return this;
        }

        /// <summary>
        /// Android specific properties.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public NotificationRequestBuilder WithAndroidOptions(AndroidOptions options)
        {
            _request.Android = options;
            return this;
        }

        /// <summary>
        /// Number of the badge displays on the Home Screen.
        /// </summary>
        public NotificationRequestBuilder WithBadgeNumber(int number)
        {
            _request.BadgeNumber = number;
            return this;
        }

        /// <summary>
        /// Category for the notification.
        /// In Android Must be one of the predefined notification categories
        /// </summary>
        public NotificationRequestBuilder WithCategoryType(NotificationCategoryType type)
        {
            _request.CategoryType = type;
            return this;
        }

        /// <summary>
        /// Details for the notification.
        /// </summary>
        public NotificationRequestBuilder WithDescription(string description)
        {
            _request.Description = description;
            return this;
        }

        /// <summary>
        /// iOS specific properties builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public NotificationRequestBuilder WithiOSOptions(Func<iOSOptionsBuilder, iOSOptions> builder)
        {
            _request.iOS = builder.Invoke(new iOSOptionsBuilder());
            return this;
        }

        /// <summary>
        /// iOS specific properties.
        /// </summary>
        public NotificationRequestBuilder WithiOSOptions(iOSOptions options)
        {
            _request.iOS = options;
            return this;
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
            _request.NotificationId = notificationId;
            return this;
        }

        /// <summary>
        /// Returning data when tapped or received notification.
        /// </summary>
        public NotificationRequestBuilder WithReturningData(string returningData)
        {
            _request.ReturningData = returningData;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public NotificationRequestBuilder WithScheduleOptions(Func<NotificationRequestScheduleBuilder, NotificationRequestSchedule> builder)
        {
            _request.Schedule = builder.Invoke(new NotificationRequestScheduleBuilder());
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public NotificationRequestBuilder WithScheduleOptions(NotificationRequestSchedule options)
        {
            _request.Schedule = options;
            return this;
        }

        /// <summary>
        /// Sound file name for the notification.
        /// In Android, Only used if Android Api below 26.
        /// Use NotificationCenter.CreateNotificationChannel when Android Api equal or above 26
        /// </summary>
        public NotificationRequestBuilder WithSound(string fileName)
        {
            _request.Sound = fileName;
            return this;
        }

        /// <summary>
        /// SubTitle for the notification.
        /// </summary>
        public NotificationRequestBuilder WithSubTitle(string subtitle)
        {
            _request.Subtitle = subtitle;
            return this;
        }

        /// <summary>
        /// Title for the notification.
        /// </summary>
        public NotificationRequestBuilder WithTitle(string title)
        {
            _request.Title = title;
            return this;
        }

        /// <summary>
        /// Title for the notification.
        /// </summary>
        public NotificationRequestBuilder WithSubtitle(string subtitle)
        {
            _request.Subtitle = subtitle;
            return this;
        }
    }
}