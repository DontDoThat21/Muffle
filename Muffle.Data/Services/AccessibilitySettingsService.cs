using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing per-user accessibility settings
    /// </summary>
    public static class AccessibilitySettingsService
    {
        /// <summary>
        /// Supported font sizes.
        /// </summary>
        public static List<double> AvailableFontSizes { get; } = new() { 12.0, 14.0, 18.0, 22.0 };

        /// <summary>
        /// Retrieve accessibility settings for a user. Returns defaults if none are saved.
        /// </summary>
        public static AccessibilitySettings GetAccessibilitySettings(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "SELECT * FROM AccessibilitySettings WHERE UserId = @UserId;";
                var settings = connection.QueryFirstOrDefault<AccessibilitySettings>(query, new { UserId = userId });

                return settings ?? new AccessibilitySettings { UserId = userId };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting accessibility settings: {ex.Message}");
                return new AccessibilitySettings { UserId = userId };
            }
        }

        /// <summary>
        /// Save (upsert) accessibility settings for a user.
        /// </summary>
        public static bool SaveAccessibilitySettings(AccessibilitySettings settings)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"
                    INSERT INTO AccessibilitySettings (UserId, FontSize, HighContrast, ScreenReader)
                    VALUES (@UserId, @FontSize, @HighContrast, @ScreenReader)
                    ON CONFLICT(UserId) DO UPDATE SET
                        FontSize     = @FontSize,
                        HighContrast = @HighContrast,
                        ScreenReader = @ScreenReader;";

                connection.Execute(query, settings);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving accessibility settings: {ex.Message}");
                return false;
            }
        }
    }
}
