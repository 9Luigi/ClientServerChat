using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatShared.Protocol;
using WFChatClient;
using WFChatServer;
using Xunit;

namespace WFChat.Tests
{
    /// <summary>ChatClient against a real ChatServer on localhost with fake users.</summary>
    public sealed class ChatClientTests : IDisposable
    {
        static readonly TimeSpan EventTimeout = TimeSpan.FromSeconds(5);

        readonly ChatServer _server = new ChatServer(new FakeAuthenticator());
        readonly List<ChatClient> _chats = new List<ChatClient>();

        public ChatClientTests()
        {
            _server.Start(IPAddress.Loopback, 0);
        }

        int Port => _server.LocalEndPoint.Port;

        public void Dispose()
        {
            foreach (ChatClient chat in _chats)
            {
                chat.Dispose();
            }
            _server.Dispose();
        }

        [Fact]
        public async Task Successful_login_then_messages_arrive_after_StartReceiving()
        {
            ChatClient chat = NewClient(out BlockingCollection<ChatMessage> received);

            LoginResult result = await chat.ConnectAsync(IPAddress.Loopback, Port, "bob", "bob-pass");
            chat.StartReceiving();

            Assert.True(result.Success);
            Assert.Equal("bob", Assert.IsType<UserJoined>(Take(received)).UserName); //sent before StartReceiving, not lost
        }

        [Fact]
        public async Task Refused_login_returns_the_reason_instead_of_throwing()
        {
            ChatClient chat = NewClient(out _);

            LoginResult result = await chat.ConnectAsync(IPAddress.Loopback, Port, "bob", "wrong");

            Assert.Equal(new LoginResult(false, LoginError.IncorrectPassword), result);
        }

        [Fact]
        public async Task Unreachable_server_throws_SocketException()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int closedPort = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop(); //nobody listens on this port now

            ChatClient chat = NewClient(out _);

            await Assert.ThrowsAnyAsync<SocketException>(() => chat.ConnectAsync(IPAddress.Loopback, closedPort, "bob", "bob-pass"));
        }

        [Fact]
        public async Task Sent_text_comes_back_from_the_server()
        {
            var (chat, received) = await LoggedInClientAsync("bob", "bob-pass");

            await chat.SendTextAsync("hi");

            ChatText text = Assert.IsType<ChatText>(Take(received));
            Assert.Equal("bob", text.From);
            Assert.Equal("hi", text.Text);
        }

        [Fact]
        public async Task Text_from_another_user_arrives()
        {
            var (_, bobReceived) = await LoggedInClientAsync("bob", "bob-pass");
            var (alice, _) = await LoggedInClientAsync("alice", "alice pass");
            Assert.Equal("alice", Assert.IsType<UserJoined>(Take(bobReceived)).UserName);

            await alice.SendTextAsync("hello bob");

            Assert.Equal("hello bob", Assert.IsType<ChatText>(Take(bobReceived)).Text);
        }

        [Fact]
        public async Task Server_stop_raises_Disconnected()
        {
            var (chat, _) = await LoggedInClientAsync("bob", "bob-pass");
            var disconnected = new TaskCompletionSource<bool>();
            chat.Disconnected += _ => disconnected.TrySetResult(true);

            _server.Stop();

            await disconnected.Task.WaitAsync(EventTimeout);
        }

        [Fact]
        public async Task Dispose_does_not_raise_Disconnected()
        {
            var (chat, _) = await LoggedInClientAsync("bob", "bob-pass");
            bool raised = false;
            chat.Disconnected += _ => raised = true;

            chat.Dispose();
            await Task.Delay(300);

            Assert.False(raised);
        }

        [Fact]
        public async Task Sending_before_login_throws()
        {
            ChatClient chat = NewClient(out _);

            await Assert.ThrowsAsync<InvalidOperationException>(() => chat.SendTextAsync("hi"));
        }

        ChatClient NewClient(out BlockingCollection<ChatMessage> received)
        {
            var chat = new ChatClient();
            var messages = new BlockingCollection<ChatMessage>();
            chat.MessageReceived += messages.Add;
            _chats.Add(chat);
            received = messages;
            return chat;
        }

        /// <summary>Logs in, starts receiving and consumes the user's own join notice.</summary>
        async Task<(ChatClient Chat, BlockingCollection<ChatMessage> Received)> LoggedInClientAsync(string userName, string password)
        {
            ChatClient chat = NewClient(out BlockingCollection<ChatMessage> received);
            Assert.True((await chat.ConnectAsync(IPAddress.Loopback, Port, userName, password)).Success);
            chat.StartReceiving();
            Assert.Equal(userName, Assert.IsType<UserJoined>(Take(received)).UserName);
            return (chat, received);
        }

        static ChatMessage Take(BlockingCollection<ChatMessage> received)
        {
            Assert.True(received.TryTake(out ChatMessage message, EventTimeout), "No message within " + EventTimeout);
            return message;
        }
    }
}
