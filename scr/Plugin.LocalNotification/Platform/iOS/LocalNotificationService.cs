using Foundation;
using Plugin.LocalNotification.Platform.iOS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIKit;
using UserNotifications;

[assembly: Xamarin.Forms.Dependency(typeof(LocalNotificationService))]
namespace Plugin.LocalNotification.Platform.iOS
{
    /// <inheritdoc />
    [Foundation.Preserve]
    public class LocalNotificationService : ILocalNotificationService
    {
        /// <summary>
        /// Return Data Key.
        /// </summary>
        internal static NSString ExtraReturnData = new NSString("Plugin.LocalNotification.Platform.iOS.RETURN_DATA");

        /// <summary>
        /// Return Notification Key
        /// </summary>
        internal static NSString ExtraNotificationKey = new NSString("Plugin.LocalNotification.Platform.iOS.NOTIFICATION_KEY");

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

                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                {
                    UNUserNotificationCenter.Current.RemovePendingNotificationRequests(itemList);
                    UNUserNotificationCenter.Current.RemoveDeliveredNotifications(itemList);
                }
                else
                {
                    var notifications = UIApplication.SharedApplication.ScheduledLocalNotifications;

                    var notification = notifications.Where(n => n.UserInfo.ContainsKey(ExtraNotificationKey))
                        .FirstOrDefault(n => n.UserInfo[ExtraNotificationKey].ToString() == notificationId.ToString());

                    if (notification != null)
                    {
                        UIApplication.SharedApplication.CancelLocalNotification(notification);
                    }
                }
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
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                {
                    UNUserNotificationCenter.Current.RemovePendingNotificationRequests(_notificationList.ToArray());
                    UNUserNotificationCenter.Current.RemoveDeliveredNotifications(_notificationList.ToArray());
                }
                else
                {
                    var notifications = UIApplication.SharedApplication.ScheduledLocalNotifications;
                    var notificationsToCancel = notifications.Where(n => n.UserInfo.ContainsKey(ExtraNotificationKey));
                    foreach (var notification in notificationsToCancel)
                    {
                        UIApplication.SharedApplication.CancelLocalNotification(notification);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void Show(LocalNotification localNotification)
        {
            try
            {
                if (localNotification is null)
                {
                    return;
                }

                var userInfoDictionary = new NSMutableDictionary();
                if (string.IsNullOrWhiteSpace(localNotification.ReturningData) == false)
                {
                    userInfoDictionary.SetValueForKey(ExtraReturnData, new NSString(localNotification.ReturningData));
                }

                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                {
                    var content = new UNMutableNotificationContent
                    {
                        Title = localNotification.Title,
                        Body = localNotification.Description,
                        Badge = localNotification.BadgeNumber,
                        UserInfo = userInfoDictionary
                    };

                    var trigger =
                        UNCalendarNotificationTrigger.CreateTrigger(
                            GetNSDateComponentsFromDateTime(localNotification.NotifyTime), false);

                    _notificationList.Add(localNotification.NotificationId.ToString());
                    var request = UNNotificationRequest.FromIdentifier(localNotification.NotificationId.ToString(),
                        content, trigger);
                    UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) => { });
                }
                else
                {
                    var fireDate = DateTime.Now;
                    if (localNotification.NotifyTime.HasValue)
                    {
                        fireDate = localNotification.NotifyTime.Value;
                    }

                    userInfoDictionary.SetValueForKey(ExtraNotificationKey,
                        new NSString(localNotification.NotificationId.ToString()));

                    var notification = new UILocalNotification
                    {
                        FireDate = (NSDate)fireDate,
                        AlertTitle = localNotification.Title,
                        AlertBody = localNotification.Description,
                        UserInfo = userInfoDictionary
                    };

                    UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private NSDateComponents GetNSDateComponentsFromDateTime(DateTime? nullableDateTime)
        {
            if (!nullableDateTime.HasValue)
            {
                return new NSDateComponents
                {
                    Second = 1
                };
            }

            var dateTime = nullableDateTime.Value;

            return new NSDateComponents
            {
                Month = dateTime.Month,
                Day = dateTime.Day,
                Year = dateTime.Year,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            };
        }

        /// <summary>
        /// Setup Local Notification
        /// </summary>
        public static void Init()
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