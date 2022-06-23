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
				config.AddAndroidChannel(new NotificationChannelRequest
				{
					Sound = "good_things_happen"
				});
			});

		return builder.Build();
	}
}
