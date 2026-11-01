using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ChatShared.Protocol
{
    /// <summary>
    /// Sends and receives <see cref="ChatMessage"/>s over a stream (usually a NetworkStream).
    /// Wire format: one JSON object per line, UTF-8, lines end with '\n'.
    /// </summary>
    /// <remarks>
    /// SendAsync may be called from several threads at once. ReceiveAsync must be called by one reader at a time.
    /// The stream is not owned: dispose the TcpClient/stream yourself.
    /// </remarks>
    public sealed class ChatConnection : IDisposable
    {
        /// <summary>Longest accepted line, in characters. Protects the reader from a peer that never sends '\n'.</summary>
        public const int DefaultMaxMessageLength = 64 * 1024;

        static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            AllowOutOfOrderMetadataProperties = true, //"type" doesn't have to be the first field
            RespectRequiredConstructorParameters = true //a missing field is a protocol error, not a silent null
        };

        static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        readonly int _maxMessageLength;
        readonly StreamReader _reader;
        readonly StreamWriter _writer;
        readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        readonly char[] _readBuffer = new char[4096];
        int _readBufferPosition;
        int _readBufferLength;
        readonly StringBuilder _line = new StringBuilder();

        public ChatConnection(Stream stream, int maxMessageLength = DefaultMaxMessageLength)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentOutOfRangeException.ThrowIfLessThan(maxMessageLength, 1);
            _maxMessageLength = maxMessageLength;
            _reader = new StreamReader(stream, Utf8NoBom, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true);
            _writer = new StreamWriter(stream, Utf8NoBom, bufferSize: 4096, leaveOpen: true) { NewLine = "\n" };
        }

        /// <exception cref="ArgumentException">The serialized message is longer than the maximum message length.</exception>
        public async Task SendAsync(ChatMessage message, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);
            string json = JsonSerializer.Serialize(message, JsonOptions); //escapes '\n' inside strings, so the line stays one line
            if (json.Length > _maxMessageLength)
            {
                throw new ArgumentException($"Message is longer than {_maxMessageLength} characters.", nameof(message));
            }

            await _sendLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try //the lock keeps lines from two threads from interleaving
            {
                await _writer.WriteLineAsync(json.AsMemory(), cancellationToken).ConfigureAwait(false);
                await _writer.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        /// <summary>Waits for the next message. Returns null when the other side has closed the connection.</summary>
        /// <exception cref="InvalidDataException">
        /// The received line is not a valid message. After an invalid line the connection can be read further,
        /// except when the line was too long: then the connection is out of sync and should be closed.
        /// </exception>
        public async Task<ChatMessage> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            string line = await ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line == null)
            {
                return null;
            }
            try
            {
                return JsonSerializer.Deserialize<ChatMessage>(line, JsonOptions)
                    ?? throw new InvalidDataException("Received an empty (null) message.");
            }
            catch (Exception ex) when (ex is JsonException || ex is NotSupportedException) //NotSupportedException: unknown "type"
            {
                throw new InvalidDataException("Received an invalid message: " + ex.Message, ex);
            }
        }

        /// <summary>Like StreamReader.ReadLineAsync, but stops at the length limit. Returns null at the end of the stream.</summary>
        async Task<string> ReadLineAsync(CancellationToken cancellationToken)
        {
            _line.Clear();
            while (true)
            {
                if (_readBufferPosition == _readBufferLength)
                {
                    _readBufferLength = await _reader.ReadAsync(_readBuffer.AsMemory(), cancellationToken).ConfigureAwait(false);
                    _readBufferPosition = 0;
                    if (_readBufferLength == 0)
                    {
                        return null; //closed; an unfinished last line is not a message
                    }
                }

                int newLine = Array.IndexOf(_readBuffer, '\n', _readBufferPosition, _readBufferLength - _readBufferPosition);
                int end = newLine >= 0 ? newLine : _readBufferLength;
                _line.Append(_readBuffer, _readBufferPosition, end - _readBufferPosition);
                _readBufferPosition = newLine >= 0 ? newLine + 1 : end;

                if (_line.Length > _maxMessageLength)
                {
                    throw new InvalidDataException($"Received a message longer than {_maxMessageLength} characters.");
                }
                if (newLine >= 0)
                {
                    if (_line.Length > 0 && _line[_line.Length - 1] == '\r')
                    {
                        _line.Length--; //tolerate "\r\n" from hand-written input
                    }
                    return _line.ToString();
                }
            }
        }

        public void Dispose()
        {
            //_sendLock is deliberately not disposed: a sender waiting on a disposed SemaphoreSlim would never wake up
            _reader.Dispose();
            try
            {
                _writer.Dispose(); //flushes; throws if the socket is already broken
            }
            catch (IOException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }
}
