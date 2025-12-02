using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SecureApp.Controllers;
using SecureApp.Model;
using SecureChat.Data;
using SecureChat.Model.DTOs;


namespace SecureChat.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;

        public MessageService(AppDbContext context)
        {
            _context = context;
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

            return message;
        }
       
    }
}