using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Extensions.Logging;
#if ANDROID
using Microsoft.Maui.ApplicationModel;
#endif
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.EventArgs;
using Plugin.LocalNotification.Platforms;
using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Plugin.LocalNotification
{
    public partial class LocalNotificationCenter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RequestNotificationPermissionAsync(NotificationPermission permission = null)
        {
            permission ??= new NotificationPermission();
#if ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                return false;
            }

            if (!permission.AskPermission)
            {
                return false;
            }

            var status = await Permissions.RequestAsync<NotificationPerms>();
            return status == PermissionStatus.Granted;
#else
            if (!permission.AskPermission)
            {
                return await Task.FromResult(false);
            }

            if ((int)Build.VERSION.SdkInt >= 33)
            {
                var permissionName = "android.permission.POST_NOTIFICATIONS";

                if (Application.Context.CheckSelfPermission(permissionName) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(MainActivity, new[] { permissionName }, 0);
                }
            }
            return await Task.FromResult(true);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public static Android.App.Activity MainActivity { get; set; }

        /// <summary>
        /// Notify Local Notification Tapped.
        /// </summary>
        /// <param name="intent"></param>
        public static void NotifyNotificationTapped(Intent intent)
        {
            try
            {
                var actionId = intent?.GetIntExtra(ReturnRequestActionId, -1000);
                if (actionId is null || actionId == -1000)
                {
                    return;
                }

                var requestSerialize = intent.GetStringExtra(ReturnRequest);
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
        /// <param name="groupChannelRequests"></param>
        public static void CreateNotificationChannelGroups(IList<NotificationChannelGroupRequest> groupChannelRequests)
        {
#if MONOANDROID
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }
#elif ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                return;
            }
#endif
            if (!groupChannelRequests.Any())
            {
                return;
            }

            if (!(Application.Context.GetSystemService(Context.NotificationService) is NotificationManager
            notificationManager))
            {
                return;
            }

#pragma warning disable CA1416 // Validate platform compatibility
            var channelGroups = groupChannelRequests.Select(gcr => new NotificationChannelGroup(gcr.Group, gcr.Name)).ToList();
#pragma warning restore CA1416 // Validate platform compatibility
            notificationManager.CreateNotificationChannelGroups(channelGroups);
        }

        /// <summary>
        /// Create Notification Channel when API >= 26.
        /// </summary>
        /// <param name="channelRequests"></param>
        public static void CreateNotificationChannels(IList<NotificationChannelRequest> channelRequests)
        {
#if MONOANDROID
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }
#elif ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                return;
            }
#endif
            if (channelRequests.Any())
            {
                channelRequests.Add(new NotificationChannelRequest());
            }

            if (!(Application.Context.GetSystemService(Context.NotificationService) is NotificationManager notificationManager))
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
                    channel.ShouldVibrate();
                }

                channel.SetShowBadge(channelRequest.ShowBadge);
                channel.EnableLights(channelRequest.EnableLights);
                channel.EnableVibration(channelRequest.EnableVibration);
                channel.SetBypassDnd(channelRequest.CanBypassDnd);

                channels.Add(channel);
            }

            notificationManager.CreateNotificationChannels(channels);
        }

        internal static Android.Net.Uri GetSoundUri(string soundFileName)
        {
            if (string.IsNullOrWhiteSpace(soundFileName))
            {
                return RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            }
            if (soundFileName.Contains("://", StringComparison.InvariantCulture))
            {
                return Android.Net.Uri.Parse(soundFileName);
            }

            soundFileName = Path.GetFileNameWithoutExtension(soundFileName);
            soundFileName =
                $"{ContentResolver.SchemeAndroidResource}://{Application.Context.PackageName}/raw/{soundFileName}";

            return Android.Net.Uri.Parse(soundFileName);
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
            if (LogLevel == LogLevel.Trace)
            {
                Android.Util.Log.Debug(Application.Context.PackageName, logMessage);
            }
            if (LogLevel == LogLevel.Information)
            {
                Android.Util.Log.Info(Application.Context.PackageName, logMessage);
            }
            if (LogLevel == LogLevel.Warning)
            {
                Android.Util.Log.Warn(Application.Context.PackageName, logMessage);
            }
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
            Android.Util.Log.Error(Application.Context.PackageName, $"{logMessage}: {ex.Message}");
        }
    }
}