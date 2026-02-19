using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing blocked users
    /// </summary>
    public static class BlockService
    {
        /// <summary>
        /// Block a user
        /// </summary>
        /// <param name="blockerId">ID of the user blocking</param>
        /// <param name="blockedUserId">ID of the user being blocked</param>
        /// <returns>True if blocked successfully, false otherwise</returns>
        public static bool BlockUser(int blockerId, int blockedUserId)
        {
            if (blockerId == blockedUserId)
            {
                Console.WriteLine("Cannot block yourself");
                return false;
            }

            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                // Check if already blocked
                var checkQuery = @"
                    SELECT COUNT(*) FROM BlockedUsers 
                    WHERE BlockerId = @BlockerId AND BlockedUserId = @BlockedUserId;";

                var alreadyBlocked = connection.ExecuteScalar<int>(checkQuery, new
                {
                    BlockerId = blockerId,
                    BlockedUserId = blockedUserId
                }) > 0;

                if (alreadyBlocked)
                {
                    Console.WriteLine("User is already blocked");
                    return false;
                }

                // Insert block record
                var insertQuery = @"
                    INSERT INTO BlockedUsers (BlockerId, BlockedUserId, BlockedAt)
                    VALUES (@BlockerId, @BlockedUserId, @BlockedAt);";

                connection.Execute(insertQuery, new
                {
                    BlockerId = blockerId,
                    BlockedUserId = blockedUserId,
                    BlockedAt = DateTime.Now
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error blocking user: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Unblock a user
        /// </summary>
        /// <param name="blockerId">ID of the user who blocked</param>
        /// <param name="blockedUserId">ID of the user to unblock</param>
        /// <returns>True if unblocked successfully, false otherwise</returns>
        public static bool UnblockUser(int blockerId, int blockedUserId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var deleteQuery = @"
                    DELETE FROM BlockedUsers 
                    WHERE BlockerId = @BlockerId AND BlockedUserId = @BlockedUserId;";

                var rowsAffected = connection.Execute(deleteQuery, new
                {
                    BlockerId = blockerId,
                    BlockedUserId = blockedUserId
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unblocking user: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if a user is blocked
        /// </summary>
        /// <param name="blockerId">ID of the blocker</param>
        /// <param name="blockedUserId">ID of the potentially blocked user</param>
        /// <returns>True if blocked, false otherwise</returns>
        public static bool IsUserBlocked(int blockerId, int blockedUserId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var checkQuery = @"
                    SELECT COUNT(*) FROM BlockedUsers 
                    WHERE BlockerId = @BlockerId AND BlockedUserId = @BlockedUserId;";

                return connection.ExecuteScalar<int>(checkQuery, new
                {
                    BlockerId = blockerId,
                    BlockedUserId = blockedUserId
                }) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if user is blocked: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all blocked users for a user
        /// </summary>
        /// <param name="blockerId">ID of the blocker</param>
        /// <returns>List of blocked users</returns>
        public static List<BlockedUser> GetBlockedUsers(int blockerId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var getBlockedQuery = @"
                    SELECT 
                        bu.BlockId,
                        bu.BlockerId,
                        bu.BlockedUserId,
                        bu.BlockedAt,
                        u.Name as BlockedUserName,
                        u.Email as BlockedUserEmail,
                        u.Discriminator as BlockedUserDiscriminator
                    FROM BlockedUsers bu
                    INNER JOIN Users u ON bu.BlockedUserId = u.UserId
                    WHERE bu.BlockerId = @BlockerId
                    ORDER BY bu.BlockedAt DESC;";

                var blockedUsers = connection.Query<BlockedUser>(getBlockedQuery, new { BlockerId = blockerId }).ToList();
                return blockedUsers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting blocked users: {ex.Message}");
                return new List<BlockedUser>();
            }
        }

        /// <summary>
        /// Get list of user IDs that are blocked by a specific user
        /// </summary>
        /// <param name="blockerId">ID of the blocker</param>
        /// <returns>List of blocked user IDs</returns>
        public static List<int> GetBlockedUserIds(int blockerId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var getIdsQuery = @"
                    SELECT BlockedUserId FROM BlockedUsers 
                    WHERE BlockerId = @BlockerId;";

                return connection.Query<int>(getIdsQuery, new { BlockerId = blockerId }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting blocked user IDs: {ex.Message}");
                return new List<int>();
            }
        }
    }
}
