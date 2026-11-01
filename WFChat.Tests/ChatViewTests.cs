using System;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;
using ChatShared.Protocol;
using WFChat.UI;
using Xunit;

namespace WFChat.Tests
{
    public class UserColorsTests
    {
        [Fact]
        public void Same_name_always_gets_the_same_color_regardless_of_case()
        {
            Assert.Equal(UserColors.For("alice"), UserColors.For("alice"));
            Assert.Equal(UserColors.For("alice"), UserColors.For("Alice"));
        }

        [Fact]
        public void Different_names_spread_over_the_palette()
        {
            int distinct = Enumerable.Range(0, 50).Select(i => UserColors.For("user" + i)).Distinct().Count();

            Assert.True(distinct >= 6, $"only {distinct} colors for 50 names");
        }
    }

    public class ChatViewTests
    {
        static readonly DateTimeOffset LocalTime = new DateTimeOffset(new DateTime(2026, 11, 1, 23, 5, 7, DateTimeKind.Local));

        [Fact]
        public void Chat_text_has_gray_time_colored_name_and_plain_text()
        {
            RunInSta(() =>
            {
                using var box = NewBox();

                ChatView.AppendMessage(box, new ChatText("alice", "hi", LocalTime), currentUserName: "bob");

                Assert.Equal("23:05:07  alice: hi", box.Text);
                Assert.Equal(Theme.TextMuted, ColorAt(box, "23:05:07"));
                Assert.Equal(UserColors.For("alice"), ColorAt(box, "alice"));
                Assert.True(FontAt(box, "alice").Bold);
                Assert.Equal(Theme.Text, ColorAt(box, "hi"));
            });
        }

        [Fact]
        public void Own_name_is_navy()
        {
            RunInSta(() =>
            {
                using var box = NewBox();

                ChatView.AppendMessage(box, new ChatText("bob", "hello", LocalTime), currentUserName: "bob");

                Assert.Equal(Theme.Primary, ColorAt(box, "bob"));
            });
        }

        [Fact]
        public void Join_and_leave_are_gray_italic_lines()
        {
            RunInSta(() =>
            {
                using var box = NewBox();

                ChatView.AppendMessage(box, new UserJoined("alice", LocalTime));
                ChatView.AppendMessage(box, new UserLeft("alice", LocalTime));

                Assert.Equal(new[] { "23:05:07  alice entered the chat", "23:05:07  alice left the chat" }, box.Lines);
                Assert.Equal(Theme.TextMuted, ColorAt(box, "entered"));
                Assert.True(FontAt(box, "entered").Italic);
            });
        }

        [Fact]
        public void Each_message_starts_on_a_new_line()
        {
            RunInSta(() =>
            {
                using var box = NewBox();

                ChatView.AppendNotice(box, "Welcome bob!");
                ChatView.AppendMessage(box, new ChatText("alice", "one", LocalTime));
                ChatView.AppendMessage(box, new ChatText("bob", "two", LocalTime));

                Assert.Equal(new[] { "Welcome bob!", "23:05:07  alice: one", "23:05:07  bob: two" }, box.Lines);
            });
        }

        static RichTextBox NewBox()
        {
            var box = new RichTextBox { ReadOnly = true, Font = Theme.ChatFont };
            _ = box.Handle; //formatting needs the native control
            return box;
        }

        static Color ColorAt(RichTextBox box, string fragment)
        {
            box.Select(box.Text.IndexOf(fragment, StringComparison.Ordinal), 1);
            return box.SelectionColor;
        }

        static Font FontAt(RichTextBox box, string fragment)
        {
            box.Select(box.Text.IndexOf(fragment, StringComparison.Ordinal), 1);
            return box.SelectionFont;
        }

        /// <summary>RichTextBox is a COM-based control and must live on an STA thread; test threads are MTA.</summary>
        static void RunInSta(Action action)
        {
            Exception error = null;
            var thread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (error != null)
            {
                ExceptionDispatchInfo.Capture(error).Throw();
            }
        }
    }
}
