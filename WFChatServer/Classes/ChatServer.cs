using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ChatShared.Protocol;

namespace WFChatServer
{
    /// <summary>
    /// TCP chat server: accepts connections, logs users in and relays their messages to every logged-in user.
    /// Knows nothing about WinForms; the form listens to <see cref="MessageBroadcast"/> and <see cref="Log"/>.
    /// </summary>
    /// <remarks>Events are raised on thread-pool threads.</remarks>
    public sealed class ChatServer : IDisposable
    {
        readonly IUserAuthenticator _authenticator;
        readonly List<ClientConnection> _clients = new List<ClientConnection>(); //logged-in clients only; guarded by lock (_clients)
        TcpListener _listener;
        CancellationTokenSource _stopping;

        public ChatServer(IUserAuthenticator authenticator)
        {
            ArgumentNullException.ThrowIfNull(authenticator);
            _authenticator = authenticator;
        }

        /// <summary>How long a new connection may wait before sending its <see cref="LoginRequest"/>.</summary>
        public TimeSpan LoginTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>Every message sent to all clients: <see cref="UserJoined"/>, <see cref="ChatText"/>, <see cref="UserLeft"/>.</summary>
        public event Action<ChatMessage> MessageBroadcast;

        /// <summary>Diagnostics for the operator: rejected logins, protocol violations, network errors.</summary>
        public event Action<string> Log;

        /// <summary>Address the server listens on; useful when started with port 0.</summary>
        public IPEndPoint LocalEndPoint => (IPEndPoint)_listener?.LocalEndpoint;

        /// <summary>Starts listening.</summary>
        /// <exception cref="SocketException">The address or port can't be used (port taken, address not on this machine).</exception>
        public void Start(IPAddress address, int port)
        {
            if (_listener != null)
            {
                throw new InvalidOperationException("The server is already started.");
            }
            var listener = new TcpListener(address, port);
            listener.Start();
            _listener = listener;
            _stopping = new CancellationTokenSource();
            _ = AcceptLoopAsync(listener, _stopping.Token);
        }

