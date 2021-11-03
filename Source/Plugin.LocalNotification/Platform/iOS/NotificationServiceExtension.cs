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
        public override async void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
        {
            try
            {
                ContentHandler = contentHandler;

                BestAttemptContent = (UNMutableNotificationContent)request.Content.MutableCopy();

                var notificationService = TryGetDefaultIOsNotificationService();
                var notificationRequest = notificationService.GetRequest(request?.Content);

                if (notificationService.NotificationReceiving != null)
                {
                    var newLocalNotification = await notificationService.NotificationReceiving(notificationRequest);
                    if (newLocalNotification != null)
                    {
                        var newtContent = await notificationService.GetNotificationContent(newLocalNotification);

                        BestAttemptContent.Title = newtContent.Title;
                        BestAttemptContent.Subtitle = newtContent.Subtitle;
                        BestAttemptContent.Body = newtContent.Body;
                        BestAttemptContent.Badge = newtContent.Badge;
                        BestAttemptContent.UserInfo = newtContent.UserInfo;
                        BestAttemptContent.Sound = newtContent.Sound;
                        BestAttemptContent.Attachments = newtContent.Attachments;
                        BestAttemptContent.CategoryIdentifier = newtContent.CategoryIdentifier;
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
