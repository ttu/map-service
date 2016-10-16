using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MapApi
{
    public class WebSocketMiddleware
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>> _sockets =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>>();

        private MessageHub _hub;
        private CancellationToken _token = CancellationToken.None;

        public WebSocketMiddleware(MessageHub hub)
        {
            _hub = hub;
            _hub.Updated += (s, data) =>
            {
                if (!_sockets.ContainsKey(data.Item1))
                    return;

                _sockets[data.Item1].Values
                    .Where(socket => socket.State == WebSocketState.Open)
                    .ToList()
                    .ForEach(async socket =>
                    {
                        var type = WebSocketMessageType.Text;
                        var text = Encoding.UTF8.GetBytes(data.Item2);
                        var buffer = new ArraySegment<Byte>(text);
                        await socket.SendAsync(buffer, type, true, _token);
                    });
            };
        }

        public async Task Handle(HttpContext http, Func<Task> next)
        {
            if (http.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await http.WebSockets.AcceptWebSocketAsync();

                while (webSocket.State == WebSocketState.Open)
                {
                    var buffer = new ArraySegment<Byte>(new Byte[1024]);
                    var received = await webSocket.ReceiveAsync(buffer, _token);

                    switch (received.MessageType)
                    {
                        case WebSocketMessageType.Text:
                            // Expect join xxxx from the client
                            var request = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                            if (request.StartsWith("join"))
                            {
                                request = request.IndexOf("\0") > -1 ? request.Substring(0, request.IndexOf("\0")) : request;

                                var key = request.Split(' ')[1];
                                if (!_sockets.ContainsKey(key))
                                    _sockets.TryAdd(key, new ConcurrentDictionary<string, WebSocket>());

                                _sockets[key].TryAdd(webSocket.GetHashCode().ToString(), webSocket);
                            }
                            break;
                        case WebSocketMessageType.Close:
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _token);
                            WebSocket toRemove;
                            foreach (var dict in _sockets.Values)
                                dict.TryRemove(webSocket.GetHashCode().ToString(), out toRemove);
                            break;
                    }
                }
            }
            else
            {
                await next();
            }
        }

        private async Task SendData(WebSocket socket, string json)
        {
            var type = WebSocketMessageType.Text;
            var text = Encoding.UTF8.GetBytes(json);
            var buffer = new ArraySegment<Byte>(text);
            await socket.SendAsync(buffer, type, true, _token);
        }
    }
}