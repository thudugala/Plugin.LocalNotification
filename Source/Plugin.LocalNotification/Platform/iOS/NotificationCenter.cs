using Plugin.LocalNotification.Json;
using Plugin.LocalNotification.Platform.iOS;
using System;
using System.Linq;
using System.Threading.Tasks;
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

                UNUserNotificationCenter.Current.Delegate = new LocalNotificationDelegate();
            }
            catch (Exception ex)
            {
                Log(ex);
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

            Log(error?.LocalizedDescription);
            return alertsAllowed;
        }

        /// <summary>
        /// Reset Application Icon Badge Number when there are no notifications.
        /// </summary>
        /// <param name="uiApplication"></param>
        public static async Task ResetApplicationIconBadgeNumber(UIApplication uiApplication)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
            {
                return;
            }

            //Remove badges on app enter foreground if user cleared the notification in the notification panel
            var notificationList = await UNUserNotificationCenter.Current.GetDeliveredNotificationsAsync().ConfigureAwait(false);

            if (notificationList.Any())
            {
                return;
            }
            uiApplication.InvokeOnMainThread(() =>
            {
                uiApplication.ApplicationIconBadgeNumber = 0;
            });
        }

        internal static void Log(string message)
        {
            Console.WriteLine(message);
            NotificationLog?.Invoke(new NotificationLogArgs
            {
                Message = message
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