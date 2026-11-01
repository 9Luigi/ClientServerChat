using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatShared.Protocol;
using Xunit;

namespace ChatShared.Tests
{
    public class ChatConnectionTests
    {
        static readonly DateTimeOffset SomeTime = new DateTimeOffset(2026, 11, 1, 23, 15, 15, TimeSpan.FromHours(3));

        public static IEnumerable<object[]> AllMessageTypes() => new List<object[]>
        {
            new object[] { new LoginRequest("bob", "secret") },
            new object[] { new LoginResult(true) },
            new object[] { new LoginResult(false, LoginError.IncorrectPassword) },
            new object[] { new SendText("hello") },
            new object[] { new ChatText("bob", "hello", SomeTime) },
            new object[] { new UserJoined("bob", SomeTime) },
            new object[] { new UserLeft("bob", SomeTime) },
        };

        [Theory]
        [MemberData(nameof(AllMessageTypes))]
        public async Task Every_message_type_arrives_unchanged(ChatMessage sent)
        {
            using var pair = await TcpPair.CreateAsync();

            await pair.Client.SendAsync(sent);
            ChatMessage received = await pair.Server.ReceiveAsync();

            Assert.Equal(sent, received); //records compare by value and by concrete type
        }

        [Fact]
        public async Task Wire_format_is_one_json_object_per_line()
        {
            string wire = await WriteToWire(new LoginRequest("bob", "p w"), new LoginResult(false, LoginError.IncorrectPassword));

            Assert.Equal(
                "{\"type\":\"login\",\"userName\":\"bob\",\"password\":\"p w\"}\n" +
                "{\"type\":\"loginResult\",\"success\":false,\"error\":\"IncorrectPassword\"}\n",
                wire);
        }

        [Fact]
        public async Task Messages_sent_back_to_back_are_received_separately_and_in_order()
        {
            using var pair = await TcpPair.CreateAsync();
            var sent = Enumerable.Range(0, 100).Select(i => new SendText("message " + i)).ToList();

            foreach (var message in sent)
            {
                await pair.Client.SendAsync(message);
            }

            foreach (var expected in sent)
            {
                Assert.Equal(expected, await pair.Server.ReceiveAsync());
            }
        }

        [Theory]
        [InlineData("first line\nsecond line")]
        [InlineData("привет, мир 👋")]
        [InlineData("quotes \" and backslash \\ and {\"json\":1}")]
        [InlineData("")]
        public async Task Text_content_survives_the_trip(string text)
        {
            using var pair = await TcpPair.CreateAsync();

            await pair.Client.SendAsync(new SendText(text));

            Assert.Equal(new SendText(text), await pair.Server.ReceiveAsync());
        }

        [Fact]
        public async Task Long_message_is_not_cut()
        {
            using var pair = await TcpPair.CreateAsync();
            string text = new string('x', 200_000);

            Task send = pair.Client.SendAsync(new SendText(text)); //larger than socket buffers, so read while sending
            ChatMessage received = await pair.Server.ReceiveAsync();
            await send;

            Assert.Equal(text, Assert.IsType<SendText>(received).Text);
        }

        [Fact]
        public async Task Password_with_spaces_is_kept()
        {
            using var pair = await TcpPair.CreateAsync();

            await pair.Client.SendAsync(new LoginRequest("bob", "my secret pass"));

            Assert.Equal("my secret pass", Assert.IsType<LoginRequest>(await pair.Server.ReceiveAsync()).Password);
        }

        [Fact]
        public async Task Concurrent_sends_do_not_mix_lines()
        {
            using var pair = await TcpPair.CreateAsync();
            var sent = Enumerable.Range(0, 200).Select(i => new SendText(i + new string('x', 1000))).ToList();

            Task sending = Task.WhenAll(sent.Select(m => Task.Run(() => pair.Client.SendAsync(m))));
            var received = new List<ChatMessage>();
            for (int i = 0; i < sent.Count; i++)
            {
                received.Add(await pair.Server.ReceiveAsync());
            }
            await sending;

            Assert.Equal(sent.OrderBy(m => m.Text), received.Cast<SendText>().OrderBy(m => m.Text));
        }

        [Fact]
        public async Task Receive_returns_null_when_the_other_side_disconnects()
        {
            using var pair = await TcpPair.CreateAsync();

            pair.CloseClient();

            Assert.Null(await pair.Server.ReceiveAsync());
        }

        [Fact]
        public async Task Type_field_does_not_have_to_be_first()
        {
            ChatMessage received = await ReadFromWire("{\"text\":\"hi\",\"type\":\"text\"}\n");

            Assert.Equal(new SendText("hi"), received);
        }

        [Theory]
        [InlineData("not json at all\n")]
        [InlineData("{\"type\":\"noSuchType\"}\n")]
        [InlineData("{\"text\":\"no type field\"}\n")]
        [InlineData("{\"type\":\"login\",\"userName\":\"bob\"}\n")] //password missing
        [InlineData("null\n")]
        public async Task Invalid_line_throws_InvalidDataException(string wire)
        {
            await Assert.ThrowsAsync<InvalidDataException>(() => ReadFromWire(wire));
        }

        [Fact]
        public async Task Connection_stays_usable_after_an_invalid_line()
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("garbage\n{\"type\":\"text\",\"text\":\"ok\"}\n"));
            using var connection = new ChatConnection(stream);

            await Assert.ThrowsAsync<InvalidDataException>(() => connection.ReceiveAsync());

            Assert.Equal(new SendText("ok"), await connection.ReceiveAsync());
        }

        static async Task<string> WriteToWire(params ChatMessage[] messages)
        {
            using var stream = new MemoryStream();
            using (var connection = new ChatConnection(stream))
            {
                foreach (var message in messages)
                {
                    await connection.SendAsync(message);
                }
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        static async Task<ChatMessage> ReadFromWire(string wire)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(wire));
            using var connection = new ChatConnection(stream);
            return await connection.ReceiveAsync();
        }

        /// <summary>Two ChatConnections joined by a real loopback TCP socket.</summary>
        sealed class TcpPair : IDisposable
        {
            readonly TcpClient _clientSocket;
            readonly TcpClient _serverSocket;
            public ChatConnection Client { get; }
            public ChatConnection Server { get; }

            TcpPair(TcpClient clientSocket, TcpClient serverSocket)
            {
                _clientSocket = clientSocket;
                _serverSocket = serverSocket;
                Client = new ChatConnection(clientSocket.GetStream());
                Server = new ChatConnection(serverSocket.GetStream());
            }

            public static async Task<TcpPair> CreateAsync()
            {
                var listener = new TcpListener(IPAddress.Loopback, 0);
                listener.Start();
                try
                {
                    var clientSocket = new TcpClient();
                    Task<TcpClient> accept = listener.AcceptTcpClientAsync();
                    await clientSocket.ConnectAsync(IPAddress.Loopback, ((IPEndPoint)listener.LocalEndpoint).Port);
                    return new TcpPair(clientSocket, await accept);
                }
                finally
                {
                    listener.Stop();
                }
            }

            public void CloseClient()
            {
                Client.Dispose();
                _clientSocket.Close();
            }

            public void Dispose()
            {
                Client.Dispose();
                Server.Dispose();
                _clientSocket.Dispose();
                _serverSocket.Dispose();
            }
        }
    }
}
