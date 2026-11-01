using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ChatShared.Protocol;

namespace WFChatClient
{
    /// <summary>
    /// Network side of the chat client: connects, logs in, sends text and reports incoming messages.
    /// Knows nothing about WinForms. One instance serves one connection; create a new one to reconnect.
    /// </summary>
    /// <remarks>Events are raised on thread-pool threads.</remarks>
    public sealed class ChatClient : IDisposable
    {
        static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(10);

        TcpClient _tcp;
        ChatConnection _connection;
        bool _loggedIn;
        bool _receiving;
        volatile bool _disposed;

        /// <summary>Messages from the server after login: <see cref="UserJoined"/>, <see cref="ChatText"/>, <see cref="UserLeft"/>.</summary>
        public event Action<ChatMessage> MessageReceived;

        /// <summary>
        /// Raised once when the connection is lost. The argument is the error, or null if the server closed the connection.
        /// Not raised after <see cref="Dispose"/>.
        /// </summary>
        public event Action<Exception> Disconnected;

        /// <summary>Connects and logs in. On a refused login the connection is closed and the result tells why.</summary>
        /// <exception cref="SocketException">The server can't be reached.</exception>
        /// <exception cref="OperationCanceledException">The server didn't answer in time.</exception>
        /// <exception cref="InvalidDataException">The server answered something other than a login result.</exception>
        public async Task<LoginResult> ConnectAsync(IPAddress address, int port, string userName, string password)
        {
            if (_tcp != null)
            {
                throw new InvalidOperationException("ChatClient connects only once; create a new one to reconnect.");
            }
            _tcp = new TcpClient();
            try
            {
                using (var timeout = new CancellationTokenSource(ConnectTimeout))
                {
                    await _tcp.ConnectAsync(address, port, timeout.Token).ConfigureAwait(false);
                    _connection = new ChatConnection(_tcp.GetStream());
                    await _connection.SendAsync(new LoginRequest(userName, password), timeout.Token).ConfigureAwait(false);
                    if (!(await _connection.ReceiveAsync(timeout.Token).ConfigureAwait(false) is LoginResult result))
                    {
                        throw new InvalidDataException("The server did not answer the login request.");
                    }
                    if (!result.Success)
                    {
                        Dispose();
                        return result;
                    }
                    _loggedIn = true;
                    return result;
                }
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        /// <summary>
        /// Starts delivering <see cref="MessageReceived"/> events. Call after a successful login, once the UI is ready:
        /// messages that arrive before are kept in the socket and not lost.
        /// </summary>
        public void StartReceiving()
        {
            if (!_loggedIn || _disposed)
            {
                throw new InvalidOperationException("Not logged in.");
            }
            if (_receiving)
            {
                return;
            }
            _receiving = true;
            _ = ReceiveLoopAsync();
        }

        public Task SendTextAsync(string text)
        {
            if (!_loggedIn || _disposed)
            {
                throw new InvalidOperationException("Not logged in.");
            }
            return _connection.SendAsync(new SendText(text));
        }

        async Task ReceiveLoopAsync()
        {
            Exception error = null;
            try
            {
                ChatMessage message;
                while ((message = await _connection.ReceiveAsync().ConfigureAwait(false)) != null)
                {
                    MessageReceived?.Invoke(message);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            bool disposedByOwner = _disposed;
            Dispose();
            if (!disposedByOwner)
            {
                Disconnected?.Invoke(error);
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _tcp?.Dispose();
            _connection?.Dispose();
        }
    }
}
