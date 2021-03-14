using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.LocalNotification
{
	/// <summary>
	/// NotificationRequest for iOS
	/// </summary>
	public class iOSOptionsBuilder
	{
		private bool HideForgroundAlert;
		private bool PlayForegroundSound;

		internal iOSOptionsBuilder()
		{

		}

		/// <summary>
		/// Builds the request to <see cref="iOSOptions"/>
		/// </summary>
		/// <returns></returns>
		public iOSOptions Build() => new iOSOptions()
		{
			HideForegroundAlert = HideForgroundAlert,
			PlayForegroundSound = PlayForegroundSound
		};

		/// <summary>
		/// Setting this flag will prevent iOS from displaying the default banner when a Notification is received in foreground
		/// Default is false
		/// </summary>
		public iOSOptionsBuilder WithForegroundAlertStatus(bool shouldHideForegroundAlert)
		{
			HideForgroundAlert = shouldHideForegroundAlert;
			return this;
		}

		/// <summary>
		/// Setting this flag will enable iOS to play the default notification sound even if the app is in foreground
		/// Default is false
		/// </summary>
		public iOSOptionsBuilder WithForegroundSoundStatus(bool shouldPlayForegroundSound)
		{
			PlayForegroundSound = shouldPlayForegroundSound;
			return this;
		}
	}
}
