using System;
using System.Text.Json.Serialization;

namespace SecureApp.Model
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public required string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

         // navigation properties
    [JsonIgnore]
    public User? Sender { get; set; }   // nullable

    [JsonIgnore]
    public User? Receiver { get; set; } // nullable
    }
 }