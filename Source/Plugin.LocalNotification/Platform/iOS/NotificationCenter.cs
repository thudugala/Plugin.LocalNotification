using Foundation;
using Plugin.LocalNotification.Platform.iOS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    }
}