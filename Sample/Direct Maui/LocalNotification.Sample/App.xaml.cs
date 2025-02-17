namespace LocalNotification.Sample;

public partial class App : Application
{
    private readonly MainPage mainPage;

    public App(MainPage mainPage)
	{
		InitializeComponent();
        this.mainPage = mainPage;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(mainPage));
    }
}
