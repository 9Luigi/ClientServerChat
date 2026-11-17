using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ChatShared.Protocol;
using WFChatServer;
using Xunit;

namespace WFChat.Tests
{
    /// <summary>A real ChatServer on a free localhost port, with fake users instead of the database.</summary>
    public sealed class ChatServerTests : IDisposable
    {
        readonly FakeAuthenticator _authenticator = new FakeAuthenticator();
        readonly ChatServer _server;
        readonly ConcurrentQueue<string> _log = new ConcurrentQueue<string>();
        readonly ConcurrentQueue<ChatMessage> _broadcasts = new ConcurrentQueue<ChatMessage>();
        readonly List<TestClient> _clients = new List<TestClient>();

        public ChatServerTests()
        {
            _server = new ChatServer(_authenticator);
            _server.Log += _log.Enqueue;
            _server.MessageBroadcast += _broadcasts.Enqueue;
            _server.Start(IPAddress.Loopback, 0); //port 0: the system picks a free one
        }

        public void Dispose()
        {
            foreach (TestClient client in _clients)
            {
                client.Dispose();
            }
            _server.Dispose();
        }

        [Fact]
        public async Task Correct_login_is_answered_with_success_and_announced()
        {
            await LoginAsync("bob", "bob-pass"); //the helper checks LoginResult(true) and UserJoined("bob")

            Assert.Equal(new[] { "bob" }, _server.GetOnlineUsers());
        }

        [Theory]
        [InlineData("bob", "wrong")]
        [InlineData("nobody", "x")]
        [InlineData("", "x")]
        [InlineData("bob", null)]
        public async Task Wrong_name_or_password_gets_the_same_answer_and_is_disconnected(string userName, string password)
        {
            TestClient client = await ConnectAsync();

            await client.SendAsync(new LoginRequest(userName, password));

            Assert.Equal(new LoginResult(false, LoginError.InvalidCredentials), await client.ReceiveAsync());
            await client.AssertClosedByServerAsync();
            Assert.Empty(_server.GetOnlineUsers());
        }

        [Fact]
        public async Task Password_with_spaces_works()
        {
            await LoginAsync("alice", "alice pass");
        }

        [Fact]
        public async Task Text_goes_to_everyone_including_the_sender_with_the_sender_name()
        {
            TestClient bob = await LoginAsync("bob", "bob-pass");
            TestClient alice = await LoginAsync("alice", "alice pass");
            Assert.Equal("alice", (await bob.ReceiveAsync<UserJoined>()).UserName);

            await alice.SendAsync(new SendText("hi"));

            foreach (TestClient client in new[] { bob, alice })
            {
                ChatText text = await client.ReceiveAsync<ChatText>();
                Assert.Equal("alice", text.From);
                Assert.Equal("hi", text.Text);
            }
        }

        [Theory]
        [InlineData("bob")]
        [InlineData("BOB")]
        public async Task Second_login_with_an_online_name_is_refused(string secondName)
        {
            await LoginAsync("bob", "bob-pass");
            _authenticator.AcceptAnyCase = true;
            TestClient second = await ConnectAsync();

            await second.SendAsync(new LoginRequest(secondName, "bob-pass"));

            Assert.Equal(new LoginResult(false, LoginError.AlreadyLoggedIn), await second.ReceiveAsync());
            await second.AssertClosedByServerAsync();
            Assert.Equal(new[] { "bob" }, _server.GetOnlineUsers());
        }

        [Fact]
        public async Task Name_is_free_again_after_the_user_leaves()
        {
            TestClient bob = await LoginAsync("bob", "bob-pass");
            TestClient alice = await LoginAsync("alice", "alice pass");
            await bob.ReceiveAsync<UserJoined>();

            bob.Dispose();
            await alice.ReceiveAsync<UserLeft>();

            await LoginAsync("bob", "bob-pass");
        }

        [Fact]
        public async Task Line_breaks_in_a_text_become_spaces()
        {
            TestClient bob = await LoginAsync("bob", "bob-pass");

            await bob.SendAsync(new SendText("hi\n12:00:00  alice: fake\r\tline"));

            Assert.Equal("hi 12:00:00  alice: fake  line", (await bob.ReceiveAsync<ChatText>()).Text);
        }

