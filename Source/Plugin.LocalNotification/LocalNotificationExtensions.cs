#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using System;
using System.Linq;
#endif

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public static class LocalNotificationExtensions
    {
#if NET6_0_OR_GREATER
        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        public static MauiAppBuilder UseLocalNotification(this MauiAppBuilder builder, Action<ILocalNotificationBuilder> configureDelegate = null)
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
                    android.OnCreate((activity, savedInstanceState) =>
                    {
                        LocalNotificationCenter.CreateNotificationChannelGroups(localNotificationBuilder.AndroidBuilder.GroupChannelRequestList);

                        LocalNotificationCenter.CreateNotificationChannels(localNotificationBuilder.AndroidBuilder.ChannelRequestList);

                        LocalNotificationCenter.NotifyNotificationTapped(activity.Intent);
                    })
                    .OnNewIntent((activity, intent) =>
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
#endif
            });

            return builder;
        }
#endif
    }
}