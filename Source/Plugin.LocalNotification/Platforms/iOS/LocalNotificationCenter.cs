using Foundation;
using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Platforms;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification;

public partial class LocalNotificationCenter
{
    /// <summary>
    /// Sets the <see cref="UNUserNotificationCenterDelegate"/> for IOS notifications. Allows developers to provide a custom delegate for handling notification events.
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

        if (!dictionary.ContainsKey(new NSString(RequestConstants.ReturnRequest)))
        {
            return null;
        }

        var requestSerialize = dictionary[RequestConstants.ReturnRequest].ToString();

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
                if (OperatingSystem.IsIOSVersionAtLeast(16))
                {
                    UNUserNotificationCenter.Current.SetBadgeCount(0, (error) =>
                    {
                        if (error != null)
                        {
                            LocalNotificationLogger.Log(error.LocalizedDescription);
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
            LocalNotificationLogger.Log(ex);
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
                if (OperatingSystem.IsIOSVersionAtLeast(16))
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
            LocalNotificationLogger.Log(ex);
            throw;
        }
    }    
}