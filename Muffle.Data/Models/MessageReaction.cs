namespace Muffle.Data.Models
{
    public class MessageReaction
    {
        public int ReactionId { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public string Emoji { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
