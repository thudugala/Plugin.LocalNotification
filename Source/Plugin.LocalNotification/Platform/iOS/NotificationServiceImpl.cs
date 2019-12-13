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
        /// <inheritdoc />
        public event NotificationTappedEventHandler NotificationTapped;

        /// <inheritdoc />
        public void OnNotificationTapped(NotificationTappedEventArgs e)
        {
            NotificationTapped?.Invoke(e);
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

                var itemList = new[]
                {
                    notificationId.ToString(CultureInfo.CurrentCulture)
                };

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

                UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
                UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public async void Show(NotificationRequest notificationRequest)
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

                var allowed = await NotificationCenter.AskPermissionAsync().ConfigureAwait(false);
                if (allowed == false)
                {
                    return;
                }

                var userInfoDictionary = new NSMutableDictionary();

                if (string.IsNullOrWhiteSpace(notificationRequest.ReturningData) == false)
                {
                    using (var returningData = new NSString(notificationRequest.ReturningData))
                    {
                        userInfoDictionary.SetValueForKey(
                            string.IsNullOrWhiteSpace(notificationRequest.ReturningData)
                                ? NSString.Empty
                                : returningData, NotificationCenter.ExtraReturnDataIos);
                    }
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
                        using (var trigger = UNCalendarNotificationTrigger.CreateTrigger(notifyTime, notificationRequest.Repeats))
                        {
                            var notificationId =
                                notificationRequest.NotificationId.ToString(CultureInfo.CurrentCulture);

                            var request = UNNotificationRequest.FromIdentifier(notificationId, content, trigger);

                            await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request).ConfigureAwait(false);
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