using System;
using System.Globalization;
using ChatShared.Protocol;

namespace ChatShared
{
    /// <summary>How broadcast messages look in the chat windows of the client and the server.</summary>
    public static class ChatMessageText
    {
        /// <summary>Returns the display line for <see cref="ChatText"/>, <see cref="UserJoined"/> and <see cref="UserLeft"/>; null for other messages.</summary>
        public static string Format(ChatMessage message)
        {
            switch (message)
            {
                case ChatText text:
                    return $"{FormatTime(text.Time)} {text.From}: {text.Text}";
                case UserJoined joined:
                    return $"{FormatTime(joined.Time)} {joined.UserName} entered the chat";
                case UserLeft left:
                    return $"{FormatTime(left.Time)} {left.UserName} left the chat";
                default:
                    return null;
            }
        }

        public static string FormatTime(DateTimeOffset time)
        {
            return time.ToLocalTime().ToString("HH:mm:ss", CultureInfo.InvariantCulture); //24-hour clock in the reader's time zone
        }
    }
}
