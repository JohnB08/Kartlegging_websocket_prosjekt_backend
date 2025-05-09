using kartlegging_websocket_prosjekt_backend.Models;

namespace kartlegging_websocket_prosjekt_backend.Interfaces;


public interface IConnectionHandler
{
    void AddConnection(WebSocketConnection connection);
    void RemoveConnection(string userName);

    Task BroadcastAsync(string message, string? exceptionUserName = null);

    Task RecieveMessageAsync(WebSocketConnection connection, INameHandler handler);
}