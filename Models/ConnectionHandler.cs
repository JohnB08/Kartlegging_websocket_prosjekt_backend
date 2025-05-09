using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using kartlegging_websocket_prosjekt_backend.Interfaces;

namespace kartlegging_websocket_prosjekt_backend.Models;

public class ConnectionHandler : IConnectionHandler
{
    private readonly ConcurrentDictionary<string , WebSocketConnection> _connections = [];

    public void AddConnection(WebSocketConnection connection)
    {
        _connections.TryAdd(connection.UserName, connection);
    }

    public async Task BroadcastAsync(string message, string? exceptionUserName = null)
    {
        foreach (var connection in _connections.Values)
        {
            if (string.Equals(exceptionUserName, connection.UserName) || connection.Socket.State != WebSocketState.Open) continue;
            var bytes = Encoding.UTF8.GetBytes(message);   
            await connection.Socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public async Task RecieveMessageAsync(WebSocketConnection connection, INameHandler handler)
    {
        var buffer = new ArraySegment<byte>(new byte[1024]);
        var recievedResult = await connection.Socket.ReceiveAsync(buffer, CancellationToken.None);
        while(!recievedResult.CloseStatus.HasValue)
        {
            var recievedMessage = Encoding.UTF8.GetString(buffer.Array!, 0, recievedResult.Count);
            handler.NameActive(connection.UserName);
            await BroadcastAsync(recievedMessage, connection.UserName);
            recievedResult = await connection.Socket.ReceiveAsync(buffer, CancellationToken.None);
        }
        await connection.Socket.CloseAsync(recievedResult.CloseStatus.Value, recievedResult.CloseStatusDescription, CancellationToken.None);
    }

    public void RemoveConnection(string userName)
    {
        _connections.TryRemove(userName, out _ );
    }
}
