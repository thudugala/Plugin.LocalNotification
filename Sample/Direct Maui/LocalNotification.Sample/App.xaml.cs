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

        NotificationCenter.NotificationLog += NotificationCenter_NotificationLog;
        NotificationCenter.Current.NotificationTapped += LoadPageFromNotification;
    }

    private void NotificationCenter_NotificationLog(NotificationLogArgs e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.Error);
    }

    private async void LoadPageFromNotification(NotificationEventArgs e)
    {
        if (e.Request is null)
        {
            return;
        }

        // No need to use NotificationSerializer, you can use your own one.
        var list = _notificationSerializer.Deserialize<List<string>>(e.Request.ReturningData);
        if (list is null || list.Count != 4)
        {
            return;
        }

        if (list[0] != typeof(NotificationPage).FullName)
        {
            return;
        }

        var id = list[1];
        var message = list[2];
        var tapCount = list[3];

        await ((NavigationPage)MainPage).Navigation.PushModalAsync(new NotificationPage(int.Parse(id), message,
            int.Parse(tapCount)));
    }
}
