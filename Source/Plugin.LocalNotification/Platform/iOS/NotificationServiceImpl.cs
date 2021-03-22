using Foundation;
using System;
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
        public event NotificationReceivedEventHandler NotificationReceived;

        /// <inheritdoc />
        public void OnNotificationTapped(NotificationTappedEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        /// <inheritdoc />
        public void OnNotificationReceived(NotificationReceivedEventArgs e)
        {
            NotificationReceived?.Invoke(e);
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
        public void Show(Func<NotificationRequestBuilder, NotificationRequest> builder) => Show(builder.Invoke(new NotificationRequestBuilder()));

        /// <inheritdoc />
        public async void Show(NotificationRequest notificationRequest)
        {
            UNNotificationTrigger trigger = null;
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
                    using var returningData = new NSString(notificationRequest.ReturningData);
                    userInfoDictionary.SetValueForKey(
                        string.IsNullOrWhiteSpace(notificationRequest.ReturningData)
                            ? NSString.Empty
                            : returningData, NotificationCenter.ExtraReturnDataIos);
                }

                using var receivedData = new NSString(notificationRequest.iOS.HideForegroundAlert.ToString());
                userInfoDictionary.SetValueForKey(receivedData, NotificationCenter.ExtraNotificationReceivedIos);

                using var soundData = new NSString(notificationRequest.iOS.PlayForegroundSound.ToString());
                userInfoDictionary.SetValueForKey(soundData, NotificationCenter.ExtraSoundInForegroundIos);

                using var content = new UNMutableNotificationContent
                {
                    Title = notificationRequest.Title,
                    Body = notificationRequest.Description,
                    Badge = notificationRequest.BadgeNumber,
                    UserInfo = userInfoDictionary,
                    Sound = UNNotificationSound.Default
                };
                if (string.IsNullOrWhiteSpace(notificationRequest.Sound) == false)
                {
                    content.Sound = UNNotificationSound.GetSound(notificationRequest.Sound);
                }

                var repeats = notificationRequest.Repeats != NotificationRepeat.No;

                if (repeats && notificationRequest.Repeats == NotificationRepeat.TimeInterval &&
                    notificationRequest.NotifyRepeatInterval.HasValue)
                {
                    TimeSpan interval = notificationRequest.NotifyRepeatInterval.Value;

                    trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(interval.TotalSeconds, true);
                }
                else
                {
                    using var notifyTime = GetNsDateComponentsFromDateTime(notificationRequest);
                    trigger = UNCalendarNotificationTrigger.CreateTrigger(notifyTime, repeats);
                }

                var notificationId =
                    notificationRequest.NotificationId.ToString(CultureInfo.CurrentCulture);

                var request = UNNotificationRequest.FromIdentifier(notificationId, content, trigger);

                await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                trigger?.Dispose();
            }
        }

        private static NSDateComponents GetNsDateComponentsFromDateTime(NotificationRequest notificationRequest)
        {
            var dateTime = notificationRequest.NotifyTime ?? DateTime.Now.AddSeconds(1);

            return notificationRequest.Repeats switch
            {
                NotificationRepeat.Daily => new NSDateComponents
                {
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                },
                NotificationRepeat.Weekly => new NSDateComponents
                {
                    // iOS: Weekday units are the numbers 1 through n, where n is the number of days in the week.
                    // For example, in the Gregorian calendar, n is 7 and Sunday is represented by 1.
                    // .Net: The returned value is an integer between 0 and 6,
                    // where 0 indicates Sunday, 1 indicates Monday, 2 indicates Tuesday, 3 indicates Wednesday, 4 indicates Thursday, 5 indicates Friday, and 6 indicates Saturday.
                    Weekday = (int)dateTime.DayOfWeek + 1,
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                },
                NotificationRepeat.No => new NSDateComponents
                {
                    Day = dateTime.Day,
                    Month = dateTime.Month,
                    Year = dateTime.Year,
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                },
                _ => new NSDateComponents
                {
                    Day = dateTime.Day,
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                }
            };
        }
    }
}