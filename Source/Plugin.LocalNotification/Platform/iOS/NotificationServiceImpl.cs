using Foundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification.Platform.iOS
{
    /// <inheritdoc />
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

                var item = notificationId.ToString(CultureInfo.CurrentCulture);
                var itemList = new[] { notificationId.ToString(CultureInfo.CurrentCulture) };
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

                using (var returningData = new NSString(notificationRequest.ReturningData))
                {
                    userInfoDictionary.SetValueForKey(
                        string.IsNullOrWhiteSpace(notificationRequest.ReturningData)
                            ? NSString.Empty
                            : returningData, NotificationCenter.ExtraReturnDataIos);
                }

                using (var content = new UNMutableNotificationContent
                {
                    Title = notificationRequest.Title,
                    Body = notificationRequest.Description,
                    Badge = notificationRequest.BadgeNumber,
                    UserInfo = userInfoDictionary,
                    Sound = UNNotificationSound.Default
                })
                {
                    if (string.IsNullOrWhiteSpace(notificationRequest.Sound) == false)
                    {
                        content.Sound = UNNotificationSound.GetSound(notificationRequest.Sound);
                    }

                    using (var notifyTime = GetNsDateComponentsFromDateTime(notificationRequest.NotifyTime))
                    {
                        using (var trigger = UNCalendarNotificationTrigger.CreateTrigger(notifyTime, false))
                        {
                            var notificationId = notificationRequest.NotificationId.ToString(CultureInfo.CurrentCulture);

                            _notificationList.Add(notificationId);
                            var request = UNNotificationRequest.FromIdentifier(notificationId, content, trigger);

                            UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);
                        }
                    }
                }
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