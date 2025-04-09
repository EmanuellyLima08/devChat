using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ServidorChat.Hubs
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, string> UsuariosConectados = new();

        public Task DefinirUsuario(string nome)
        {
            UsuariosConectados[Context.ConnectionId] = nome;

            Console.WriteLine($"✅ {nome} ({Context.ConnectionId}) entrou no chat.");

            return Task.CompletedTask;
        }

        public async Task EnviarMensagem(string mensagem)
        {
            if (UsuariosConectados.TryGetValue(Context.ConnectionId, out var nome))
            {
                Console.WriteLine($"📨 {nome} enviou: {mensagem}");

                await Clients.All.SendAsync("ReceberMensagem", nome, mensagem);
            }
            else
            {
                Console.WriteLine($"⚠️ Mensagem recebida de uma conexão não identificada: {Context.ConnectionId}");
                await Clients.Caller.SendAsync("ReceberMensagem", "Sistema", "Erro: nome de usuário não definido.");
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (UsuariosConectados.TryRemove(Context.ConnectionId, out var nome))
            {
                Console.WriteLine($"❌ {nome} ({Context.ConnectionId}) saiu do chat.");
            }
            else
            {
                Console.WriteLine($"❌ Conexão desconhecida {Context.ConnectionId} foi desconectada.");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
