using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public class SignalingService : ISignalingService
    {
        private readonly ClientWebSocket _webSocket;

        public SignalingService()
        {
            _webSocket = new ClientWebSocket();
        }

        public async Task ConnectAsync(Uri serverUri)
        {
            await _webSocket.ConnectAsync(serverUri, CancellationToken.None);
        }

        public async Task SendMessageAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(bytes);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendMessageWrapperAsync(MessageWrapper messageWrapper)
        {
            var json = JsonSerializer.Serialize(messageWrapper);
            var bytes = Encoding.UTF8.GetBytes(json);
            var buffer = new ArraySegment<byte>(bytes);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<string> ReceiveMessageAsync()
        {
            var buffer = new byte[4096]; // Increased buffer size for images
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        public async Task<MessageWrapper?> ReceiveMessageWrapperAsync()
        {
            try
            {
                var messageJson = await ReceiveMessageAsync();
                return JsonSerializer.Deserialize<MessageWrapper>(messageJson);
            }
            catch
            {
                // If deserialization fails, treat as legacy text message
                return null;
            }
        }

        public event Action<string> OnMessageReceived;

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[4096]; // Increased buffer size for images
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                OnMessageReceived?.Invoke(message);
            }
        }
    }
}
