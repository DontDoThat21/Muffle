using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing profile connections to external services (Steam, Twitch, etc.)
    /// </summary>
    public static class ProfileConnectionService
    {
        /// <summary>
        /// Supported external services
        /// </summary>
        public static readonly string[] SupportedServices = new[]
        {
            "Steam", "Twitch", "YouTube", "Twitter", "Spotify",
            "Xbox Live", "PlayStation Network", "Battle.net",
            "GitHub", "Reddit"
        };

        /// <summary>
        /// Add a connection to an external service
        /// </summary>
        public static ProfileConnection? AddConnection(int userId, string serviceName, string serviceUsername, string? serviceUrl = null)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                // Check if this service is already connected
                var checkQuery = @"SELECT COUNT(*) FROM ProfileConnections 
                                   WHERE UserId = @UserId AND ServiceName = @ServiceName;";
                var exists = connection.QueryFirst<int>(checkQuery, new { UserId = userId, ServiceName = serviceName });

                if (exists > 0)
                {
                    Console.WriteLine($"Service '{serviceName}' is already connected");
                    return null;
                }

                var insertQuery = @"
                    INSERT INTO ProfileConnections (UserId, ServiceName, ServiceUsername, ServiceUrl, IsVisible, ConnectedAt)
                    VALUES (@UserId, @ServiceName, @ServiceUsername, @ServiceUrl, 1, @ConnectedAt);
                    SELECT last_insert_rowid();";

                var connectionId = connection.QueryFirst<int>(insertQuery, new
                {
                    UserId = userId,
                    ServiceName = serviceName,
                    ServiceUsername = serviceUsername,
                    ServiceUrl = serviceUrl,
                    ConnectedAt = DateTime.Now
                });

                return new ProfileConnection
                {
                    ConnectionId = connectionId,
                    UserId = userId,
                    ServiceName = serviceName,
                    ServiceUsername = serviceUsername,
                    ServiceUrl = serviceUrl,
                    IsVisible = true,
                    ConnectedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding connection: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Remove a connection to an external service
        /// </summary>
        public static bool RemoveConnection(int connectionId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "DELETE FROM ProfileConnections WHERE ConnectionId = @ConnectionId;";
                connection.Execute(query, new { ConnectionId = connectionId });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing connection: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all connections for a user
        /// </summary>
        public static List<ProfileConnection> GetConnections(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"SELECT * FROM ProfileConnections 
                              WHERE UserId = @UserId 
                              ORDER BY ConnectedAt DESC;";

                return connection.Query<ProfileConnection>(query, new { UserId = userId }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting connections: {ex.Message}");
                return new List<ProfileConnection>();
            }
        }

        /// <summary>
        /// Get only visible connections for a user (for public profile display)
        /// </summary>
        public static List<ProfileConnection> GetVisibleConnections(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"SELECT * FROM ProfileConnections 
                              WHERE UserId = @UserId AND IsVisible = 1
                              ORDER BY ConnectedAt DESC;";

                return connection.Query<ProfileConnection>(query, new { UserId = userId }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting visible connections: {ex.Message}");
                return new List<ProfileConnection>();
            }
        }

        /// <summary>
        /// Toggle visibility of a connection
        /// </summary>
        public static bool SetConnectionVisibility(int connectionId, bool isVisible)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "UPDATE ProfileConnections SET IsVisible = @IsVisible WHERE ConnectionId = @ConnectionId;";
                connection.Execute(query, new { ConnectionId = connectionId, IsVisible = isVisible });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling connection visibility: {ex.Message}");
                return false;
            }
        }
    }
}
