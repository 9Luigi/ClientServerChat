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
        static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            AllowOutOfOrderMetadataProperties = true, //"type" doesn't have to be the first field
            RespectRequiredConstructorParameters = true //a missing field is a protocol error, not a silent null
        };

        static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        readonly StreamReader _reader;
        readonly StreamWriter _writer;
        readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        public ChatConnection(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            _reader = new StreamReader(stream, Utf8NoBom, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true);
            _writer = new StreamWriter(stream, Utf8NoBom, bufferSize: 4096, leaveOpen: true) { NewLine = "\n" };
        }

        public async Task SendAsync(ChatMessage message, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);
            string json = JsonSerializer.Serialize(message, JsonOptions); //escapes '\n' inside strings, so the line stays one line

            await _sendLock.WaitAsync(cancellationToken);
            try //the lock keeps lines from two threads from interleaving
            {
                await _writer.WriteLineAsync(json.AsMemory(), cancellationToken);
                await _writer.FlushAsync(cancellationToken);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        /// <summary>Waits for the next message. Returns null when the other side has closed the connection.</summary>
        /// <exception cref="InvalidDataException">The received line is not a valid message.</exception>
        public async Task<ChatMessage> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            string line = await _reader.ReadLineAsync(cancellationToken);
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

        public void Dispose()
        {
            _reader.Dispose();
            _writer.Dispose();
            _sendLock.Dispose();
        }
    }
}
