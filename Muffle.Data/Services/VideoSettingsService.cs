using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing per-user video settings
    /// </summary>
    public static class VideoSettingsService
    {
        /// <summary>
        /// Available camera device names. "Default" is always included.
        /// Platform-specific enumeration can be injected at startup.
        /// </summary>
        public static List<string> AvailableCameras { get; set; } = new() { "Default" };

        /// <summary>
        /// Supported video resolutions.
        /// </summary>
        public static List<string> AvailableResolutions { get; } = new()
        {
            "3840x2160",
            "1920x1080",
            "1280x720",
            "854x480",
            "640x360"
        };

        /// <summary>
        /// Supported frame rates.
        /// </summary>
        public static List<int> AvailableFpsOptions { get; } = new() { 15, 30, 60 };

        /// <summary>
        /// Retrieve video settings for a user. Returns defaults if none are saved.
        /// </summary>
        public static VideoSettings GetVideoSettings(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "SELECT * FROM VideoSettings WHERE UserId = @UserId;";
                var settings = connection.QueryFirstOrDefault<VideoSettings>(query, new { UserId = userId });

                return settings ?? new VideoSettings { UserId = userId };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting video settings: {ex.Message}");
                return new VideoSettings { UserId = userId };
            }
        }

        /// <summary>
        /// Save (upsert) video settings for a user.
        /// </summary>
        public static bool SaveVideoSettings(VideoSettings settings)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"
                    INSERT INTO VideoSettings (UserId, Camera, Resolution, Fps)
                    VALUES (@UserId, @Camera, @Resolution, @Fps)
                    ON CONFLICT(UserId) DO UPDATE SET
                        Camera     = @Camera,
                        Resolution = @Resolution,
                        Fps        = @Fps;";

                connection.Execute(query, settings);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving video settings: {ex.Message}");
                return false;
            }
        }
    }
}
