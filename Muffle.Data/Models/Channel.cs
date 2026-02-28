namespace Muffle.Data.Models
{
    /// <summary>
    /// Type of channel
    /// </summary>
    public enum ChannelType
    {
        Text = 0,
        Voice = 1,
        Video = 2
    }

    /// <summary>
    /// Represents a channel within a server
    /// </summary>
    public class Channel
    {
        public int ChannelId { get; set; }
        public int ServerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ChannelType Type { get; set; }
        public int Position { get; set; } // For ordering channels
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; } // UserId who created the channel
        public string? IconUrl { get; set; }

        /// <summary>
        /// Gets a formatted display name with type indicator
        /// </summary>
        public string DisplayName => Type switch
        {
            ChannelType.Text => $"# {Name}",
            ChannelType.Voice => $"🔊 {Name}",
            ChannelType.Video => $"📹 {Name}",
            _ => Name
        };
    }
}
