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
    public class NotificationServiceImpl : INotificationService
    {
        private readonly List<string> _notificationList;

        /// <inheritdoc />
        public event NotificationTappedEventHandler NotificationTapped;

        /// <inheritdoc />
        public void OnNotificationTapped(NotificationTappedEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        /// <inheritdoc />
        public NotificationServiceImpl()
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
        public void Show(NotificationRequest notificationRequest)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return;
                }

                if (notificationRequest is null)
                {
                    return;
                }

                var userInfoDictionary = new NSMutableDictionary();

                userInfoDictionary.SetValueForKey(
                    string.IsNullOrWhiteSpace(notificationRequest.ReturningData)
                        ? NSString.Empty
                        : new NSString(notificationRequest.ReturningData), NotificationCenter.ExtraReturnDataIos);

                var content = new UNMutableNotificationContent
                {
                    Title = notificationRequest.Title,
                    Body = notificationRequest.Description,
                    Badge = notificationRequest.BadgeNumber,
                    UserInfo = userInfoDictionary,
                    Sound = UNNotificationSound.Default
                };

                var trigger =
                    UNCalendarNotificationTrigger.CreateTrigger(
                        GetNsDateComponentsFromDateTime(notificationRequest.NotifyTime), false);

                _notificationList.Add(notificationRequest.NotificationId.ToString());
                var request = UNNotificationRequest.FromIdentifier(notificationRequest.NotificationId.ToString(),
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