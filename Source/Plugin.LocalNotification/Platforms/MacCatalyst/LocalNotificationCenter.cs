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
    /// Sets the <see cref="UNUserNotificationCenterDelegate"/> for iOS notifications. Allows developers to provide a custom delegate for handling notification events.
    /// </summary>
    /// <param name="notificationDelegate">The custom notification center delegate to use. If null, uses the default <see cref="UserNotificationCenterDelegate"/>.</param>
    public static void SetUserNotificationCenterDelegate(UserNotificationCenterDelegate? notificationDelegate = null) => UNUserNotificationCenter.Current.Delegate = notificationDelegate ?? new UserNotificationCenterDelegate();

    /// <summary>
    /// Gets the <see cref="NotificationRequest"/> from the provided <see cref="UNNotificationContent"/>.
    /// </summary>
    /// <param name="notificationContent">The notification content to extract the request from.</param>
    /// <returns>The deserialized <see cref="NotificationRequest"/>, or null if not found.</returns>
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
    /// Resets the application icon badge number when there are no notifications.
    /// </summary>
    /// <param name="uiApplication">The current <see cref="UIApplication"/> instance.</param>
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
                if (OperatingSystem.IsMacCatalystVersionAtLeast(16))
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
    /// Asynchronously resets the application icon badge number when there are no notifications.
    /// </summary>
    /// <param name="uiApplication">The current <see cref="UIApplication"/> instance.</param>
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
                if (OperatingSystem.IsMacCatalystVersionAtLeast(16))
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
    /// Logs a message to the internal logger with the caller name.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerName">The name of the calling method (automatically provided).</param>
    internal static void Log(string? message, [CallerMemberName] string callerName = "")
    {
        Logger?.Log(LogLevel, "{callerName}: {message}", callerName, message);
    }

    /// <summary>
    /// Logs an exception and optional message to the internal logger with the caller name.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <param name="message">An optional message to include with the log.</param>
    /// <param name="callerName">The name of the calling method (automatically provided).</param>
    internal static void Log(Exception? ex, string? message = null, [CallerMemberName] string callerName = "")
    {
        Logger?.LogError(ex, "{callerName}: {message}", callerName, message);
    }
}