using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Handles email verification token generation and validation for new user accounts.
    /// In production the code would be emailed; here it is returned so the UI can
    /// display it in a simulated "check your email" step.
    /// </summary>
    public static class EmailVerificationService
    {
        private const int CodeExpiryMinutes = 30;

        /// <summary>
        /// Generates a 6-digit verification code for the given user and stores it.
        /// Returns the code so the UI can display it (simulated email).
        /// </summary>
        public static string CreateVerificationToken(int userId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            // Invalidate any existing unused tokens for this user
            connection.Execute(
                "UPDATE EmailVerificationTokens SET IsUsed = 1 WHERE UserId = @UserId AND IsUsed = 0;",
                new { UserId = userId });

            var code = Random.Shared.Next(100000, 999999).ToString();
            var now = DateTime.Now;

            connection.Execute(@"
                INSERT INTO EmailVerificationTokens (UserId, Code, CreatedAt, ExpiresAt, IsUsed)
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
        /// Validates the verification code and marks the user's email as verified.
        /// </summary>
        /// <returns>True if the email was successfully verified.</returns>
        public static bool VerifyEmail(int userId, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var token = connection.QueryFirstOrDefault<EmailVerificationToken>(@"
                SELECT * FROM EmailVerificationTokens
                WHERE UserId = @UserId AND Code = @Code AND IsUsed = 0
                ORDER BY CreatedAt DESC
                LIMIT 1;",
                new { UserId = userId, Code = code });

            if (token == null || DateTime.Now > token.ExpiresAt)
                return false;

            connection.Execute(
                "UPDATE Users SET IsEmailVerified = 1 WHERE UserId = @UserId;",
                new { UserId = userId });

            connection.Execute(
                "UPDATE EmailVerificationTokens SET IsUsed = 1 WHERE TokenId = @TokenId;",
                new { TokenId = token.TokenId });

            return true;
        }

        /// <summary>
        /// Regenerates a verification code for a user whose email is not yet verified.
        /// Returns the new code, or null if the user is already verified.
        /// </summary>
        public static string? ResendVerificationCode(int userId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var isVerified = connection.ExecuteScalar<bool>(
                "SELECT IsEmailVerified FROM Users WHERE UserId = @UserId;",
                new { UserId = userId });

            if (isVerified)
                return null;

            return CreateVerificationToken(userId);
        }
    }
}
