using Foundation;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification.Platforms;
using System.Runtime.CompilerServices;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification;

public partial class LocalNotificationCenter
{
    /// <summary>
    /// This allow developer to change UNUserNotificationCenterDelegate,
    /// extend Plugin.LocalNotification.Platform.iOS.UserNotificationCenterDelegate
    /// Create custom IUNUserNotificationCenterDelegate
    /// and set it using this method
    /// </summary>
    /// <param name="notificationDelegate"></param>
    public static void SetUserNotificationCenterDelegate(UserNotificationCenterDelegate? notificationDelegate = null) => UNUserNotificationCenter.Current.Delegate = notificationDelegate ?? new UserNotificationCenterDelegate();

    /// <summary>
    ///
    /// </summary>
    /// <param name="notificationContent"></param>
    /// <returns></returns>
    public static NotificationRequest? GetRequest(UNNotificationContent? notificationContent)
    {
        if (notificationContent is null)
        {
            return null;
        }

        var dictionary = notificationContent.UserInfo;

        if (!dictionary.ContainsKey(new NSString(ReturnRequest)))
        {
            return null;
        }

        var requestSerialize = dictionary[ReturnRequest].ToString();

        var request = GetRequest(requestSerialize);

        return request;
    }

    /// <summary>
    /// Reset Application Icon Badge Number when there are no notifications.
    /// </summary>
    /// <param name="uiApplication"></param>
    public static void ResetApplicationIconBadgeNumber(UIApplication uiApplication)
    {
        try
        {
            var notificationList = new List<UNNotification>();
            //Remove badges on app enter foreground if user cleared the notification in the notification panel
            var completionSource = new TaskCompletionSource<bool>();
            UNUserNotificationCenter.Current.GetDeliveredNotifications((notificationArray) =>
            {
                notificationList.AddRange(notificationArray);
                completionSource.SetResult(true);
            });
            completionSource.Task.Wait();
            if (notificationList.Count != 0)
            {
                return;
            }

            uiApplication.InvokeOnMainThread(() =>
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(16, 0))
                {
                    UNUserNotificationCenter.Current.SetBadgeCount(0, (error) =>
                    {
                        if (error != null)
                        {
                            Log(error.LocalizedDescription);
                        }
                    });
                }
                else
                {
                    uiApplication.ApplicationIconBadgeNumber = 0;
                    UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                }
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
            //Remove badges on app enter foreground if user cleared the notification in the notification panel
            var notificationList = await UNUserNotificationCenter.Current.GetDeliveredNotificationsAsync()
                .ConfigureAwait(false);

            if (notificationList.Length != 0)
            {
                return;
            }

            uiApplication.InvokeOnMainThread(async () =>
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(16, 0))
                {
                    await UNUserNotificationCenter.Current.SetBadgeCountAsync(0);
                }
                else
                {
                    uiApplication.ApplicationIconBadgeNumber = 0;
                    UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                }
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
    internal static void Log(string? message, [CallerMemberName] string callerName = "")
    {
        Logger?.Log(LogLevel, "{callerName}: {message}", callerName, message);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="message"></param>
    /// <param name="callerName"></param>
    internal static void Log(Exception? ex, string? message = null, [CallerMemberName] string callerName = "")
    {
        Logger?.LogError(ex, "{callerName}: {message}", callerName, message);
    }
}