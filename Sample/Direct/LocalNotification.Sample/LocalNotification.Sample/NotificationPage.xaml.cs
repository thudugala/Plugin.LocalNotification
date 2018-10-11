using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LocalNotification.Sample
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotificationPage : ContentPage
	{
	    public NotificationPage (int tabCount)
	    {
            InitializeComponent ();

	        TapCountLabel.Text = $"Tap Count {tabCount}";
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            App.Current.GoToMainPage();
        }
    }
}