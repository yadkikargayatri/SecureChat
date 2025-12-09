using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SecureApp.Model;
using SecureChat.Data;
using SecureChat.Model.DTOs;
using SecureChat.Services;
using System.Security.Claims;


namespace SecureApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // <-- This protects all endpoints
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _service;
        private readonly AppDbContext _context;
        //private readonly IMessageService _messageService;

        public MessagesController(IMessageService service, AppDbContext context, IMessageService messageService)
        {
            _service = service;
            _context = context;
            //_messageService = messageService;
        }

        // GET: api/messages/history?user1={user1Id}&user2={user2Id}
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(int user1Id, int user2Id)
        {
            var history = await _service.GetMessageHistoryAsync(user1Id, user2Id);
            return Ok(history);
        }

        // GET: api/messages
        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Select(m => new
                {
                    m.Id,
                    m.Content,
                    m.Timestamp,
                    Sender = new { m.Sender.Id, m.Sender.Username, m.Sender.Email },
                    Receiver = new { m.Receiver.Id, m.Receiver.Username, m.Receiver.Email }
                })
                .ToListAsync();

            return Ok(messages);
        }

        // POST: api/messages
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto dto)
        {
            // Get the senderId from JWT token claims
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var message = new Message
            {
                Content = dto.Content,
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message.Id,
                message.Content,
                message.Timestamp,
                SenderId = senderId,
                ReceiverId = message.ReceiverId
            });
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            dto.SenderId = senderId;
            
            var message = await _service.SendMessageAsync(dto);
            return Ok(new
            {
                message.Id,
                message.Content,
                message.Timestamp,
                message.SenderId,
                message.ReceiverId
            });
        }

    }
    // public interface IMessageService
    // {
    //     Task<IEnumerable<Message>> GetMessageHistoryAsync(int user1Id, int user2Id);
    // }

    public class CreateMessageDto
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; } = null!;
    }

}
