namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents a user's theme preferences
    /// </summary>
    public class UserThemePreference
    {
        public int UserId { get; set; }
        public string ThemeName { get; set; } = "Dark";
        public bool IsDarkMode { get; set; } = true;
        public string AccentColor { get; set; } = "#7289DA";
    }
}
