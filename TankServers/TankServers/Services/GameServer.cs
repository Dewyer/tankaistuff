using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TankServers.Services
{
    public interface IGameServer
    {
        void OnNewConnection(HttpContext context, WebSocket webSocket);
    }

    public class GameServer : IGameServer
    {

        public void OnNewConnection(HttpContext context, WebSocket webSocket)
        {
            Task.Run( async ()=>
            {
                await LoopWithConnection(context, webSocket);
            });
        }

        private async Task LoopWithConnection(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = null;
            try
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            }
            catch (Exception e)
            {
                var a = e.Message;
            }
            string message1= Encoding.ASCII.GetString(buffer);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.ASCII.GetString(buffer);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
