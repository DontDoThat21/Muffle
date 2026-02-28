namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents a user's video settings
    /// </summary>
    public class VideoSettings
    {
        public int UserId { get; set; }
        public string Camera { get; set; } = "Default";
        public string Resolution { get; set; } = "1280x720";
        public int Fps { get; set; } = 30;
    }
}
