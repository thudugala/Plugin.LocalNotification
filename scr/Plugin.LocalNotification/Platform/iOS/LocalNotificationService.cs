﻿using Foundation;
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
        /// <summary>
        /// Return Data Key.
        /// </summary>
        public static NSString ExtraReturnData = new NSString("Plugin.LocalNotification.Platform.iOS.RETURN_DATA");

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
        public void Show(LocalNotification localNotification)
        {
            try
            {
                if (localNotification is null)
                {
                    return;
                }
                
                var userInfoDictionary = new NSMutableDictionary();
                userInfoDictionary.SetValueForKey(ExtraReturnData, new NSString(localNotification.ReturningData));
                
                var content = new UNMutableNotificationContent
                {
                    Title = localNotification.Title,
                    UserInfo = userInfoDictionary,
                    Body = localNotification.Description,
                    Badge = localNotification.BadgeNumber,
                };

                var trigger = UNCalendarNotificationTrigger.CreateTrigger(GetNSDateComponentsFromDateTime(localNotification.NotifyTime), false);

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

        private NSDateComponents GetNSDateComponentsFromDateTime(DateTime? nullableDateTime)
        {
            if (!nullableDateTime.HasValue)
            {
                return new NSDateComponents
                {
                    Second = 10
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