using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Handles the password change flow: verifies current password, issues a
    /// time-limited verification code (simulating an email), then applies the
    /// new BCrypt-hashed password on successful code entry.
    /// </summary>
    public static class PasswordChangeService
    {
        private const int CodeExpiryMinutes = 15;

        /// <summary>
        /// Verifies the current password and generates a 6-digit verification code.
        /// In production this code would be emailed; here it is returned so the UI
        /// can display it in a simulated "check your email" step.
        /// </summary>
        /// <returns>The verification code, or null if the current password is wrong.</returns>
        public static string? InitiatePasswordChange(int userId, string currentPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword))
                return null;

            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var user = connection.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE UserId = @UserId;", new { UserId = userId });

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                return null;

            // Invalidate any existing unused tokens for this user
            connection.Execute(
                "UPDATE PasswordResetTokens SET IsUsed = 1 WHERE UserId = @UserId AND IsUsed = 0;",
                new { UserId = userId });

            // Generate a 6-digit numeric code
            var code = Random.Shared.Next(100000, 999999).ToString();
            var now = DateTime.Now;

            connection.Execute(@"
                INSERT INTO PasswordResetTokens (UserId, Code, CreatedAt, ExpiresAt, IsUsed)
                VALUES (@UserId, @Code, @CreatedAt, @ExpiresAt, 0);",
                new
                {
                    UserId = userId,
                    Code = code,
                    CreatedAt = now,
                    ExpiresAt = now.AddMinutes(CodeExpiryMinutes)
                });

            return code;
        }

        /// <summary>
        /// Validates the verification code and, if valid, updates the user's password.
        /// </summary>
        /// <returns>True if the password was changed successfully.</returns>
        public static bool VerifyAndChangePassword(int userId, string code, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(newPassword))
                return false;

            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var token = connection.QueryFirstOrDefault<PasswordResetToken>(@"
                SELECT * FROM PasswordResetTokens
                WHERE UserId = @UserId AND Code = @Code AND IsUsed = 0
                ORDER BY CreatedAt DESC
                LIMIT 1;",
                new { UserId = userId, Code = code });

            if (token == null)
                return false;

            if (DateTime.Now > token.ExpiresAt)
                return false;

            var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            connection.Execute(
                "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserId = @UserId;",
                new { PasswordHash = newHash, UserId = userId });

            connection.Execute(
                "UPDATE PasswordResetTokens SET IsUsed = 1 WHERE TokenId = @TokenId;",
                new { TokenId = token.TokenId });

            return true;
        }
    }
}
