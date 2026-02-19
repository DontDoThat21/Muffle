using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing server channels
    /// </summary>
    public static class ChannelService
    {
        /// <summary>
        /// Create a new channel in a server
        /// </summary>
        /// <param name="serverId">ID of the server</param>
        /// <param name="name">Channel name</param>
        /// <param name="type">Channel type (text/voice/video)</param>
        /// <param name="createdBy">User ID of creator</param>
        /// <param name="description">Optional description</param>
        /// <returns>The created Channel or null if failed</returns>
        public static Channel? CreateChannel(int serverId, string name, ChannelType type, int createdBy, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                // Get the next position for this server
                var getMaxPositionQuery = @"
                    SELECT COALESCE(MAX(Position), -1) FROM Channels 
                    WHERE ServerId = @ServerId;";

                var maxPosition = connection.ExecuteScalar<int>(getMaxPositionQuery, new { ServerId = serverId });
                var newPosition = maxPosition + 1;

                // Insert the channel
                var insertQuery = @"
                    INSERT INTO Channels (ServerId, Name, Description, Type, Position, CreatedAt, CreatedBy)
                    VALUES (@ServerId, @Name, @Description, @Type, @Position, @CreatedAt, @CreatedBy);
                    SELECT last_insert_rowid();";

                var channelId = connection.QuerySingle<long>(insertQuery, new
                {
                    ServerId = serverId,
                    Name = name,
                    Description = description,
                    Type = (int)type,
                    Position = newPosition,
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                });

                return new Channel
                {
                    ChannelId = (int)channelId,
                    ServerId = serverId,
                    Name = name,
                    Description = description,
                    Type = type,
                    Position = newPosition,
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating channel: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get all channels for a server
        /// </summary>
        /// <param name="serverId">Server ID</param>
        /// <returns>List of channels ordered by position</returns>
        public static List<Channel> GetServerChannels(int serverId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"
                    SELECT * FROM Channels 
                    WHERE ServerId = @ServerId 
                    ORDER BY Position ASC;";

                return connection.Query<Channel>(query, new { ServerId = serverId }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting server channels: {ex.Message}");
                return new List<Channel>();
            }
        }

        /// <summary>
        /// Get a specific channel by ID
        /// </summary>
        /// <param name="channelId">Channel ID</param>
        /// <returns>Channel or null if not found</returns>
        public static Channel? GetChannel(int channelId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "SELECT * FROM Channels WHERE ChannelId = @ChannelId;";
                return connection.QueryFirstOrDefault<Channel>(query, new { ChannelId = channelId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting channel: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Update channel information
        /// </summary>
        /// <param name="channelId">Channel ID</param>
        /// <param name="name">New name (optional)</param>
        /// <param name="description">New description (optional)</param>
        /// <returns>True if updated successfully</returns>
        public static bool UpdateChannel(int channelId, string? name = null, string? description = null)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var updates = new List<string>();
                var parameters = new DynamicParameters();
                parameters.Add("ChannelId", channelId);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    updates.Add("Name = @Name");
                    parameters.Add("Name", name);
                }

                if (description != null)
                {
                    updates.Add("Description = @Description");
                    parameters.Add("Description", description);
                }

                if (updates.Count == 0)
                {
                    return false;
                }

                var query = $"UPDATE Channels SET {string.Join(", ", updates)} WHERE ChannelId = @ChannelId;";
                var rowsAffected = connection.Execute(query, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating channel: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete a channel
        /// </summary>
        /// <param name="channelId">Channel ID</param>
        /// <returns>True if deleted successfully</returns>
        public static bool DeleteChannel(int channelId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                // Get channel info for reordering
                var channel = GetChannel(channelId);
                if (channel == null)
                {
                    return false;
                }

                // Delete the channel
                var deleteQuery = "DELETE FROM Channels WHERE ChannelId = @ChannelId;";
                var rowsAffected = connection.Execute(deleteQuery, new { ChannelId = channelId });

                if (rowsAffected > 0)
                {
                    // Reorder remaining channels
                    var reorderQuery = @"
                        UPDATE Channels 
                        SET Position = Position - 1 
                        WHERE ServerId = @ServerId AND Position > @Position;";

                    connection.Execute(reorderQuery, new
                    {
                        ServerId = channel.ServerId,
                        Position = channel.Position
                    });

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting channel: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reorder a channel (change its position)
        /// </summary>
        /// <param name="channelId">Channel ID</param>
        /// <param name="newPosition">New position index</param>
        /// <returns>True if reordered successfully</returns>
        public static bool ReorderChannel(int channelId, int newPosition)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var channel = GetChannel(channelId);
                if (channel == null)
                {
                    return false;
                }

                var oldPosition = channel.Position;

                if (oldPosition == newPosition)
                {
                    return true; // No change needed
                }

                // Shift other channels
                if (newPosition < oldPosition)
                {
                    // Moving up - shift down channels in between
                    var shiftQuery = @"
                        UPDATE Channels 
                        SET Position = Position + 1 
                        WHERE ServerId = @ServerId 
                        AND Position >= @NewPosition 
                        AND Position < @OldPosition;";

                    connection.Execute(shiftQuery, new
                    {
                        ServerId = channel.ServerId,
                        NewPosition = newPosition,
                        OldPosition = oldPosition
                    });
                }
                else
                {
                    // Moving down - shift up channels in between
                    var shiftQuery = @"
                        UPDATE Channels 
                        SET Position = Position - 1 
                        WHERE ServerId = @ServerId 
                        AND Position > @OldPosition 
                        AND Position <= @NewPosition;";

                    connection.Execute(shiftQuery, new
                    {
                        ServerId = channel.ServerId,
                        OldPosition = oldPosition,
                        NewPosition = newPosition
                    });
                }

                // Update the channel's position
                var updateQuery = "UPDATE Channels SET Position = @Position WHERE ChannelId = @ChannelId;";
                connection.Execute(updateQuery, new
                {
                    Position = newPosition,
                    ChannelId = channelId
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reordering channel: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get channels by type for a server
        /// </summary>
        /// <param name="serverId">Server ID</param>
        /// <param name="type">Channel type filter</param>
        /// <returns>List of channels of the specified type</returns>
        public static List<Channel> GetChannelsByType(int serverId, ChannelType type)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"
                    SELECT * FROM Channels 
                    WHERE ServerId = @ServerId AND Type = @Type 
                    ORDER BY Position ASC;";

                return connection.Query<Channel>(query, new
                {
                    ServerId = serverId,
                    Type = (int)type
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting channels by type: {ex.Message}");
                return new List<Channel>();
            }
        }
    }
}
