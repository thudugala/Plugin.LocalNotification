#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using System;
using System.Linq;
#if WINDOWS
using Microsoft.Toolkit.Uwp.Notifications;
#endif
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
                        if (localNotificationBuilder.AndroidBuilder.ChannelRequestList.Any())
                        {
                            foreach (var channelRequest in localNotificationBuilder.AndroidBuilder.ChannelRequestList)
                            {
                                LocalNotificationCenter.CreateNotificationChannel(channelRequest);
                            }
                        }
                        if (localNotificationBuilder.AndroidBuilder.GroupChannelRequestList.Any())
                        {
                            foreach (var groupChannelReques in localNotificationBuilder.AndroidBuilder.GroupChannelRequestList)
                            {
                                LocalNotificationCenter.CreateNotificationChannelGroup(groupChannelReques);
                            }
                        }
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
#elif WINDOWS
                life.AddWindows(windows =>
                {
                    windows.OnActivated((window, args) =>
                    {
                        ToastNotificationManagerCompat.OnActivated += (notificationArgs) =>
                        {
                            // this will run everytime ToastNotification.Activated is called,
                            // regardless of what toast is clicked and what element is clicked on.
                            // Works for all types of ToastActivationType so long as the Windows app manifest
                            // has been updated to support ToastNotifications. 

                            LocalNotificationCenter.NotifyNotificationTapped(notificationArgs.Argument);
                        };
                    });
                });
#endif
            });

            return builder;
        }
#endif
            }
}