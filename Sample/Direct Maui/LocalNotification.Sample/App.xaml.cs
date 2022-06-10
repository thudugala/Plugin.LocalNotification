using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace LocalNotification.Sample;

public partial class App : Application
{
    public App()
	{
		InitializeComponent();

        //MainPage = new AppShell();

        MainPage = new NavigationPage(new MainPage());

        LocalNotificationCenter.NotificationLog += NotificationCenter_NotificationLog;
    }

    private void NotificationCenter_NotificationLog(NotificationLogArgs e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.Error);
    }
}
