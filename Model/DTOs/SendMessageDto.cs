using System.Security.Cryptography.X509Certificates;

namespace SecureChat.Model.DTOs
{
    public class SendMessageDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
