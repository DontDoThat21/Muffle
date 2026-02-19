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
        Image,
        WebRtcOffer,
        WebRtcAnswer,
        IceCandidate,
        CallInvite,
        CallAccept,
        CallReject,
        CallEnd
    }

    public class ChatMessage
    {
        public string Content { get; set; }
        public User Sender { get; set; }
        public DateTime Timestamp { get; set; }
        public MessageType Type { get; set; } = MessageType.Text;
        public string? ImagePath { get; set; }
        public string? ImageData { get; set; } // Base64 encoded image data for transmission
        // Add other properties as needed
    }
}
