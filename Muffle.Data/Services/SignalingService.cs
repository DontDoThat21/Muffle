using System.Net.WebSockets;
using System.Text;

namespace Muffle.Data.Services
{
    public class SignalingService
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

        public async Task<string> ReceiveMessageAsync()
        {
            var buffer = new byte[1024];
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }
    }
}
