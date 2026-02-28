using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing per-user developer settings
    /// </summary>
    public static class DeveloperSettingsService
    {
        /// <summary>
        /// Retrieve developer settings for a user. Returns defaults if none are saved.
        /// </summary>
        public static DeveloperSettings GetDeveloperSettings(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "SELECT * FROM DeveloperSettings WHERE UserId = @UserId;";
                var settings = connection.QueryFirstOrDefault<DeveloperSettings>(query, new { UserId = userId });

                return settings ?? new DeveloperSettings { UserId = userId };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting developer settings: {ex.Message}");
                return new DeveloperSettings { UserId = userId };
            }
        }

        /// <summary>
        /// Save (upsert) developer settings for a user.
        /// </summary>
        public static bool SaveDeveloperSettings(DeveloperSettings settings)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"
                    INSERT INTO DeveloperSettings (UserId, DebugMode, WebSocketInspector, EnableDevTools)
                    VALUES (@UserId, @DebugMode, @WebSocketInspector, @EnableDevTools)
                    ON CONFLICT(UserId) DO UPDATE SET
                        DebugMode          = @DebugMode,
                        WebSocketInspector = @WebSocketInspector,
                        EnableDevTools     = @EnableDevTools;";

                connection.Execute(query, settings);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving developer settings: {ex.Message}");
                return false;
            }
        }
    }
}
