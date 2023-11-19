using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.LifecycleEvents;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public static class LocalNotificationExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        public static MauiAppBuilder UseLocalNotification(this MauiAppBuilder builder, Action<ILocalNotificationBuilder>? configureDelegate = null)
        {
            var localNotificationBuilder = new LocalNotificationBuilder();
            configureDelegate?.Invoke(localNotificationBuilder);

            builder.Services.AddSingleton(localNotificationBuilder);
            builder.Services.AddSingleton(LocalNotificationCenter.Current);
            builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, LocalNotificationInitializeService>());

            builder.ConfigureLifecycleEvents(life =>
            {
#if ANDROID
                life.AddAndroid(android =>
                {
                    android.OnCreate((activity, _) =>
                    {                        
                        LocalNotificationCenter.CreateNotificationChannels(localNotificationBuilder.AndroidBuilder.ChannelRequestList);

                        LocalNotificationCenter.CreateNotificationChannelGroups(localNotificationBuilder.AndroidBuilder.GroupChannelRequestList);

                        LocalNotificationCenter.NotifyNotificationTapped(activity.Intent);
                    })
                    .OnNewIntent((_, intent) =>
                    {
                        LocalNotificationCenter.NotifyNotificationTapped(intent);
                    });
                });
#elif IOS
                life.AddiOS(iOS =>
                {
                    iOS.FinishedLaunching((application, _) =>
                    {
                        LocalNotificationCenter.SetUserNotificationCenterDelegate(localNotificationBuilder.IOSBuilder.CustomUserNotificationCenterDelegate);
                        return true;
                    });
                    iOS.WillEnterForeground(application =>
                    {
                        LocalNotificationCenter.ResetApplicationIconBadgeNumber(application);
                    });
                });
#elif WINDOWS
                life.AddWindows(windows =>
                {
                    windows.OnActivated((window, args) =>
                    {
                        LocalNotificationCenter.SetupBackgroundActivation();
                    });
                });
#endif
            });

            return builder;
        }
    }
}