namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents a user's accessibility settings
    /// </summary>
    public class AccessibilitySettings
    {
        public int UserId { get; set; }
        public double FontSize { get; set; } = 14.0;
        public bool HighContrast { get; set; } = false;
        public bool ScreenReader { get; set; } = false;
    }
}
