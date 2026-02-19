using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing user accounts (disable, enable, delete)
    /// </summary>
    public static class AccountManagementService
    {
        /// <summary>
        /// Disable a user account (soft-delete, recoverable)
        /// </summary>
        /// <param name="userId">ID of the user to disable</param>
        /// <returns>True if disabled successfully, false otherwise</returns>
        public static bool DisableAccount(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                // Check if account is already disabled
                var checkQuery = "SELECT IsActive FROM Users WHERE UserId = @UserId;";
                var isActive = connection.QueryFirstOrDefault<bool?>(checkQuery, new { UserId = userId });

                if (isActive == null)
                {
                    Console.WriteLine("User not found");
                    return false;
                }

                if (!isActive.Value)
                {
                    Console.WriteLine("Account is already disabled");
                    return false;
                }

                // Disable the account
                var updateQuery = @"
                    UPDATE Users 
                    SET IsActive = 0, DisabledAt = @DisabledAt
                    WHERE UserId = @UserId;";

                connection.Execute(updateQuery, new
                {
                    UserId = userId,
                    DisabledAt = DateTime.Now
                });

                // Revoke all authentication tokens for this user
                AuthenticationService.RevokeAllUserTokens(userId);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disabling account: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Enable a previously disabled account (recover from soft-delete)
        /// </summary>
        /// <param name="userId">ID of the user to enable</param>
        /// <returns>True if enabled successfully, false otherwise</returns>
        public static bool EnableAccount(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                // Check if account exists and is disabled
                var checkQuery = "SELECT IsActive FROM Users WHERE UserId = @UserId;";
                var isActive = connection.QueryFirstOrDefault<bool?>(checkQuery, new { UserId = userId });

                if (isActive == null)
                {
                    Console.WriteLine("User not found");
                    return false;
                }

                if (isActive.Value)
                {
                    Console.WriteLine("Account is already active");
                    return false;
                }

                // Enable the account
                var updateQuery = @"
                    UPDATE Users 
                    SET IsActive = 1, DisabledAt = NULL
                    WHERE UserId = @UserId;";

                connection.Execute(updateQuery, new { UserId = userId });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enabling account: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Permanently delete a user account (cannot be recovered)
        /// </summary>
        /// <param name="userId">ID of the user to delete</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public static bool DeleteAccount(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                using var transaction = connection.BeginTransaction();

                try
                {
                    // Delete all authentication tokens
                    connection.Execute("DELETE FROM AuthTokens WHERE UserId = @UserId;", new { UserId = userId }, transaction);

                    // Delete all friend requests (both sent and received)
                    connection.Execute("DELETE FROM FriendRequests WHERE SenderId = @UserId OR ReceiverId = @UserId;", new { UserId = userId }, transaction);

                    // Delete all blocked user relationships
                    connection.Execute("DELETE FROM BlockedUsers WHERE BlockerId = @UserId OR BlockedUserId = @UserId;", new { UserId = userId }, transaction);

                    // Delete all friends relationships
                    connection.Execute("DELETE FROM Friends WHERE UserId = @UserId;", new { UserId = userId }, transaction);

                    // Delete the user
                    connection.Execute("DELETE FROM Users WHERE UserId = @UserId;", new { UserId = userId }, transaction);

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting account: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if a user account is active
        /// </summary>
        /// <param name="userId">ID of the user to check</param>
        /// <returns>True if active, false if disabled or not found</returns>
        public static bool IsAccountActive(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "SELECT IsActive FROM Users WHERE UserId = @UserId;";
                var isActive = connection.QueryFirstOrDefault<bool?>(query, new { UserId = userId });

                return isActive ?? false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking account status: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get user account information including disabled status
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>User object or null if not found</returns>
        public static User? GetUserAccountInfo(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "SELECT * FROM Users WHERE UserId = @UserId;";
                return connection.QueryFirstOrDefault<User>(query, new { UserId = userId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user account info: {ex.Message}");
                return null;
            }
        }
    }
}
