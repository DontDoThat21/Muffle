using Dapper;
using Muffle.Data.Models;
using BCrypt.Net;

namespace Muffle.Data.Services
{
    public class AuthenticationService
    {
        /// <summary>
        /// Registers a new user with email, username, and password
        /// </summary>
        /// <returns>The created User object or null if registration failed</returns>
        public static User? RegisterUser(string email, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            // Check if email already exists
            var checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email;";
            var emailExists = connection.ExecuteScalar<int>(checkEmailQuery, new { Email = email }) > 0;

            if (emailExists)
            {
                return null;
            }

            // Hash the password using BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            // Insert new user
            var insertUserQuery = @"
                INSERT INTO Users (Name, Email, PasswordHash, Description, CreationDate)
                VALUES (@Name, @Email, @PasswordHash, @Description, @CreationDate);
                SELECT last_insert_rowid();";

            try
            {
                var userId = connection.QuerySingle<long>(insertUserQuery, new
                {
                    Name = username,
                    Email = email,
                    PasswordHash = passwordHash,
                    Description = string.Empty,
                    CreationDate = DateTime.Now
                });

                return new User
                {
                    UserId = (int)userId,
                    Name = username,
                    Email = email,
                    PasswordHash = passwordHash,
                    Description = string.Empty,
                    CreationDate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Authenticates a user with email and password
        /// </summary>
        /// <returns>The User object if authentication succeeded, null otherwise</returns>
        public static User? LoginUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            // Get user by email
            var getUserQuery = "SELECT * FROM Users WHERE Email = @Email;";
            var user = connection.QueryFirstOrDefault<User>(getUserQuery, new { Email = email });

            if (user == null)
            {
                return null;
            }

            // Verify password using BCrypt
            var passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return passwordValid ? user : null;
        }

        /// <summary>
        /// Checks if an email is already registered
        /// </summary>
        public static bool EmailExists(string email)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email;";
            return connection.ExecuteScalar<int>(checkEmailQuery, new { Email = email }) > 0;
        }

        /// <summary>
        /// Generates and stores a new authentication token for a user
        /// </summary>
        /// <param name="userId">The user ID to create a token for</param>
        /// <param name="expirationDays">Number of days until token expires (default 30)</param>
        /// <returns>The generated token string, or null if failed</returns>
        public static string? GenerateAuthToken(int userId, int expirationDays = 30)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                // Generate a unique token
                var token = Guid.NewGuid().ToString();
                var createdAt = DateTime.Now;
                var expiresAt = createdAt.AddDays(expirationDays);

                var insertTokenQuery = @"
                    INSERT INTO AuthTokens (UserId, Token, CreatedAt, ExpiresAt)
                    VALUES (@UserId, @Token, @CreatedAt, @ExpiresAt);";

                connection.Execute(insertTokenQuery, new
                {
                    UserId = userId,
                    Token = token,
                    CreatedAt = createdAt,
                    ExpiresAt = expiresAt
                });

                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating auth token: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Validates an authentication token
        /// </summary>
        /// <param name="token">The token string to validate</param>
        /// <returns>True if token exists and is not expired, false otherwise</returns>
        public static bool ValidateAuthToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var validateTokenQuery = @"
                    SELECT COUNT(*) FROM AuthTokens 
                    WHERE Token = @Token AND ExpiresAt > @CurrentTime;";

                var count = connection.ExecuteScalar<int>(validateTokenQuery, new
                {
                    Token = token,
                    CurrentTime = DateTime.Now
                });

                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating auth token: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves a user by their authentication token
        /// </summary>
        /// <param name="token">The authentication token</param>
        /// <returns>The User object if token is valid, null otherwise</returns>
        public static User? GetUserByToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var getUserQuery = @"
                    SELECT u.* FROM Users u
                    INNER JOIN AuthTokens t ON u.UserId = t.UserId
                    WHERE t.Token = @Token AND t.ExpiresAt > @CurrentTime;";

                var user = connection.QueryFirstOrDefault<User>(getUserQuery, new
                {
                    Token = token,
                    CurrentTime = DateTime.Now
                });

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user by token: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Revokes (deletes) an authentication token
        /// </summary>
        /// <param name="token">The token to revoke</param>
        /// <returns>True if token was revoked, false otherwise</returns>
        public static bool RevokeAuthToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var deleteTokenQuery = "DELETE FROM AuthTokens WHERE Token = @Token;";
                var rowsAffected = connection.Execute(deleteTokenQuery, new { Token = token });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error revoking auth token: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Revokes all authentication tokens for a specific user
        /// </summary>
        /// <param name="userId">The user ID whose tokens should be revoked</param>
        /// <returns>Number of tokens revoked</returns>
        public static int RevokeAllUserTokens(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var deleteTokensQuery = "DELETE FROM AuthTokens WHERE UserId = @UserId;";
                var rowsAffected = connection.Execute(deleteTokensQuery, new { UserId = userId });

                return rowsAffected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error revoking user tokens: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Cleans up expired authentication tokens from the database
        /// </summary>
        /// <returns>Number of expired tokens deleted</returns>
        public static int CleanupExpiredTokens()
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var deleteExpiredQuery = "DELETE FROM AuthTokens WHERE ExpiresAt < @CurrentTime;";
                var rowsAffected = connection.Execute(deleteExpiredQuery, new { CurrentTime = DateTime.Now });

                return rowsAffected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning up expired tokens: {ex.message}");
                return 0;
            }
        }

        /// <summary>
        /// Retrieves all valid stored accounts for a list of tokens
        /// </summary>
        /// <param name="accounts">List of stored accounts with tokens</param>
        /// <returns>List of stored accounts with valid tokens only</returns>
        public static List<StoredAccount> ValidateStoredAccounts(List<StoredAccount> accounts)
        {
            var validAccounts = new List<StoredAccount>();

            foreach (var account in accounts)
            {
                // Validate token and get fresh user data
                var user = GetUserByToken(account.Token);
                
                if (user != null)
                {
                    // Token is still valid - update account info with latest data
                    account.Username = user.Name;
                    account.Email = user.Email;
                    validAccounts.Add(account);
                }
            }

            return validAccounts;
        }
    }
}
