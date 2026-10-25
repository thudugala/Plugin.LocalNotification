using Android.App;
using Android.Content;
using Android.Media;
using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Plugin.LocalNotification.Core.Platforms.Android;
using Plugin.LocalNotification.EventArgs;
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
            var actionId = intent?.GetIntExtra(RequestConstants.ReturnRequestActionId, -1000);
            if (actionId is null or -1000)
            {
                return;
            }

            var requestSerialize = intent?.GetStringExtra(RequestConstants.ReturnRequest);
            var request = GetRequest(requestSerialize);

            var actionArgs = new NotificationActionEventArgs
            {
                ActionId = actionId.GetValueOrDefault(),
                Request = request
            };

            // Extract inline-reply text from a RemoteInput result (Android direct-reply actions).
            var remoteInputBundle = AndroidX.Core.App.RemoteInput.GetResultsFromIntent(intent);
            if (remoteInputBundle != null)
            {
                actionArgs.Input = remoteInputBundle.GetCharSequence(RequestConstants.RemoteInputKey)?.ToString();
            }

            Current.OnNotificationActionTapped(actionArgs);
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
        }
    }

    /// <summary>
    /// Inspects the activity's launch intent and, if it contains notification data from this
    /// plugin, stores the result in <see cref="LaunchNotificationDetails"/> so that callers
    /// can determine whether the app was cold-started by a notification tap.
    /// Call this once from the <c>OnCreate</c> lifecycle handler, before
    /// <see cref="NotifyNotificationTapped"/>.
    /// </summary>
    /// <param name="intent">The activity's launch intent.</param>
    public static void SetLaunchNotificationFromIntent(Intent? intent)
    {
        try
        {
            var actionId = intent?.GetIntExtra(RequestConstants.ReturnRequestActionId, -1000);
            if (actionId is null or -1000)
            {
                LaunchNotificationDetails = new NotificationLaunchDetails
                {
                    DidNotificationLaunchApp = false
                };
                return;
            }

            var requestSerialize = intent?.GetStringExtra(RequestConstants.ReturnRequest);
            var request = GetRequest(requestSerialize);

            LaunchNotificationDetails = new NotificationLaunchDetails
            {
                DidNotificationLaunchApp = true,
                Request = request,
                ActionId = actionId.GetValueOrDefault()
            };
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
            LaunchNotificationDetails = new NotificationLaunchDetails
            {
                DidNotificationLaunchApp = false
            };
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
    public static void CreateNotificationChannelGroups(IList<AndroidNotificationChannelGroupRequest> groupChannelRequests)
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
    public static void CreateNotificationChannels(IList<AndroidNotificationChannelRequest> channelRequests)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            return;
        }

        if (channelRequests.Any())
        {
            channelRequests.Add(new AndroidNotificationChannelRequest());
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
        if (soundFileUri is null || !IsValidResource(soundFileUri, Application.Context))
        {
            throw new ArgumentException($"Invalid sound file: {soundFileUri}. Your sound has to be AndroidResource stored in Platforms/Android/Resources/raw");
        }
#endif

        return soundFileUri;
    }

    private static bool IsValidResource(Android.Net.Uri uri, Context context)
    {
        var contentResolver = context.ContentResolver;
        if (contentResolver is null)
        {
            return false;
        }

        try
        {
            contentResolver.OpenInputStream(uri)?.Close();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}