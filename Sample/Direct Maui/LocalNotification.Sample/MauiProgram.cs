using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;

namespace LocalNotification.Sample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
            //.UseLocalNotification();
            .UseLocalNotification(config =>
            {
                config.AddCategory(new NotificationCategory(NotificationCategoryType.Status)
                {
                    ActionList = 
                        [
                            new(100)
                            {
                                Title = "Hello",
                                Android =
                                {
                                    LaunchAppWhenTapped = true,
                                    IconName =
                                    {
                                        ResourceName = "i2"
                                    }
                                },
                                IOS =
                                {
                                    Action = iOSActionType.Foreground
                                },
                                Windows =
                                {
                                    LaunchAppWhenTapped = true
                                }
                            },
                            new(101)
                            {
                                Title = "Close",
                                Android =
                                {
                                    LaunchAppWhenTapped = false,
                                    IconName =
                                    {
                                        ResourceName = "i3"
                                    }
                                },
                                IOS =
                                {
                                    Action = iOSActionType.Destructive
                                },
                                Windows =
                                {
                                    LaunchAppWhenTapped = false
                                }
                            }
                        ]
                })
                .AddAndroid(android =>
                {
                    android.AddChannel(new NotificationChannelRequest
                    {
                        Sound = "good_things_happen"
                    });
                })
                .AddiOS(iOS =>
                {
#if IOS
                    //iOS.SetCustomUserNotificationCenterDelegate(new CustomUserNotificationCenterDelegate());
#endif
                });
            });


#if DEBUG
        LocalNotificationCenter.LogLevel = LogLevel.Debug;
        //builder.Logging.AddDebug();
        builder.Logging.AddConsole();
#endif

        builder.Services.AddTransient<MainPage>();

        return builder.Build();
	}
}
