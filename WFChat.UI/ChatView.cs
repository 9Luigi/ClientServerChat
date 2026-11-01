using System;
using System.Drawing;
using System.Windows.Forms;
using ChatShared;
using ChatShared.Protocol;

namespace WFChat.UI
{
    /// <summary>
    /// Writes chat messages into a RichTextBox with colors: gray time, the user name in its own color
    /// (navy and bold for the current user), gray italic notices for joins and leaves.
    /// </summary>
    public static class ChatView
    {
        /// <param name="currentUserName">Messages of this user are highlighted; null on the server.</param>
        public static void AppendMessage(RichTextBox box, ChatMessage message, string currentUserName = null)
        {
            switch (message)
            {
                case ChatText text:
                    StartLine(box);
                    Append(box, ChatMessageText.FormatTime(text.Time) + "  ", Theme.TextMuted, Theme.ChatFont);
                    bool isMe = currentUserName != null && string.Equals(text.From, currentUserName, StringComparison.OrdinalIgnoreCase);
                    Append(box, text.From, isMe ? Theme.Primary : UserColors.For(text.From), Theme.ChatFontBold);
                    Append(box, ": " + text.Text, Theme.Text, Theme.ChatFont);
                    break;
                case UserJoined joined:
                    AppendNotice(box, $"{ChatMessageText.FormatTime(joined.Time)}  {joined.UserName} entered the chat");
                    break;
                case UserLeft left:
                    AppendNotice(box, $"{ChatMessageText.FormatTime(left.Time)}  {left.UserName} left the chat");
                    break;
                default:
                    return;
            }
            box.ScrollToCaret();
        }

        /// <summary>A gray italic line such as "Welcome bob!" or "server started".</summary>
        public static void AppendNotice(RichTextBox box, string text)
        {
            StartLine(box);
            Append(box, text, Theme.TextMuted, Theme.ChatFontItalic);
            box.ScrollToCaret();
        }

        static void StartLine(RichTextBox box)
        {
            if (box.TextLength > 0)
            {
                Append(box, "\n", Theme.Text, Theme.ChatFont);
            }
        }

        /// <summary>The selection at the end of the text decides the color and font of the appended piece.</summary>
        static void Append(RichTextBox box, string text, Color color, Font font)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.SelectionFont = font;
            box.AppendText(text);
        }
    }
}
