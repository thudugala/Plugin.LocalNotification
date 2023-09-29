using Plugin.LocalNotification;

namespace LocalNotification.Sample;

public partial class NotificationPage : ContentPage
{
    private readonly INotificationService _notificationService;

    public NotificationPage(INotificationService notificationService, int id, string message, int tabCount)
    {
        InitializeComponent();
        _notificationService = notificationService;

        IdLabel.Text = $"Id {id}";
        MessageLabel.Text = $"Message {message}";
        TapCountLabel.Text = $"Tap Count {tabCount}";
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {       
        var deliveredNotificationList = await _notificationService.GetDeliveredNotificationList();

        if (deliveredNotificationList != null)
        {
            await DisplayAlert("Delivered Notification Count", deliveredNotificationList.Count.ToString(), "OK");
        }

        await Navigation.PopModalAsync();
    }
}