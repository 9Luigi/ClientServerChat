using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatShared.Protocol;
using WFChatServer;
using Xunit;

namespace WFChat.Tests
{
    /// <summary>Users "bob" / "bob-pass" and "alice" / "alice pass", no database.</summary>
    sealed class FakeAuthenticator : IUserAuthenticator
    {
        readonly Dictionary<string, string> _passwords = new Dictionary<string, string>
        {
            ["bob"] = "bob-pass",
            ["alice"] = "alice pass"
        };

        /// <summary>When set, every check throws it, like an unavailable database.</summary>
        public Exception FailWith { get; set; }

        public LoginError? Authenticate(string userName, string password)
        {
            if (FailWith != null)
            {
                throw FailWith;
            }
            if (!_passwords.TryGetValue(userName, out string expected))
            {
                return LoginError.UserNotFound;
            }
            return expected == password ? null : LoginError.IncorrectPassword;
        }
    }

    /// <summary>A bare protocol client for testing the server: sends and receives raw messages.</summary>
    sealed class TestClient : IDisposable
    {
        static readonly TimeSpan ReceiveTimeout = TimeSpan.FromSeconds(5);

        readonly TcpClient _tcp;

        TestClient(TcpClient tcp)
        {
            _tcp = tcp;
            Connection = new ChatConnection(tcp.GetStream());
        }

        public ChatConnection Connection { get; }

        public static async Task<TestClient> ConnectAsync(IPEndPoint server)
        {
            var tcp = new TcpClient();
            await tcp.ConnectAsync(IPAddress.Loopback, server.Port);
            return new TestClient(tcp);
        }

        public Task SendAsync(ChatMessage message)
        {
            return Connection.SendAsync(message);
        }

        /// <summary>Writes bytes past the protocol, e.g. to send garbage.</summary>
        public async Task SendRawAsync(string text)
        {
            await _tcp.GetStream().WriteAsync(Encoding.UTF8.GetBytes(text));
        }

        /// <summary>Next message; fails the test instead of hanging if nothing comes in 5 seconds.</summary>
        public async Task<ChatMessage> ReceiveAsync()
        {
            using var timeout = new CancellationTokenSource(ReceiveTimeout);
            return await Connection.ReceiveAsync(timeout.Token);
        }

        public async Task<T> ReceiveAsync<T>() where T : ChatMessage
        {
            return Assert.IsType<T>(await ReceiveAsync());
        }

        /// <summary>Passes when the server closes the connection (end of stream or a reset).</summary>
        public async Task AssertClosedByServerAsync()
        {
            try
            {
                Assert.Null(await ReceiveAsync());
            }
            catch (IOException)
            {
            }
        }

        /// <summary>Fails if any message arrives within <paramref name="wait"/>. The connection is unusable afterwards.</summary>
        public async Task AssertNothingArrivesAsync(TimeSpan wait)
        {
            using var timeout = new CancellationTokenSource(wait);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Connection.ReceiveAsync(timeout.Token));
        }

        public void Dispose()
        {
            Connection.Dispose();
            _tcp.Dispose();
        }
    }
}
