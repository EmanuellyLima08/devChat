using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Habilita WebSockets
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
};

app.UseWebSockets(webSocketOptions);

// Lista de clientes conectados
List<WebSocket> connectedClients = new();

// Endpoint de WebSocket
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();
        connectedClients.Add(socket);

        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        while (socket.State == WebSocketState.Open)
        {
            result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Console.WriteLine($"Mensagem recebida: {message}");

            // Broadcast para todos os clientes
            foreach (var client in connectedClients.ToList())
            {
                if (client.State == WebSocketState.Open)
                {
                    var data = Encoding.UTF8.GetBytes(message);
                    await client.SendAsync(
                        new ArraySegment<byte>(data, 0, data.Length),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                }
            }
        }

        connectedClients.Remove(socket);
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Conexão encerrada", CancellationToken.None);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();
