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
    }
}
