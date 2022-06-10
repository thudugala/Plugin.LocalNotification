using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using Plugin.LocalNotification.Json;

namespace LocalNotification.Sample;

public partial class App : Application
{
    private readonly NotificationSerializer _notificationSerializer;

    public App()
	{
		InitializeComponent();

        //MainPage = new AppShell();

        _notificationSerializer = new NotificationSerializer();

        MainPage = new NavigationPage(new MainPage());

        LocalNotificationCenter.NotificationLog += NotificationCenter_NotificationLog;
    }

    private void NotificationCenter_NotificationLog(NotificationLogArgs e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.Error);
    }
}
