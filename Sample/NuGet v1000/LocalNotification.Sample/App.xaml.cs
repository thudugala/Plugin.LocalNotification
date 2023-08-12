namespace LocalNotification.Sample;

public partial class App : Application
{
    public App(MainPage mainPage)
	{
		InitializeComponent();

        //MainPage = new AppShell();

        MainPage = new NavigationPage(mainPage);
    }
}
