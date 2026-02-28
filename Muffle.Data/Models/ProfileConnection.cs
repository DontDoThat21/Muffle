namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents a connection to an external service (Steam, Twitch, etc.)
    /// </summary>
    public class ProfileConnection
    {
        public int ConnectionId { get; set; }
        public int UserId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceUsername { get; set; } = string.Empty;
        public string? ServiceUrl { get; set; }
        public bool IsVisible { get; set; } = true;
        public DateTime ConnectedAt { get; set; }
    }
}
