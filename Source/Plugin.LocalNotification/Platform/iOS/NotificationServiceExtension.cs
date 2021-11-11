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

        #endregion Computed Properties

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

        #endregion Constructors

        #region Override Methods

        /// <inheritdoc />
        public override async void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
        {
            try
            {
                ContentHandler = contentHandler;

                BestAttemptContent = (UNMutableNotificationContent)request.Content.MutableCopy();

                var notificationService = TryGetDefaultIOsNotificationService();
                var notificationRequest = notificationService.GetRequest(request.Content);

                if (notificationService.NotificationReceiving != null)
                {
                    var requestArg = await notificationService.NotificationReceiving(notificationRequest);
                    if (requestArg != null)
                    {
                        var newtContent = await notificationService.GetNotificationContent(requestArg.Request);

                        BestAttemptContent.Title = newtContent.Title;
                        BestAttemptContent.Subtitle = newtContent.Subtitle;
                        BestAttemptContent.Body = newtContent.Body;
                        BestAttemptContent.Badge = newtContent.Badge;
                        BestAttemptContent.UserInfo = newtContent.UserInfo;
                        BestAttemptContent.Sound = newtContent.Sound;
                        BestAttemptContent.Attachments = newtContent.Attachments;
                        BestAttemptContent.CategoryIdentifier = newtContent.CategoryIdentifier;

                        BestAttemptContent.UserInfo = GetUserInfo(requestArg.Request, requestArg.Handled);
                    }
                    else
                    {
                        BestAttemptContent.UserInfo = GetUserInfo(notificationRequest, true);
                    }
                }
                ContentHandler(BestAttemptContent);
            }
            catch (Exception ex)
            {
                NotificationCenter.Log(ex);
            }
        }

        private NSMutableDictionary GetUserInfo(NotificationRequest request, bool handled)
        {
            var userInfoDictionary = new NSMutableDictionary();
            var dictionary = NotificationCenter.GetRequestSerializeDictionary(request);
            foreach (var item in dictionary)
            {
                userInfoDictionary.SetValueForKey(new NSString(item.Value), new NSString(item.Key));
            }
            userInfoDictionary.SetValueForKey(NSNumber.FromBoolean(true), new NSString(NotificationCenter.ReturnRequestHandled));
            return userInfoDictionary;
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

        #endregion Override Methods

        private static NotificationServiceImpl TryGetDefaultIOsNotificationService()
        {
            return NotificationCenter.Current is NotificationServiceImpl notificationService
                ? notificationService
                : new NotificationServiceImpl();
        }
    }
}