        [Fact]
        public async Task Too_long_text_is_cut()
        {
            TestClient bob = await LoginAsync("bob", "bob-pass");

            await bob.SendAsync(new SendText(new string('x', SendText.MaxLength + 10)));

            Assert.Equal(new string('x', SendText.MaxLength), (await bob.ReceiveAsync<ChatText>()).Text);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\n\r\t")]
        [InlineData(null)]
        public async Task Empty_text_is_not_relayed(string text)
        {
            TestClient bob = await LoginAsync("bob", "bob-pass");

            await bob.SendAsync(new SendText(text));
            await bob.SendAsync(new SendText("next"));

            Assert.Equal("next", (await bob.ReceiveAsync<ChatText>()).Text); //the empty one was skipped
        }

        [Fact]
        public async Task Connection_without_login_receives_nothing()
        {
            TestClient spy = await ConnectAsync();
            TestClient bob = await LoginAsync("bob", "bob-pass");

            await bob.SendAsync(new SendText("secret"));
            await bob.ReceiveAsync<ChatText>();

            await spy.AssertNothingArrivesAsync(TimeSpan.FromMilliseconds(500));
            Assert.Equal(new[] { "bob" }, _server.GetOnlineUsers());
        }

        [Fact]
        public async Task First_message_other_than_login_disconnects()
        {
            TestClient client = await ConnectAsync();

            await client.SendAsync(new SendText("hi"));

            await client.AssertClosedByServerAsync();
            Assert.Contains(_log, line => line.Contains("instead of login"));
        }

        [Fact]
        public async Task Leaving_user_is_announced_and_removed_from_the_online_list()
        {
            TestClient bob = await LoginAsync("bob", "bob-pass");
            TestClient alice = await LoginAsync("alice", "alice pass");
            await bob.ReceiveAsync<UserJoined>();

            alice.Dispose();

            Assert.Equal("alice", (await bob.ReceiveAsync<UserLeft>()).UserName);
            Assert.Equal(new[] { "bob" }, _server.GetOnlineUsers());
        }

        [Fact]
        public async Task Database_failure_is_answered_with_ServerError()
        {
            _authenticator.FailWith = new InvalidOperationException("database is down");
            TestClient client = await ConnectAsync();

            await client.SendAsync(new LoginRequest("bob", "bob-pass"));

            Assert.Equal(new LoginResult(false, LoginError.ServerError), await client.ReceiveAsync());
            Assert.Contains(_log, line => line.Contains("database is down"));
        }

        [Fact]
        public async Task Silent_connection_is_dropped_after_the_login_timeout()
        {
            _server.LoginTimeout = TimeSpan.FromMilliseconds(200);

            TestClient client = await ConnectAsync();

            await client.AssertClosedByServerAsync();
            Assert.Contains(_log, line => line.Contains("no login"));
        }

        [Fact]
        public async Task Garbage_from_a_client_disconnects_only_that_client()
        {
            TestClient bob = await LoginAsync("bob", "bob-pass");
            TestClient alice = await LoginAsync("alice", "alice pass");
            await bob.ReceiveAsync<UserJoined>();

            await alice.SendRawAsync("this is not json\n");

            await alice.AssertClosedByServerAsync();
            Assert.Equal("alice", (await bob.ReceiveAsync<UserLeft>()).UserName);
        }

        [Fact]
        public async Task MessageBroadcast_reports_what_the_clients_receive()
        {
            TestClient bob = await LoginAsync("bob", "bob-pass");

            await bob.SendAsync(new SendText("hi"));
            await bob.ReceiveAsync<ChatText>();

            Assert.Collection(_broadcasts,
                m => Assert.Equal("bob", Assert.IsType<UserJoined>(m).UserName),
                m => Assert.Equal("hi", Assert.IsType<ChatText>(m).Text));
        }

        [Fact]
        public async Task Stop_disconnects_everyone()
        {
            TestClient bob = await LoginAsync("bob", "bob-pass");

            _server.Stop();

            await bob.AssertClosedByServerAsync();
        }

        async Task<TestClient> ConnectAsync()
        {
            TestClient client = await TestClient.ConnectAsync(_server.LocalEndPoint);
            _clients.Add(client);
            return client;
        }

        /// <summary>Connects and logs in; checks the success answer and the user's own join notice.</summary>
        async Task<TestClient> LoginAsync(string userName, string password)
        {
            TestClient client = await ConnectAsync();
            await client.SendAsync(new LoginRequest(userName, password));
            Assert.Equal(new LoginResult(true), await client.ReceiveAsync());
            Assert.Equal(userName, (await client.ReceiveAsync<UserJoined>()).UserName);
            return client;
        }
    }
}
