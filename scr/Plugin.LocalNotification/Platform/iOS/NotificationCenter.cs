using System;
using System.Diagnostics;
using System.Linq;
using Foundation;
using Plugin.LocalNotification.Platform.iOS;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification
{
    public static partial class NotificationCenter
    {
        /// <summary>
        /// Return Data Key.
        /// </summary>
        internal static NSString ExtraReturnDataIos = new NSString("Plugin.LocalNotification.RETURN_DATA");

        static NotificationCenter()
        {
            try
            {
                Current = new Platform.iOS.NotificationServiceImpl();

                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return;
                }

                var alertsAllowed = false;

                UNUserNotificationCenter.Current.GetNotificationSettings((settings) =>
                {
                    alertsAllowed = settings.AlertSetting == UNNotificationSetting.Enabled;
                });

                if (!alertsAllowed)
                {
                    // Ask the user for permission to get notifications on iOS 10.0+
                    UNUserNotificationCenter.Current.RequestAuthorizationAsync(
                        UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound);
                }

                UNUserNotificationCenter.Current.Delegate = new LocalNotificationDelegate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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
