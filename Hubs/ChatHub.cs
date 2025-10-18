
using System.Net.Sockets;
using Microsoft.AspNetCore.SignalR;


namespace SecureChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string sender, string receiver, string message)
        {
            // Send the message to the receiver's group(or all clients for now)
            await Clients.All.SendAsync("ReceiveMessage", sender, receiver, message);
        }
        public override async Task OnConnectedAsync()
        {
        
            await base.OnConnectedAsync();
        }
    }
}