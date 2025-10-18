using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureApp.Model;
using SecureChat.Data;
using System.Security.Claims;

namespace SecureApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // <-- This protects all endpoints
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
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
    }

    public class CreateMessageDto
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; } = null!;
    }
}
