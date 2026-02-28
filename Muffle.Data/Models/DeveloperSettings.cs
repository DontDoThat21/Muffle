namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents a user's developer settings
    /// </summary>
    public class DeveloperSettings
    {
        public int UserId { get; set; }
        public bool DebugMode { get; set; } = false;
        public bool WebSocketInspector { get; set; } = false;
        public bool EnableDevTools { get; set; } = false;
    }
}
