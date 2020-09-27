using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Backend.Hubs.Hubs
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        public async Task SendMessage(string msg)
        {
            await base.Clients.All.SendAsync("ReceiveMessage", msg);
        }
    }
}
