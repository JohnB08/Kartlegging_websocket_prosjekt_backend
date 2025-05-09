using kartlegging_websocket_prosjekt_backend.Interfaces;
using kartlegging_websocket_prosjekt_backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<INameHandler, NameHandler>();
builder.Services.AddSingleton<IConnectionHandler, ConnectionHandler>();
builder.Services.AddLogging();
builder.Services.AddCors();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseWebSockets(new WebSocketOptions{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
    AllowedOrigins = {"*"}
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/name", (INameHandler handler) => handler.GenerateInitialUserName());
app.MapGet("/name/{name}", (INameHandler handler, string name) => handler.RerollName(name));

app.Map("/{name}", async (HttpContext context, string? name) => {
    if(!context.WebSockets.IsWebSocketRequest || string.IsNullOrWhiteSpace(name))
    {
        context.Response.StatusCode = 400;
        return;
    }
    var connectionHandler = context.RequestServices.GetService<IConnectionHandler>();
    var nameHandler = context.RequestServices.GetService<INameHandler>();
    var logger = context.RequestServices.GetService<ILogger<Program>>();
    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    var connection = new WebSocketConnection(name, webSocket);
    connectionHandler!.AddConnection(connection);
    try {
        logger!.LogInformation($"Recieved a connection from {name}");
        await connectionHandler.RecieveMessageAsync(connection, nameHandler!);
    }
    finally
    {
        connectionHandler.RemoveConnection(name);
    }
});

app.UseHttpsRedirection();

app.Run();
