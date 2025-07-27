﻿using Android.App;
using Android.Content;
using Android.Media;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.EventArgs;
using Plugin.LocalNotification.Platforms;
using System.Runtime.CompilerServices;
using Application = Android.App.Application;

namespace Plugin.LocalNotification;

/// <summary>
/// Provides functionality for managing local notifications on Android.
/// </summary>
public partial class LocalNotificationCenter
{
    /// <summary>
    /// Notifies the notification service that a local notification was tapped by the user.
    /// </summary>
    /// <param name="intent">The intent containing notification action data.</param>
    public static void NotifyNotificationTapped(Intent? intent)
    {
        try
        {
            var actionId = intent?.GetIntExtra(ReturnRequestActionId, -1000);
            if (actionId is null or (-1000))
            {
                return;
            }

            var requestSerialize = intent?.GetStringExtra(ReturnRequest);
            var request = GetRequest(requestSerialize);

            var actionArgs = new NotificationActionEventArgs
            {
                ActionId = actionId.GetValueOrDefault(),
                Request = request
            };
            Current.OnNotificationActionTapped(actionArgs);
        }
        catch (Exception ex)
        {
            Log(ex);
        }
    }

    /// <summary>
    /// Create Notification Channel Group when API >= 26.
    /// If you'd like to further organize the appearance of your channels in the settings UI, you can create channel groups.
    /// This is a good idea when your app supports multiple user accounts (such as for work profiles),
    /// so you can create a notification channel group for each account.
    /// This way, users can easily identify and control multiple notification channels that have identical names.
    /// </summary>
    /// <param name="groupChannelRequests">A list of channel group requests to create.</param>
    public static void CreateNotificationChannelGroups(IList<NotificationChannelGroupRequest> groupChannelRequests)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            return;
        }

        if (!groupChannelRequests.Any())
        {
            return;
        }

        if (Application.Context.GetSystemService(Context.NotificationService) is not NotificationManager notificationManager)
        {
            return;
        }

#pragma warning disable CA1416 // Validate platform compatibility
        var channelGroups = groupChannelRequests.Select(gcr => new NotificationChannelGroup(gcr.Group, gcr.Name)).ToList();
#pragma warning restore CA1416 // Validate platform compatibility
        notificationManager.CreateNotificationChannelGroups(channelGroups);
    }

    /// <summary>
    /// Creates notification channels for Android API >= 26.
    /// Channels define notification behaviors such as sound, vibration, and importance.
    /// </summary>
    /// <param name="channelRequests">A list of channel requests to create.</param>
    public static void CreateNotificationChannels(IList<NotificationChannelRequest> channelRequests)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            return;
        }

        if (channelRequests.Any())
        {
            channelRequests.Add(new NotificationChannelRequest());
        }

        if (Application.Context.GetSystemService(Context.NotificationService) is not NotificationManager notificationManager)
        {
            return;
        }

        // you can't change the importance or other notification behaviors after this.
        // once you create the channel, you cannot change these settings and
        // the user has final control of whether these behaviors are active.
        var channels = new List<NotificationChannel>();

        foreach (var channelRequest in channelRequests)
        {
            var channel = new NotificationChannel(channelRequest.Id, channelRequest.Name, channelRequest.Importance.ToNative())
            {
                Description = channelRequest.Description,
                Group = string.IsNullOrWhiteSpace(channelRequest.Group) ? null : channelRequest.Group,
                LightColor = channelRequest.LightColor.ToNative(),
                LockscreenVisibility = channelRequest.LockScreenVisibility.ToNative(),
            };

            if (channelRequest.EnableSound)
            {
                var soundUri = GetSoundUri(channelRequest.Sound);
                if (soundUri != null)
                {
                    using var audioAttributesBuilder = new AudioAttributes.Builder();
                    var audioAttributes = audioAttributesBuilder.SetUsage(AudioUsageKind.Notification)
                            ?.SetContentType(AudioContentType.Sonification)
                            ?.Build();
                    channel.SetSound(soundUri, audioAttributes);
                }
            }

            if (channelRequest.VibrationPattern != null &&
                channelRequest.VibrationPattern.Length != 0)
            {
                channel.SetVibrationPattern(channelRequest.VibrationPattern);
                _ = channel.ShouldVibrate();
            }

            channel.SetShowBadge(channelRequest.ShowBadge);
            channel.EnableLights(channelRequest.EnableLights);
            channel.EnableVibration(channelRequest.EnableVibration);
            channel.SetBypassDnd(channelRequest.CanBypassDnd);

            channels.Add(channel);
        }

        notificationManager.CreateNotificationChannels(channels);
    }

    /// <summary>
    /// Gets the Android URI for a notification sound file.
    /// </summary>
    /// <param name="soundFileName">The name of the sound file.</param>
    /// <returns>The URI for the sound file, or the default notification sound if not found.</returns>
    internal static Android.Net.Uri? GetSoundUri(string soundFileName)
    {
        if (string.IsNullOrWhiteSpace(soundFileName))
        {
            return RingtoneManager.GetDefaultUri(RingtoneType.Notification);
        }
        if (soundFileName.Contains("://", StringComparison.InvariantCulture))
        {
            return Android.Net.Uri.Parse(soundFileName);
        }

        var soundNameWithoutExtension = Path.GetFileNameWithoutExtension(soundFileName);
        var soundFilePath = $"{ContentResolver.SchemeAndroidResource}://{Application.Context.PackageName}/raw/{soundNameWithoutExtension}";
        var soundFileUri = Android.Net.Uri.Parse(soundFilePath);

#if DEBUG
        if (soundFileUri is null || !soundFileUri.IsValidResource(Application.Context))
        {
            throw new ArgumentException($"Invalid sound file: {soundFileName}. Your sound has to be AndroidResource stored in Platforms/Android/Resources/raw");
        }
#endif

        return soundFileUri;
    }

    /// <summary>
    /// Logs a message to the Android log and the configured logger.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerName">The name of the calling method (automatically provided).</param>
    internal static void Log(string message, [CallerMemberName] string callerName = "")
    {
        var logMessage = $"{callerName}: {message}";
        Logger?.Log(LogLevel, logMessage);
        if (LogLevel == LogLevel.Trace)
        {
            _ = Android.Util.Log.Debug(Application.Context.PackageName, logMessage);
        }
        if (LogLevel == LogLevel.Information)
        {
            _ = Android.Util.Log.Info(Application.Context.PackageName, logMessage);
        }
        if (LogLevel == LogLevel.Warning)
        {
            _ = Android.Util.Log.Warn(Application.Context.PackageName, logMessage);
        }
    }

    /// <summary>
    /// Logs an exception and optional message to the Android log and the configured logger.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    /// <param name="message">An optional message to include with the log.</param>
    /// <param name="callerName">The name of the calling method (automatically provided).</param>
    internal static void Log(Exception ex, string? message = null, [CallerMemberName] string callerName = "")
    {
        var logMessage = $"{callerName}: {message}";
        Logger?.LogError(ex, logMessage);
        _ = Android.Util.Log.Error(Application.Context.PackageName, $"{logMessage}: {ex.Message}");
    }
}