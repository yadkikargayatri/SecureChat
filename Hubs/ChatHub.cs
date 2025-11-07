using Microsoft.AspNetCore.SignalR;

namespace SecureChat.Hubs
{
    public class ChatHub : Hub
    {
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
            var senderId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(senderId))
                return;

            // Save message to database if needed (optional)

            // Send message to receiver if connected
            if (ConnectedUsers.TryGetValue(receiverId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderId, message);
            }

            // Optionally, send message back to sender for confirmation
            await Clients.Caller.SendAsync("ReceiveMessage", senderId, message);
        }
    }
}
