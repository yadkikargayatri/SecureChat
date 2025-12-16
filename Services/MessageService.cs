using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SecureApp.Controllers;
using SecureApp.Model;
using SecureChat.Data;
using SecureChat.Model.DTOs;
using Microsoft.AspNetCore.SignalR;
using SecureChat.Hubs;



namespace SecureChat.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hub;

        public MessageService(AppDbContext context, IHubContext<ChatHub> hub)
        {
            _context = context;
            _hub = hub;
        }

       public async Task<IEnumerable<Message>> GetMessageHistoryAsync(int user1Id, int user2Id)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                            (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .OrderBy(m => m.Timestamp)
                .Include(m => m.Sender)
                .Include(m=> m.Receiver)
                .ToListAsync();
        }

        public async Task<Message> SendMessageAsync(SendMessageDto dto)
        {
            var message = new Message
            {
                SenderId = dto.SenderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // await _hub.Clients.User(dto.ReceiverId.ToString())
            //     .SendAsync("ReceiveMessage", new
            //     {
            //         message.SenderId,
            //         message.ReceiverId,
            //         message.Timestamp,
            //         message.Content
            //     });

            // return message;
               // Broadcast to receiver in realtime using user identifier
            await _hub.Clients.User(dto.ReceiverId.ToString()).SendAsync("ReceiveMessage", dto.SenderId.ToString(), dto.Content);

            // Optionally notify sender about success or message id
            await _hub.Clients.User(dto.SenderId.ToString()).SendAsync("MessageSentConfirmation", message.Id);

            return message;
        }
       
   }
}