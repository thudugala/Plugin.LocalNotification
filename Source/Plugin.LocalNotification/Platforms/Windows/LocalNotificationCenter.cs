using Microsoft.Extensions.Logging;
using Microsoft.Windows.AppNotifications;
using Plugin.LocalNotification.EventArgs;
using System.Runtime.CompilerServices;

namespace Plugin.LocalNotification;

public partial class LocalNotificationCenter
{    
    /// <summary>
    /// Setup Windows background activation for notifications
    /// </summary>
    public static void SetupBackgroundActivation()
    {
        AppNotificationManager.Default.NotificationInvoked += (sender, args) =>
        {
            var arguments = string.Join(";", args.Arguments.Select(kv => $"{kv.Key}={kv.Value}"));
            NotifyNotificationTapped(arguments);
        };
    }

    /// <summary>
    /// Notify Local Notification Tapped.
    /// </summary>
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
            throw;
        }
    }

    internal static (int, NotificationRequest?) GetRequestFromArguments(string arguments)
    {
        try
        {
            var dict = arguments.Split(';')
                .Select(part => part.Split('='))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0], parts => parts[1]);

            if (dict.TryGetValue(ReturnRequestActionId, out var actionIdStr) &&
                dict.TryGetValue(ReturnRequest, out var requestIdStr))
            {
                if (int.TryParse(actionIdStr, out var actionId) &&
                    int.TryParse(requestIdStr, out var requestId))
                {
                    // Create a basic NotificationRequest with the ID
                    // In a real app, you might want to store more information about the notification
                    var request = new NotificationRequest
                    {
                        NotificationId = requestId
                    };

                    return (actionId, request);
                }
            }
        }
        catch (Exception ex)
        {
            Log(ex);
            throw;
        }

        return (-1000, null);
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