using Foundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification.Platform.iOS
{
    /// <inheritdoc />
    [Foundation.Preserve]
    public class LocalNotificationServiceImpl : ILocalNotificationService
    {
        private readonly List<string> _notificationList;

        /// <inheritdoc />
        public event LocalNotificationTappedEventHandler NotificationTapped;

        /// <inheritdoc />
        public void OnNotificationTapped(LocalNotificationTappedEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        /// <inheritdoc />
        public LocalNotificationServiceImpl()
        {
            _notificationList = new List<string>();
        }

        /// <inheritdoc />
        public void Cancel(int notificationId)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return;
                }

                var item = notificationId.ToString();
                var itemList = new[] { notificationId.ToString() };
                _notificationList.Remove(item);

                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(itemList);
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
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return;
                }

                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(_notificationList.ToArray());
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(_notificationList.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void Show(LocalNotificationRequest localNotificationRequest)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return;
                }

                if (localNotificationRequest is null)
                {
                    return;
                }

                var userInfoDictionary = new NSMutableDictionary();

                userInfoDictionary.SetValueForKey(
                    string.IsNullOrWhiteSpace(localNotificationRequest.ReturningData)
                        ? NSString.Empty
                        : new NSString(localNotificationRequest.ReturningData), CrossLocalNotificationService.ExtraReturnDataIos);

                var content = new UNMutableNotificationContent
                {
                    Title = localNotificationRequest.Title,
                    Body = localNotificationRequest.Description,
                    Badge = localNotificationRequest.BadgeNumber,
                    UserInfo = userInfoDictionary,
                    Sound = UNNotificationSound.Default
                };

                var trigger =
                    UNCalendarNotificationTrigger.CreateTrigger(
                        GetNsDateComponentsFromDateTime(localNotificationRequest.NotifyTime), false);

                _notificationList.Add(localNotificationRequest.NotificationId.ToString());
                var request = UNNotificationRequest.FromIdentifier(localNotificationRequest.NotificationId.ToString(),
                    content, trigger);

                UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static NSDateComponents GetNsDateComponentsFromDateTime(DateTime? nullableDateTime)
        {
            var dateTime = nullableDateTime ?? DateTime.Now.AddSeconds(1);

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
    }
}