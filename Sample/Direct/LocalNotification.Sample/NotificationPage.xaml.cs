using System;
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

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}