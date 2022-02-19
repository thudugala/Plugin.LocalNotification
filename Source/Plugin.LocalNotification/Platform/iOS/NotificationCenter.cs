using Plugin.LocalNotification.Json;
using Plugin.LocalNotification.Platform.iOS;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Plugin.LocalNotification.EventArgs;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification
{
    public static partial class NotificationCenter
    {
        static NotificationCenter()
        {
            try
            {
                Current = new Platform.iOS.NotificationServiceImpl();
                Serializer = new NotificationSerializer();

                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        /// <summary>
        /// Ask the user for permission to show notifications on iOS 10.0+.
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
                
                var allowed = await AreNotificationsEnabled();
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

                Log(error?.LocalizedDescription);
                return alertsAllowed;
            }
            catch (Exception ex)
            {
                Log(ex);
                return false;
            }
        }

        internal static async Task<bool> AreNotificationsEnabled()
        {
            var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync().ConfigureAwait(false);
            return settings.AlertSetting == UNNotificationSetting.Enabled;
        }

        /// <summary>
        /// Reset Application Icon Badge Number when there are no notifications.
        /// </summary>
        /// <param name="uiApplication"></param>
        public static async Task ResetApplicationIconBadgeNumber(UIApplication uiApplication)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return;
                }

                //Remove badges on app enter foreground if user cleared the notification in the notification panel
                var notificationList = await UNUserNotificationCenter.Current.GetDeliveredNotificationsAsync()
                    .ConfigureAwait(false);

                if (notificationList.Any())
                {
                    return;
                }

                uiApplication.InvokeOnMainThread(() =>
                {
                    uiApplication.ApplicationIconBadgeNumber = 0;

                });
                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                });
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        internal static void Log(string message, [CallerMemberName] string callerName = "")
        {
            Console.WriteLine($"{callerName}: {message}");
            NotificationLog?.Invoke(new NotificationLogArgs
            {
                Message = $"{callerName}: {message}"
            });
        }

        internal static void Log(Exception ex)
        {
            Console.WriteLine(ex);
            NotificationLog?.Invoke(new NotificationLogArgs
            {
                Error = ex
            });
        }
    }
}