using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

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
                    ActionList = new HashSet<NotificationAction>(new List<NotificationAction>()
                        {
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
                                    Action = Plugin.LocalNotification.iOSOption.iOSActionType.Foreground
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
                                    Action = Plugin.LocalNotification.iOSOption.iOSActionType.Destructive
                                },
                                Windows =
                                {
                                    LaunchAppWhenTapped = false
                                }
                            }
                        })
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
        builder.Logging.AddDebug();
#endif

        builder.Services.AddTransient<MainPage>();

        return builder.Build();
	}
}
