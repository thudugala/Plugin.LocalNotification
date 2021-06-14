using System;
namespace Plugin.LocalNotification
{
    public class NotificationAction
    {
        public string Identifier { get; }
        public string Title { get; }
        public Action<int, string> Handler { get; }
        public ActionTypes ActionType { get; }

        public NotificationAction(string identifier, string title, Action<int, string> handler, ActionTypes actionType = ActionTypes.Default)
        {
            Identifier = identifier;
            Title = title;
            Handler = handler;
            ActionType = actionType;
        }
    }
}
