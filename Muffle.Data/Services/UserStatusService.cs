using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing user presence status (online, away, DND, invisible)
    /// and custom status messages
    /// </summary>
    public static class UserStatusService
    {
        /// <summary>
        /// Update the user's presence status
        /// </summary>
        public static bool UpdateStatus(int userId, UserStatus status)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "UPDATE Users SET Status = @Status WHERE UserId = @UserId;";
                connection.Execute(query, new { UserId = userId, Status = (int)status });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating status: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get the user's current status
        /// </summary>
        public static UserStatus GetStatus(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "SELECT Status FROM Users WHERE UserId = @UserId;";
                var status = connection.QueryFirstOrDefault<int?>(query, new { UserId = userId });
                return status.HasValue ? (UserStatus)status.Value : UserStatus.Online;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting status: {ex.Message}");
                return UserStatus.Online;
            }
        }

        /// <summary>
        /// Set a custom status message (e.g., "Playing X", "Listening to Y")
        /// </summary>
        public static bool SetCustomStatus(int userId, string? statusText, string? statusEmoji = null)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"UPDATE Users 
                              SET CustomStatusText = @CustomStatusText, 
                                  CustomStatusEmoji = @CustomStatusEmoji 
                              WHERE UserId = @UserId;";

                connection.Execute(query, new
                {
                    UserId = userId,
                    CustomStatusText = statusText,
                    CustomStatusEmoji = statusEmoji
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting custom status: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Clear the custom status message
        /// </summary>
        public static bool ClearCustomStatus(int userId)
        {
            return SetCustomStatus(userId, null, null);
        }

        /// <summary>
        /// Update whether the user's online status is visible to others
        /// </summary>
        public static bool SetShowOnlineStatus(int userId, bool showOnlineStatus)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "UPDATE Users SET ShowOnlineStatus = @ShowOnlineStatus WHERE UserId = @UserId;";
                connection.Execute(query, new { UserId = userId, ShowOnlineStatus = showOnlineStatus });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating show online status: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get whether the user's online status is visible to others
        /// </summary>
        public static bool GetShowOnlineStatus(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "SELECT ShowOnlineStatus FROM Users WHERE UserId = @UserId;";
                return connection.QueryFirstOrDefault<bool>(query, new { UserId = userId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting show online status: {ex.Message}");
                return true;
            }
        }

        /// <summary>
        /// Get the display name for a user status
        /// </summary>
        public static string GetStatusDisplayName(UserStatus status)
        {
            return status switch
            {
                UserStatus.Online => "Online",
                UserStatus.Away => "Away",
                UserStatus.DoNotDisturb => "Do Not Disturb",
                UserStatus.Invisible => "Invisible",
                _ => "Online"
            };
        }

        /// <summary>
        /// Get the color associated with a user status
        /// </summary>
        public static string GetStatusColor(UserStatus status)
        {
            return status switch
            {
                UserStatus.Online => "#43B581",
                UserStatus.Away => "#FAA61A",
                UserStatus.DoNotDisturb => "#F04747",
                UserStatus.Invisible => "#747F8D",
                _ => "#43B581"
            };
        }
    }
}
