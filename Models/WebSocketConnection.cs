using System.Net.WebSockets;

namespace kartlegging_websocket_prosjekt_backend.Models;


public class WebSocketConnection(string userName, WebSocket socket)
{
    public string UserName {get;init;} = userName;
    public WebSocket Socket {get;init;} = socket;
}