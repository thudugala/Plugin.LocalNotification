using Foundation;
using System;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms.iOS
{
    /// <inheritdoc />
    [Register(nameof(NotificationServiceExtension))]
    public class NotificationServiceExtension : UNNotificationServiceExtension
    {
        private Action<UNNotificationContent> ContentHandler { get; set; }
        private UNMutableNotificationContent BestAttemptContent { get; set; }

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

        /// <inheritdoc />
        public override async void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
        {
            try
            {
                ContentHandler = contentHandler;

                BestAttemptContent = (UNMutableNotificationContent)request.Content.MutableCopy();

                var notificationService = TryGetDefaultIOsNotificationService();
                if (notificationService.NotificationReceiving is null)
                {
                    ContentHandler(BestAttemptContent);

                    LocalNotificationCenter.Log("Notification Receiving not registered");
                    return;
                }

                var notificationRequest = notificationService.GetRequest(request.Content);

                // if notificationRequest is null this maybe not a notification from this plugin.
                if (notificationRequest is null)
                {
                    ContentHandler(BestAttemptContent);

                    LocalNotificationCenter.Log("Notification request not found");
                    return;
                }

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
                    BestAttemptContent.UserInfo = GetUserInfo(notificationRequest, false);
                }

                ContentHandler(BestAttemptContent);
            }
            catch (Exception ex)
            {
                LocalNotificationCenter.Log(ex);
            }
        }

        private static NSMutableDictionary GetUserInfo(NotificationRequest request, bool handled)
        {
            var userInfoDictionary = new NSMutableDictionary();
            var dictionary = LocalNotificationCenter.GetRequestSerializeDictionary(request);
            foreach (var item in dictionary)
            {
                userInfoDictionary.SetValueForKey(new NSString(item.Value), new NSString(item.Key));
            }
            userInfoDictionary.SetValueForKey(NSNumber.FromBoolean(handled), new NSString(LocalNotificationCenter.ReturnRequestHandled));
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
                LocalNotificationCenter.Log(ex);
            }
        }

        private static NotificationServiceImpl TryGetDefaultIOsNotificationService()
        {
            return LocalNotificationCenter.Current is NotificationServiceImpl notificationService
                ? notificationService
                : new NotificationServiceImpl();
        }
    }
}