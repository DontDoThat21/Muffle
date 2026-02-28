using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing active login sessions (AuthTokens).
    /// </summary>
    public static class UserSessionService
    {
        /// <summary>
        /// Returns all non-expired sessions for a user, newest first.
        /// </summary>
        public static List<UserSession> GetActiveSessions(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"
                    SELECT TokenId, UserId, Token, DeviceName, Platform, CreatedAt, ExpiresAt
                    FROM AuthTokens
                    WHERE UserId = @UserId AND ExpiresAt > @Now
                    ORDER BY CreatedAt DESC;";

                return connection.Query<UserSession>(query, new { UserId = userId, Now = DateTime.Now }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting active sessions: {ex.Message}");
                return new List<UserSession>();
            }
        }

        /// <summary>
        /// Revokes (deletes) a session by its token ID.
        /// </summary>
        public static bool RevokeSession(int tokenId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "DELETE FROM AuthTokens WHERE TokenId = @TokenId;";
                var rows = connection.Execute(query, new { TokenId = tokenId });
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error revoking session: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Revokes all sessions for a user except the currently active token.
        /// </summary>
        /// <returns>Number of sessions revoked.</returns>
        public static int RevokeAllOtherSessions(int userId, string currentToken)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "DELETE FROM AuthTokens WHERE UserId = @UserId AND Token != @CurrentToken;";
                return connection.Execute(query, new { UserId = userId, CurrentToken = currentToken });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error revoking other sessions: {ex.Message}");
                return 0;
            }
        }
    }
}