        /// <summary>Stops listening and disconnects everyone.</summary>
        public void Stop()
        {
            if (_listener == null)
            {
                return;
            }
            _stopping.Cancel();
            _listener.Stop();
            _listener = null;
            foreach (ClientConnection client in TakeSnapshot())
            {
                client.Close();
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public IReadOnlyList<string> GetOnlineUsers()
        {
            lock (_clients)
            {
                return _clients.Select(c => c.UserName).ToList();
            }
        }

        async Task AcceptLoopAsync(TcpListener listener, CancellationToken stopping)
        {
            while (true)
            {
                TcpClient tcp;
                try
                {
                    tcp = await listener.AcceptTcpClientAsync(stopping).ConfigureAwait(false); //waits without holding a thread
                }
                catch (Exception ex) when (ex is OperationCanceledException || ex is ObjectDisposedException || ex is SocketException)
                {
                    if (!stopping.IsCancellationRequested)
                    {
                        RaiseLog("Stopped accepting connections: " + ex.Message);
                    }
                    return;
                }
                _ = HandleClientAsync(new ClientConnection(tcp), stopping); //not awaited: the loop goes back to accepting at once
            }
        }

        /// <summary>The whole life of one connection: login, then relaying its messages until it disconnects. Never throws.</summary>
        async Task HandleClientAsync(ClientConnection client, CancellationToken stopping)
        {
            bool joined = false;
            try
            {
                if (!await LoginAsync(client, stopping).ConfigureAwait(false))
                {
                    return;
                }
                lock (_clients)
                {
                    _clients.Add(client); //only now the client starts receiving chat messages
                }
                joined = true;
                await BroadcastAsync(new UserJoined(client.UserName, DateTimeOffset.Now)).ConfigureAwait(false);

                while (true)
                {
                    ChatMessage message = await client.Connection.ReceiveAsync(stopping).ConfigureAwait(false);
                    if (message == null)
                    {
                        break; //the client closed the connection
                    }
                    if (message is SendText text && text.Text != null)
                    {
                        await BroadcastAsync(new ChatText(client.UserName, text.Text, DateTimeOffset.Now)).ConfigureAwait(false);
                    }
                    else
                    {
                        RaiseLog($"{client.Name}: unexpected {message.GetType().Name} ignored");
                    }
                }
            }
            catch (OperationCanceledException) when (stopping.IsCancellationRequested)
            {
            }
            catch (Exception ex) when (IsConnectionError(ex))
            {
                if (!stopping.IsCancellationRequested)
                {
                    RaiseLog($"{client.Name}: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                RaiseLog($"{client.Name}: unexpected error: {ex}");
            }
            finally
            {
                if (joined)
                {
                    lock (_clients)
                    {
                        _clients.Remove(client); //before Close, so broadcasts don't try to send to a closed socket
                    }
                }
                client.Close();
                if (joined && !stopping.IsCancellationRequested)
                {
                    await BroadcastAsync(new UserLeft(client.UserName, DateTimeOffset.Now)).ConfigureAwait(false);
                }
            }
        }

        /// <summary>Reads the first message, checks the credentials and answers with <see cref="LoginResult"/>. Returns true on success.</summary>
        async Task<bool> LoginAsync(ClientConnection client, CancellationToken stopping)
        {
            ChatMessage first;
            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(stopping))
            {
                timeout.CancelAfter(LoginTimeout);
                try
                {
                    first = await client.Connection.ReceiveAsync(timeout.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (!stopping.IsCancellationRequested)
                {
                    RaiseLog($"{client.Name}: no login within {LoginTimeout.TotalSeconds:0.#} s, disconnected");
                    return false;
                }
            }
            if (first == null)
            {
                return false;
            }
            if (!(first is LoginRequest login))
            {
                RaiseLog($"{client.Name}: first message was {first.GetType().Name} instead of login, disconnected");
                return false;
            }

            LoginError? error;
            if (string.IsNullOrEmpty(login.UserName))
            {
                error = LoginError.UserNotFound;
            }
            else if (login.Password == null)
            {
                error = LoginError.IncorrectPassword;
            }
            else
            {
                try
                {
                    error = _authenticator.Authenticate(login.UserName, login.Password);
                }
                catch (Exception ex)
                {
                    RaiseLog($"Cannot check the login of '{login.UserName}': {ex.Message}");
                    error = LoginError.ServerError;
                }
            }

            if (error != null)
            {
                RaiseLog($"{client.Name}: login '{login.UserName}' rejected: {error}");
                await client.Connection.SendAsync(new LoginResult(false, error)).ConfigureAwait(false);
                return false;
            }
            client.UserName = login.UserName;
            await client.Connection.SendAsync(new LoginResult(true)).ConfigureAwait(false); //the client must get its answer before any broadcast
            return true;
        }

        async Task BroadcastAsync(ChatMessage message)
        {
            MessageBroadcast?.Invoke(message);
            ClientConnection[] recipients = TakeSnapshot(); //a copy, so the lock isn't held while sending
            await Task.WhenAll(recipients.Select(c => SendOrDropAsync(c, message))).ConfigureAwait(false);
        }

        /// <summary>A failed send drops only that client; the others still get the message.</summary>
        async Task SendOrDropAsync(ClientConnection client, ChatMessage message)
        {
            try
            {
                await client.Connection.SendAsync(message).ConfigureAwait(false);
            }
            catch (Exception ex) when (IsConnectionError(ex))
            {
                RaiseLog($"{client.Name}: send failed ({ex.Message}), disconnecting");
                client.Close(); //its receive loop ends with an error and does the cleanup
            }
        }

        ClientConnection[] TakeSnapshot()
        {
            lock (_clients)
            {
                return _clients.ToArray();
            }
        }

        static bool IsConnectionError(Exception ex)
        {
            return ex is IOException || ex is SocketException || ex is ObjectDisposedException || ex is InvalidDataException;
        }

        void RaiseLog(string text)
        {
            Log?.Invoke(text);
        }

        /// <summary>One connected client: its socket, protocol connection and, after login, its user name.</summary>
        sealed class ClientConnection
        {
            readonly TcpClient _tcp;
            readonly string _remoteEndPoint;

            public ClientConnection(TcpClient tcp)
            {
                _tcp = tcp;
                _remoteEndPoint = tcp.Client.RemoteEndPoint?.ToString();
                Connection = new ChatConnection(tcp.GetStream());
            }

            public ChatConnection Connection { get; }

            /// <summary>Null until the login succeeds.</summary>
            public string UserName { get; set; }

            /// <summary>For log messages: the user name, or the address before login.</summary>
            public string Name => UserName ?? _remoteEndPoint;

            /// <summary>Closes the socket; a pending receive or send fails. Safe to call more than once.</summary>
            public void Close()
            {
                _tcp.Dispose();
                Connection.Dispose();
            }
        }
    }
}
