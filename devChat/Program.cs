using ServidorChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o SignalR
builder.Services.AddSignalR();

// Configura o CORS para permitir credenciais e a origem desejada
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://192.168.0.109:5000") // Permite frontend local e externo
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Permite credenciais (cookies, headers de autenticação, etc)
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica o CORS
app.UseCors("AllowSpecificOrigin");  // Aplica a política CORS com credenciais

app.UseSwagger();
app.UseSwaggerUI();

// Endpoint raiz para ver se o servidor está online
app.MapGet("/", () => "✅ Servidor está rodando perfeitamente!");

// Endpoint do SignalR
app.MapHub<ChatHub>("/chatHub");

app.Run();
