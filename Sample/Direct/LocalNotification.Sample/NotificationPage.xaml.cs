using Xamarin.Forms;

namespace LocalNotification.Sample
{
    public partial class NotificationPage : ContentPage
    {
        public NotificationPage(int id, string message, int tabCount)
        {
            InitializeComponent();

            IdLabel.Text = $"Id {id}";
            MessageLabel.Text = $"Message {message}";
            TapCountLabel.Text = $"Tap Count {tabCount}";
        }
    }
}