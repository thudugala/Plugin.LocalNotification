using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Plugin.LocalNotification.EventArgs;
using UIKit;
using UserNotifications;
using Plugin.LocalNotification.Platforms;
using CoreLocation;

namespace Plugin.LocalNotification
{
    public partial class LocalNotificationCenter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationDelegate">This allow developer to change UNUserNotificationCenterDelegate</param>
        /// <param name="askNotificationPermission">Ask the user for notification permission to show notifications on iOS 10.0+</param>
        /// <param name="askLocationPermission">Ask the user for location permission to show notifications on iOS 10.0+</param>
        public static void Setup(IUNUserNotificationCenterDelegate notificationDelegate = null, bool askNotificationPermission = true, LocationAuthorization askLocationPermission = LocationAuthorization.No)
        {
            SetCustomUserNotificationCenterDelegate(notificationDelegate);
            if (askNotificationPermission)
            {
                RequestNotificationPermission();
                RequestLocationPermission(askLocationPermission);
            }
        }

        /// <summary>
        /// This allow developer to change UNUserNotificationCenterDelegate,
        /// extend Plugin.LocalNotification.Platform.iOS.UserNotificationCenterDelegate
        /// Create custom IUNUserNotificationCenterDelegate
        /// and set it using this method
        /// </summary>
        /// <param name="notificationDelegate"></param>
        public static void SetCustomUserNotificationCenterDelegate(IUNUserNotificationCenterDelegate notificationDelegate = null)
        {
            UNUserNotificationCenter.Current.Delegate = notificationDelegate ?? new UserNotificationCenterDelegate();
        }

        /// <summary>
        /// Ask the user for permission to show notifications on iOS 10.0+.
        /// </summary>
        public static async void RequestNotificationPermission(UNAuthorizationOptions options =
            UNAuthorizationOptions.Alert |
            UNAuthorizationOptions.Badge |
            UNAuthorizationOptions.Sound)
        {
            await RequestNotificationPermissionAsync(options).ConfigureAwait(false);
        }

        /// <summary>
        /// Ask the user for permission to show notifications on iOS 10.0+.
        /// Returns true if Allowed.
        /// </summary>
        public static async Task<bool> RequestNotificationPermissionAsync(UNAuthorizationOptions options =
            UNAuthorizationOptions.Alert |
            UNAuthorizationOptions.Badge |
            UNAuthorizationOptions.Sound)
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
                var (alertsAllowed, error) = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(options).ConfigureAwait(false);

                Log(error?.LocalizedDescription);
                return alertsAllowed;
            }
            catch (Exception ex)
            {
                Log(ex);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public static void RequestLocationPermission(LocationAuthorization authorization = LocationAuthorization.No)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return;
                }
                if(authorization == LocationAuthorization.No)
                {
                    return;
                }

                var locationManager = new CLLocationManager();

                if (authorization == LocationAuthorization.Always)
                {
                    locationManager.RequestAlwaysAuthorization();
                }
                else if (authorization == LocationAuthorization.WhenInUse)
                {
                    locationManager.RequestWhenInUseAuthorization();
                }
            }
            catch (Exception ex)
            {
                Log(ex);
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