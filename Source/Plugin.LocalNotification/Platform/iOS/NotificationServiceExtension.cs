using Foundation;
using System;
using UserNotifications;

namespace Plugin.LocalNotification.Platform.iOS
{
    /// <inheritdoc />   
    [Register(nameof(NotificationServiceExtension))]
    public class NotificationServiceExtension : UNNotificationServiceExtension
    {
        #region Computed Properties
        private Action<UNNotificationContent> ContentHandler { get; set; }
        private UNMutableNotificationContent BestAttemptContent { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        protected NotificationServiceExtension(NSObjectFlag t) : base(t)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        protected internal NotificationServiceExtension(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
        #endregion

        #region Override Methods

        /// <inheritdoc />
        public override async void DidReceiveNotificationRequest(UNNotificationRequest notificationRequest, Action<UNNotificationContent> contentHandler)
        {
            try
            {
                ContentHandler = contentHandler;

                BestAttemptContent = (UNMutableNotificationContent)notificationRequest.Content.MutableCopy();

                var notificationService = TryGetDefaultIOsNotificationService();
                var request = notificationService.GetRequest(notificationRequest?.Content);

                if (notificationService.CustomizeNotification != null)
                {
                    request = await notificationService.CustomizeNotification(request);
                    if (request != null)
                    {
                        var newContent = await notificationService.GetNotificationContent(request);

                        BestAttemptContent.Title = newContent.Title;
                        BestAttemptContent.Subtitle = newContent.Subtitle;
                        BestAttemptContent.Body = newContent.Body;
                        BestAttemptContent.Badge = newContent.Badge;
                        BestAttemptContent.UserInfo = newContent.UserInfo;
                        BestAttemptContent.Sound = newContent.Sound;
                        BestAttemptContent.Attachments = newContent.Attachments;
                        BestAttemptContent.CategoryIdentifier = newContent.CategoryIdentifier;
                    }
                }

                ContentHandler(BestAttemptContent);
            }
            catch (Exception ex)
            {
                NotificationCenter.Log(ex);
            }
        }

        /// <inheritdoc />
        public override void TimeWillExpire()
        {
            try
            {
                // Called just before the extension will be terminated by the system.
                // Use this as an opportunity to deliver your "best attempt" at modified content, otherwise the original push payload will be used.

                ContentHandler(BestAttemptContent);
            }
            catch (Exception ex)
            {
                NotificationCenter.Log(ex);
            }
        }
        #endregion

        private static NotificationServiceImpl TryGetDefaultIOsNotificationService()
        {
            return NotificationCenter.Current is NotificationServiceImpl notificationService
                ? notificationService
                : new NotificationServiceImpl();
        }
    }
}
