using System;
using System.Text.Json.Serialization;

namespace Muffle.Data.Models
{
    public class MessageWrapper
    {
        public MessageType Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageData { get; set; }
        public DateTime Timestamp { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int SenderId { get; set; }
        
        // WebRTC signaling fields
        public string? SdpOffer { get; set; }
        public string? SdpAnswer { get; set; }
        public string? IceCandidateData { get; set; }
        public string? CallType { get; set; } // "voice" or "video"
        public int TargetUserId { get; set; }
    }
}