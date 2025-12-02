using Microsoft.AspNetCore.SignalR;
using SecureChat.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SecureApp.Model;

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
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var senderName = Context.User?.Identity?.Name ?? "Unknown";

            if (string.IsNullOrWhiteSpace(senderId) ||
                string.IsNullOrWhiteSpace(receiverId) ||
                string.IsNullOrWhiteSpace(message))
                return;

            // 1️⃣ Convert string IDs → int IDs (your DB uses int)
            if (!int.TryParse(senderId, out int senderIntId) ||
                !int.TryParse(receiverId, out int receiverIntId))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "Invalid user ID");
                return;
            }

            // 2️⃣ Save message to database
            var msgEntity = new Message
            {
                SenderId = senderIntId,
                ReceiverId = receiverIntId,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(msgEntity);
            await _context.SaveChangesAsync();

            // 3️⃣ Send to receiver (real-time)
            await Clients.User(receiverId)
                .SendAsync("ReceiveMessage", senderName, message);

            // 4️⃣ Optional: echo back to sender (for UI)
            await Clients.Caller
                .SendAsync("MessageSentConfirmation", msgEntity.Id);
        }

    }
       
}
    