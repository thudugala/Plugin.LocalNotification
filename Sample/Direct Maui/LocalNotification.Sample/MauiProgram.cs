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
                        new NotificationAction(100)
                        {
                            Title = "Hello",
                            Android =
                            {
                                IconName =
                                {
                                    ResourceName = "i2"
                                }
                            }
                        },
                        new NotificationAction(101)
                        {
                            Title = "Close",
                            Android =
                            {
                                IconName =
                                {
                                    ResourceName = "i3"
                                }
                            }
                        }
                    })
                });
                config.AddAndroid(android =>
				{
					android.AddChannel(new NotificationChannelRequest
					{
						Sound = "good_things_happen"
					});
				});
//				config.AddiOS(iOS =>
//				{
//#if IOS
//					iOS.SetCustomUserNotificationCenterDelegate(new CustomUserNotificationCenterDelegate());
//#endif
//				});

			});

        builder.Services.AddLogging(logging =>
        {
            logging.AddDebug();
            //logging.AddConsole();
        });

        return builder.Build();
	}
}
