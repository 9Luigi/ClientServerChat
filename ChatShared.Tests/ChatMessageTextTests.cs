using System;
using ChatShared.Protocol;
using Xunit;

namespace ChatShared.Tests
{
    public class ChatMessageTextTests
    {
        static readonly DateTimeOffset LocalTime = new DateTimeOffset(new DateTime(2026, 11, 1, 23, 5, 7, DateTimeKind.Local));

        [Fact]
        public void Chat_text_shows_24_hour_time_sender_and_text()
        {
            Assert.Equal("23:05:07 bob: hello", ChatMessageText.Format(new ChatText("bob", "hello", LocalTime)));
        }

        [Fact]
        public void Join_and_leave_have_their_own_wording()
        {
            Assert.Equal("23:05:07 bob entered the chat", ChatMessageText.Format(new UserJoined("bob", LocalTime)));
            Assert.Equal("23:05:07 bob left the chat", ChatMessageText.Format(new UserLeft("bob", LocalTime)));
        }

        [Fact]
        public void Time_is_shown_in_the_local_time_zone()
        {
            DateTimeOffset utc = LocalTime.ToUniversalTime(); //same moment, written with offset 0

            Assert.Equal("23:05:07 bob: hi", ChatMessageText.Format(new ChatText("bob", "hi", utc)));
        }

        [Fact]
        public void Messages_that_are_not_shown_in_the_chat_give_null()
        {
            Assert.Null(ChatMessageText.Format(new LoginResult(true)));
        }
    }
}
