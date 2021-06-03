using System;
namespace Plugin.LocalNotification
{
    public class NotificationAction
    {
        public string Identifier { get; }
        public string Title { get; }
        public Action<int, string> Handler { get; }

        public NotificationAction(string identifier, string title, Action<int, string> handler)
        {
            Identifier = identifier;
            Title = title;
            Handler = handler;
        }
    }
}
