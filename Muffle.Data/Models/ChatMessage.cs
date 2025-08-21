using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muffle.Data.Models
{
    public enum MessageType
    {
        Text,
        Image
    }

    public class ChatMessage
    {
        public string? Content { get; set; }
        public User? Sender { get; set; }
        public DateTime Timestamp { get; set; }
        public MessageType Type { get; set; } = MessageType.Text;
        public byte[]? ImageData { get; set; }
        public string? ImageFileName { get; set; }
        // Add other properties as needed
    }
}
