using Microsoft.AspNetCore.SignalR;
using SecureChat.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SecureChat.Hubs
{
[Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }
        // Keep track of connected users: userId -> connectionId
        private static readonly Dictionary<string, string> ConnectedUsers = new();// maps user IDs tp SignalR connection IDs

        // When a user connects, store their connection ID
        public override async Task OnConnectedAsync()  // tracks when users join
        {
            var userId = Context.UserIdentifier; // Make sure JWT NameIdentifier is set
            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers[userId] = Context.ConnectionId;
            }

            await base.OnConnectedAsync();
        }

        // When a user disconnects, remove them from the dictionary
        public override async Task OnDisconnectedAsync(Exception? exception)  // tracks when users leave
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers.Remove(userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Send a message to a specific user
        public async Task SendMessage(string receiverId, string message)

        {
            var senderName = Context.User?.Identity?.Name ?? "Unknown";
            if(string.IsNullOrWhiteSpace(receiverId) || string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            // var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // var senderName = Context.User?.Identity?.Name ?? "Unknown";

            // if (receiverId == null || message == null)
            //     return;

            // Send message to the specific user
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderName, message);

            await Clients.Caller.SendAsync("ReceiveMessage", senderName, message);
        }
        //     var senderId = Context.UserIdentifier;
        //     if (string.IsNullOrEmpty(senderId))
        //         return;

        //     // Optionally get sender username
        //     string senderUsername = Context.User?.Identity?.Name ?? senderId;

        //     // Send message to receiver if connected
        //     if (ConnectedUsers.TryGetValue(receiverId, out var connectionId))
        // {
        //      await Clients.Client(connectionId)
        //     .SendAsync("ReceiveMessage", senderUsername, message);
        }

    // // Echo to sender
    //     await Clients.Caller
    //     .SendAsync("ReceiveMessage", senderUsername, message);
    // }

            // var senderId = Context.UserIdentifier;
            // if (string.IsNullOrEmpty(senderId))
            //     return;

            // // Save message to database if needed (optional)

            // // Send message to receiver if connected
            // if (ConnectedUsers.TryGetValue(receiverId, out var connectionId))
            // {
            //     await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderId, message);
            // }

            // // Optionally, send message back to sender for confirmation
            // await Clients.Caller.SendAsync("ReceiveMessage", senderId, message);
        }
    