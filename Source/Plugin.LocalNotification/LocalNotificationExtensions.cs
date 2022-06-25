﻿#if ANDROID || IOS
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using System;
#endif

namespace Plugin.LocalNotification
{
    /// <summary>
    /// 
    /// </summary> 
    public static class LocalNotificationExtensions
    {
#if ANDROID || IOS
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

            builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, LocalNotificationInitializeService>());

            builder.ConfigureLifecycleEvents(life =>
            {
#if ANDROID
                life.AddAndroid(android =>
                {
                    android.OnCreate((activity, savedInstanceState) =>
                    {
                        if (localNotificationBuilder.AndroidBuilder.ChannelRequestList.Count > 0)
                        {
                            foreach(var channelRequest in localNotificationBuilder.AndroidBuilder.ChannelRequestList)
                            {
                                LocalNotificationCenter.CreateNotificationChannel(channelRequest);
                            }
                        }
                        if (localNotificationBuilder.AndroidBuilder.GroupChannelRequestList.Count > 0)
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
                        if (localNotificationBuilder.IOSBuilder.Permission.SetUserNotificationCenterDelegate)
                        {
                            LocalNotificationCenter.SetCustomUserNotificationCenterDelegate(localNotificationBuilder.IOSBuilder.CustomUserNotificationCenterDelegate);
                        }
                        LocalNotificationCenter.RequestNotificationPermissionAsync(localNotificationBuilder.IOSBuilder.Permission).GetAwaiter().GetResult();
                        return true;
                    });
                    iOS.WillEnterForeground(application =>
                    {
                        LocalNotificationCenter.ResetApplicationIconBadgeNumber(application).GetAwaiter().GetResult();
                    });
                });
#endif
            });

            return builder;
        }
#endif
    }
}