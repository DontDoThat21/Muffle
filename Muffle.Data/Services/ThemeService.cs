using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing user theme preferences (light/dark mode, custom themes)
    /// </summary>
    public static class ThemeService
    {
        /// <summary>
        /// Available theme names
        /// </summary>
        public static readonly string[] AvailableThemes = new[]
        {
            "Dark", "Light", "Midnight", "Forest", "Ocean"
        };

        /// <summary>
        /// Available accent colors
        /// </summary>
        public static readonly Dictionary<string, string> AccentColors = new()
        {
            { "Blurple", "#7289DA" },
            { "Green", "#43B581" },
            { "Red", "#F04747" },
            { "Yellow", "#FAA61A" },
            { "Fuchsia", "#EB459E" },
            { "White", "#FFFFFF" },
            { "Purple", "#9B59B6" },
            { "Teal", "#1ABC9C" }
        };

        /// <summary>
        /// Get theme preferences for a user. Returns defaults if none saved.
        /// </summary>
        public static UserThemePreference GetThemePreference(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "SELECT * FROM UserThemePreferences WHERE UserId = @UserId;";
                var pref = connection.QueryFirstOrDefault<UserThemePreference>(query, new { UserId = userId });

                return pref ?? new UserThemePreference { UserId = userId };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting theme preference: {ex.Message}");
                return new UserThemePreference { UserId = userId };
            }
        }

        /// <summary>
        /// Save theme preferences for a user (upsert)
        /// </summary>
        public static bool SaveThemePreference(int userId, string themeName, bool isDarkMode, string accentColor)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"
                    INSERT INTO UserThemePreferences (UserId, ThemeName, IsDarkMode, AccentColor)
                    VALUES (@UserId, @ThemeName, @IsDarkMode, @AccentColor)
                    ON CONFLICT(UserId) DO UPDATE SET
                        ThemeName = @ThemeName,
                        IsDarkMode = @IsDarkMode,
                        AccentColor = @AccentColor;";

                connection.Execute(query, new
                {
                    UserId = userId,
                    ThemeName = themeName,
                    IsDarkMode = isDarkMode,
                    AccentColor = accentColor
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving theme preference: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Toggle between dark and light mode for a user
        /// </summary>
        public static bool ToggleDarkMode(int userId)
        {
            var pref = GetThemePreference(userId);
            return SaveThemePreference(userId, pref.IsDarkMode ? "Light" : "Dark", !pref.IsDarkMode, pref.AccentColor);
        }

        /// <summary>
        /// Update just the accent color
        /// </summary>
        public static bool SetAccentColor(int userId, string accentColor)
        {
            var pref = GetThemePreference(userId);
            return SaveThemePreference(userId, pref.ThemeName, pref.IsDarkMode, accentColor);
        }
    }
}
