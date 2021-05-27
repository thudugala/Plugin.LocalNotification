using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LocalNotification.Sample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationPage : ContentPage
    {
        public NotificationPage(int tabCount)
        {
            InitializeComponent();

            TapCountLabel.Text = $"Tap Count {tabCount}";
        }
    }
}