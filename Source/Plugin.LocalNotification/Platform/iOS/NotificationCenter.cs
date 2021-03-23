using Foundation;
using Plugin.LocalNotification.Platform.iOS;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification
{
    public static partial class NotificationCenter
    {
        /// <summary>
        /// Return Data Key.
        /// </summary>
        public static NSString ExtraReturnDataIos => new("Plugin.LocalNotification.RETURN_DATA");

        /// <summary>
        /// Presentation Key for notification received on foreground.
        /// </summary>
        public static NSString ExtraNotificationReceivedIos => new("Plugin.LocalNotification.NOTIFICATION_RECEIVED");

        /// <summary>
        /// Key for extra playing sound in foreground.
        /// </summary>
        public static NSString ExtraSoundInForegroundIos => new("Plugin.LocalNotification.NOTIFICATION_SOUND_FOREGROUND");

        private static void Initialize()
        {
            try
            {
                UNUserNotificationCenter.Current.Delegate = new LocalNotificationDelegate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Ask the user for permission to show notifications on iOS 10.0+.
        /// Returns true if Allowed.
        /// </summary>
        public static async void AskPermission()
        {
            await AskPermissionAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Ask the user for permission to show notifications on iOS 10.0+.
        /// Returns true if Allowed.
        /// </summary>
        public static async Task<bool> AskPermissionAsync()
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return true;
                }

                var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync().ConfigureAwait(false);
                var allowed = settings.AlertSetting == UNNotificationSetting.Enabled;

                if (allowed)
                {
                    return true;
                }

                // Ask the user for permission to show notifications on iOS 10.0+
                var (alertsAllowed, error) = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(
                                                                               UNAuthorizationOptions.Alert |
                                                                               UNAuthorizationOptions.Badge |
                                                                               UNAuthorizationOptions.Sound)
                                                                           .ConfigureAwait(false);

                Debug.WriteLine(error?.LocalizedDescription);

                Initialize();

                return alertsAllowed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Reset Application Icon Badge Number when there are no notifications.
        /// </summary>
        /// <param name="uiApplication"></param>
        public static void ResetApplicationIconBadgeNumber(UIApplication uiApplication)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
            {
                return;
            }

            //Remove badges on app enter foreground if user cleared the notification in the notification panel
            UNUserNotificationCenter.Current.GetDeliveredNotifications((notificationList) =>
            {
                if (notificationList.Any())
                {
                    return;
                }

                uiApplication.InvokeOnMainThread(() =>
                {
                    uiApplication.ApplicationIconBadgeNumber = 0;
                });
            });
        }

        #region Implementation

        /// <inheritdoc />
        private static void OnPlatformNotificationTapped(NotificationTappedEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        /// <inheritdoc />
        private static void OnPlatformNotificationReceived(NotificationReceivedEventArgs e)
        {
            NotificationReceived?.Invoke(e);
        }

        /// <inheritdoc />
        private static void PlatformCancel(int notificationId)
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
        private static void PlatformCancelAll()
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
        private static void PlatformShow(Func<NotificationRequestBuilder, NotificationRequest> builder)
        {
            Show(builder.Invoke(new NotificationRequestBuilder()));
        }

        /// <inheritdoc />
        private static async void PlatformShow(NotificationRequest notificationRequest)
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

                var allowed = await AskPermissionAsync().ConfigureAwait(false);
                if (allowed == false)
                {
                    return;
                }

                var userInfoDictionary = new NSMutableDictionary();

                if (string.IsNullOrWhiteSpace(notificationRequest.ReturningData) == false)
                {
                    using var returningData = new NSString(notificationRequest.ReturningData);
                    userInfoDictionary.SetValueForKey(string.IsNullOrWhiteSpace(notificationRequest.ReturningData)
                        ? NSString.Empty : returningData, ExtraReturnDataIos);
                }

                using var receivedData = new NSString(notificationRequest.iOS.HideForegroundAlert.ToString());
                userInfoDictionary.SetValueForKey(receivedData, ExtraNotificationReceivedIos);

                using var soundData = new NSString(notificationRequest.iOS.PlayForegroundSound.ToString());
                userInfoDictionary.SetValueForKey(soundData, ExtraSoundInForegroundIos);

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

        #endregion
    }
}
