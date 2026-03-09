using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Plugin.LocalNotification.Core;

/// <summary>
/// 
/// </summary>
public static class LocalNotificationLogger
{
    /// <summary>
    /// Gets or sets the internal logger for notification events.
    /// </summary>
    public static ILogger? Logger { get; set; }

    /// <summary>
    /// Gets or sets the log level for internal logging.
    /// </summary>
    public static LogLevel LogLevel { get; set; } = LogLevel.Trace;

    /// <summary>
    /// Logs a message to the Android log and the configured logger.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerName">The name of the calling method (automatically provided).</param>
    public static void Log(string message, [CallerMemberName] string callerName = "")
    {
        if (Logger?.IsEnabled(LogLevel) == true)
        {
            Logger?.Log(LogLevel, "{CallerName}: {Message}", callerName, message);
        }
    }

    /// <summary>
    /// Logs an exception and optional message to the Android log and the configured logger.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <param name="message">An optional message to include with the log.</param>
    /// <param name="callerName">The name of the calling method (automatically provided).</param>
    public static void Log(Exception ex, string? message = null, [CallerMemberName] string callerName = "")
    {
        if (Logger?.IsEnabled(LogLevel.Error) == true)
        {
            Logger?.LogError(ex, "{CallerName}: {Message}", callerName, message);
        }
    }
}
