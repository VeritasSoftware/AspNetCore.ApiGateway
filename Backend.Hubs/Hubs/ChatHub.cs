using Backend.Hubs.Hubs.Requests;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Backend.Hubs.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Request request)
        {
            await base.Clients.All.SendAsync("ReceiveMessage", request.Name, request.Message);
        }
    }
}
