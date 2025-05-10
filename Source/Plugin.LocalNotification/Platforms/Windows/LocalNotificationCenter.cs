using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using Plugin.LocalNotification.EventArgs;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.UI.Notifications;

namespace Plugin.LocalNotification;

public partial class LocalNotificationCenter
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="permission"></param>
    /// <returns></returns>
    public static async Task<bool> RequestNotificationPermissionAsync(NotificationPermission? permission = null)
    {
        return await Task.FromResult(true);
    }

    /// <summary>
    /// Need to add this because otherwise setting background activation does nothing.
    /// </summary>
    public static void SetupBackgroundActivation()
    {
        ToastNotificationManagerCompat.OnActivated += (notificationArgs) =>
        {
            // this will run everytime ToastNotification.Activated is called,
            // regardless of what toast is clicked and what element is clicked on.
            // Works for all types of ToastActivationType so long as the Windows app manifest
            // has been updated to support ToastNotifications.

            // you can check your args here, however I'll be doing mine below to keep it cleaner.
            // With so many ToastNotifications it would be messy to check all of them here.

            Debug.WriteLine($"A ToastNotification was just activated! Arguments: {notificationArgs.Argument}");

            NotifyNotificationTapped(notificationArgs.Argument);
        };
    }

    /// <summary>
    /// Notify Local Notification Tapped.
    /// </summary>
    /// <param name="arguments"></param>
    internal static void NotifyNotificationTapped(string arguments)
    {
        try
        {
            var (actionId, request) = GetRequestFromArguments(arguments);
            if (actionId == -1000 || request is null)
            {
                return;
            }
            var actionArgs = new NotificationActionEventArgs
            {
                ActionId = actionId,
                Request = request
            };
            Current.OnNotificationActionTapped(actionArgs);
        }
        catch (Exception ex)
        {
            Log(ex);
        }
    }

    internal static (int, NotificationRequest?) GetRequestFromArguments(string arguments)
    {
        var args = ToastArguments.Parse(arguments);

        var actionId = args.GetInt(ReturnRequestActionId);
        var notifiactionId = args.Get(ReturnRequest);

        var toastNotification = ToastNotificationManager.History.GetHistory().FirstOrDefault(t => t.Tag == notifiactionId);

        var element = toastNotification?.Content.ChildNodes.FirstOrDefault(e => e.NodeName == "toast");
        var attribute = element?.Attributes.FirstOrDefault(a => a.NodeName == "launch");

        // TODO: get the request
        var request = GetRequest("");
        return (actionId, request);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    /// <param name="callerName"></param>
    internal static void Log(string? message, [CallerMemberName] string callerName = "")
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
    internal static void Log(Exception? ex, string? message = null, [CallerMemberName] string callerName = "")
    {
        var logMessage = $"{callerName}: {message}";
        Logger?.LogError(ex, logMessage);
    }
}