using CoreLocation;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification.iOSOption;
using Plugin.LocalNotification.Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification
{
    public partial class LocalNotificationCenter
    {
        /// <summary>
        /// This allow developer to change UNUserNotificationCenterDelegate,
        /// extend Plugin.LocalNotification.Platform.iOS.UserNotificationCenterDelegate
        /// Create custom IUNUserNotificationCenterDelegate
        /// and set it using this method
        /// </summary>
        /// <param name="notificationDelegate"></param>
        public static void SetUserNotificationCenterDelegate(UserNotificationCenterDelegate notificationDelegate = null)
        {
            UNUserNotificationCenter.Current.Delegate = notificationDelegate ?? new UserNotificationCenterDelegate();
        }

        /// <summary>
        /// Ask the user for permission to show notifications on iOS 10.0+.
        /// Returns true if Allowed.
        /// If not asked at startup, user will be asked when showing the first notification.
        /// </summary>
        public static bool RequestNotificationPermission(NotificationPermission permission = null)
        {
            try
            {
                permission ??= new NotificationPermission();

                if (!permission.AskPermission)
                {
                    return false;
                }

                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return true;
                }

                var allowed = AreNotificationsEnabled();
                if (allowed)
                {
                    return true;
                }

                var alertsAllowed = false;
                var authorizationOptions = permission.IOS.NotificationAuthorization.ToNative();
                // Ask the user for permission to show notifications on iOS 10.0+
                UNUserNotificationCenter.Current.RequestAuthorization(
                    authorizationOptions,
                    (approved, error) =>
                    {
                        if (error != null)
                        {
                            Log(error?.LocalizedDescription);
                        }
                        else
                        {
                            alertsAllowed = approved;
                            if (alertsAllowed)
                            {
                                RequestLocationPermission(permission.IOS.LocationAuthorization);
                            }
                        }
                    });
                return alertsAllowed;
            }
            catch (Exception ex)
            {
                Log(ex);
                return false;
            }
        }

        /// <summary>
        /// Ask the user for permission to show notifications on iOS 10.0+.
        /// Returns true if Allowed.
        /// If not asked at startup, user will be asked when showing the first notification.
        /// </summary>
        public static async Task<bool> RequestNotificationPermissionAsync(NotificationPermission permission = null)
        {
            try
            {
                permission ??= new NotificationPermission();

                if (!permission.AskPermission)
                {
                    return false;
                }

                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return true;
                }

                var allowed = await AreNotificationsEnabledAsync();
                if (allowed)
                {
                    return true;
                }

                // Ask the user for permission to show notifications on iOS 10.0+
                var authorizationOptions = permission.IOS.NotificationAuthorization.ToNative();
                var (alertsAllowed, error) = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(authorizationOptions).ConfigureAwait(false);

                Log(error?.LocalizedDescription);

                if (alertsAllowed)
                {
                    RequestLocationPermission(permission.IOS.LocationAuthorization);
                }

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
        public static void RequestLocationPermission(iOSLocationAuthorization authorization)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return;
                }
                if (authorization == iOSLocationAuthorization.No)
                {
                    return;
                }

                var locationManager = new CLLocationManager();

                if (authorization == iOSLocationAuthorization.Always)
                {
                    locationManager.RequestAlwaysAuthorization();
                }
                else if (authorization == iOSLocationAuthorization.WhenInUse)
                {
                    locationManager.RequestWhenInUseAuthorization();
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        internal static bool AreNotificationsEnabled()
        {
            var isEnabled = false;
            var completionSource = new TaskCompletionSource<bool>();
            UNUserNotificationCenter.Current.GetNotificationSettings((settings) =>
            {
                isEnabled = settings.AlertSetting == UNNotificationSetting.Enabled;
                completionSource.SetResult(true);
            });
            completionSource.Task.Wait();
            return isEnabled;
        }

        internal static async Task<bool> AreNotificationsEnabledAsync()
        {
            var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync().ConfigureAwait(false);
            return settings.AlertSetting == UNNotificationSetting.Enabled;
        }

        /// <summary>
        /// Reset Application Icon Badge Number when there are no notifications.
        /// </summary>
        /// <param name="uiApplication"></param>
        public static void ResetApplicationIconBadgeNumber(UIApplication uiApplication)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return;
                }

                var notificationList = new List<UNNotification>();
                //Remove badges on app enter foreground if user cleared the notification in the notification panel
                var completionSource = new TaskCompletionSource<bool>();
                UNUserNotificationCenter.Current.GetDeliveredNotifications((notificationArray) =>
                {
                    notificationList.AddRange(notificationArray);
                    completionSource.SetResult(true);
                });
                completionSource.Task.Wait();
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

        /// <summary>
        /// Reset Application Icon Badge Number when there are no notifications.
        /// </summary>
        /// <param name="uiApplication"></param>
        public static async Task ResetApplicationIconBadgeNumberAsync(UIApplication uiApplication)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callerName"></param>
        internal static void Log(string message, [CallerMemberName] string callerName = "")
        {
            var logMessage = $"{callerName}: {message}";
            Logger?.Log(LogLevel, logMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="callerName"></param>
        internal static void Log(Exception ex, string message = null, [CallerMemberName] string callerName = "")
        {
            var logMessage = $"{callerName}: {message}";
            Logger?.LogError(ex, logMessage);
        }
    }
}