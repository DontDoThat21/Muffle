using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing per-user privacy and safety settings.
    /// </summary>
    public static class PrivacySettingsService
    {
        /// <summary>
        /// Display labels for <see cref="DmPrivacyLevel"/> values.
        /// </summary>
        public static List<string> DmPrivacyOptions { get; } = new() { "Everyone", "Friends Only", "Nobody" };

        /// <summary>
        /// Display labels for <see cref="FriendRequestFilterLevel"/> values.
        /// </summary>
        public static List<string> FriendRequestFilterOptions { get; } = new() { "Everyone", "Friends of Friends", "Nobody" };

        /// <summary>
        /// Display labels for <see cref="ContentFilterLevel"/> values.
        /// </summary>
        public static List<string> ContentFilterOptions { get; } = new() { "Off", "Medium", "High" };

        /// <summary>
        /// Retrieve privacy settings for a user. Returns defaults if none are saved.
        /// </summary>
        public static PrivacySettings GetPrivacySettings(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "SELECT * FROM PrivacySettings WHERE UserId = @UserId;";
                var settings = connection.QueryFirstOrDefault<PrivacySettings>(query, new { UserId = userId });

                return settings ?? new PrivacySettings { UserId = userId };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting privacy settings: {ex.Message}");
                return new PrivacySettings { UserId = userId };
            }
        }

        /// <summary>
        /// Save (upsert) privacy settings for a user.
        /// </summary>
        public static bool SavePrivacySettings(PrivacySettings settings)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"
                    INSERT INTO PrivacySettings (UserId, DmPrivacy, FriendRequestFilter, ContentFilter)
                    VALUES (@UserId, @DmPrivacy, @FriendRequestFilter, @ContentFilter)
                    ON CONFLICT(UserId) DO UPDATE SET
                        DmPrivacy           = @DmPrivacy,
                        FriendRequestFilter = @FriendRequestFilter,
                        ContentFilter       = @ContentFilter;";

                connection.Execute(query, settings);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving privacy settings: {ex.Message}");
                return false;
            }
        }
    }
}
