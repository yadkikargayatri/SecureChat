using SecureApp.Model;
using SecureChat.Model.DTOs;

namespace SecureChat.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetMessageHistoryAsync(int user1Id, int user2Id);
        Task<Message> SendMessageAsync(SendMessageDto dto);
    }
}