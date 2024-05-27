using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Muffle.Data.Services
{
    public class WebSocketService
    {
        private readonly ClientWebSocket _webSocket = new ClientWebSocket();

        public async Task ConnectAsync(Uri uri)
        {
            await _webSocket.ConnectAsync(uri, CancellationToken.None);
        }

        public async Task SendMessageAsync(string message)
        {
            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<string> ReceiveMessageAsync()
        {
            var buffer = new ArraySegment<byte>(new byte[4096]);
            var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
            return Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
        }

        public async Task CloseAsync()
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
    }
}
