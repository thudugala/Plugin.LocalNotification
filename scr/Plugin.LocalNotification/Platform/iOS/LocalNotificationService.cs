using Foundation;
using Plugin.LocalNotification.Platform.iOS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIKit;
using UserNotifications;

[assembly: Xamarin.Forms.Dependency(typeof(LocalNotificationService))]
namespace Plugin.LocalNotification.Platform.iOS
{
    /// <inheritdoc />
    [Preserve]
    public class LocalNotificationService : ILocalNotificationService
    {
        private readonly List<string> _notificationList;

        /// <inheritdoc />
        public LocalNotificationService()
        {
            _notificationList = new List<string>();
        }

        /// <inheritdoc />
        public void Cancel(int notificationId)
        {
            try
            {
                var item = notificationId.ToString();
                var itemList = new[] { notificationId.ToString() };
                _notificationList.Remove(item);
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(itemList);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void CancelAll()
        {
            try
            {
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(_notificationList.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void Send(LocalNotification localNotification)
        {
            try
            {
                var userInfoDictionary = new NSMutableDictionary();
                foreach (var data in localNotification.ReturningData)
                {
                    userInfoDictionary.SetValueForKey(new NSString(data), new NSString(data));
                }

                var content = new UNMutableNotificationContent
                {
                    Title = localNotification.Title,
                    UserInfo = userInfoDictionary,
                    Body = localNotification.Description,
                    Badge = localNotification.BadgeNumber
                };

                var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(10, false);
                _notificationList.Add(localNotification.NotificationId.ToString());
                var request = UNNotificationRequest.FromIdentifier(localNotification.NotificationId.ToString(), content, trigger);
                UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
                {
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Setup Local Notification
        /// </summary>
        public static void SetupLocalNotification()
        {
            try
            {
                var alertsAllowed = false;

                UNUserNotificationCenter.Current.GetNotificationSettings((settings) =>
                {
                    alertsAllowed = (settings.AlertSetting == UNNotificationSetting.Enabled);
                });
                if (!alertsAllowed)
                {
                    if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                    {
                        // Ask the user for permission to get notifications on iOS 10.0+
                        UNUserNotificationCenter.Current.RequestAuthorization(
                            UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                            (approved, error) => { });
                    }
                    else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                    {
                        // Ask the user for permission to get notifications on iOS 8.0+
                        var settings = UIUserNotificationSettings.GetSettingsForTypes(
                            UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                            new NSSet());

                        UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                    }
                }
                UNUserNotificationCenter.Current.Delegate = new LocalNotificationDelegate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}