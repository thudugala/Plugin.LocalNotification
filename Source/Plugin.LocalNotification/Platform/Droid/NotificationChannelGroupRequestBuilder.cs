using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.LocalNotification.Platform.Droid
{
	/// <summary>
	/// 
	/// </summary>
	public class NotificationChannelGroupRequestBuilder
	{
		private string Group;
		private string Name;

		internal NotificationChannelGroupRequestBuilder()
		{

		}

		/// <summary>
		/// Builds the request to <see cref="NotificationChannelGroupRequest"/>
		/// </summary>
		/// <returns></returns>
		public NotificationChannelGroupRequest Build() => new(Group, Name);

		/// <summary>
		/// Sets the Group
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		public NotificationChannelGroupRequestBuilder WithGroup(string group)
		{
			Group = group;
			return this;
		}

		/// <summary>
		/// Sets the Name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public NotificationChannelGroupRequestBuilder WithName(string name)
		{
			Name = name;
			return this;
		}
	}
}
