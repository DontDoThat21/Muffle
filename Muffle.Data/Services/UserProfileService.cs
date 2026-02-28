using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Service for managing user profile customization (avatar, banner, about me, pronouns)
    /// </summary>
    public static class UserProfileService
    {
        /// <summary>
        /// Get the full profile for a user
        /// </summary>
        public static User? GetProfile(int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"SELECT UserId, Name, Email, Description, Discriminator, 
                              AvatarUrl, BannerUrl, AboutMe, Pronouns,
                              Status, CustomStatusText, CustomStatusEmoji, ShowOnlineStatus
                              FROM Users WHERE UserId = @UserId;";

                return connection.QueryFirstOrDefault<User>(query, new { UserId = userId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting profile: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Update the user's avatar URL
        /// </summary>
        public static bool UpdateAvatar(int userId, string? avatarUrl)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "UPDATE Users SET AvatarUrl = @AvatarUrl WHERE UserId = @UserId;";
                connection.Execute(query, new { UserId = userId, AvatarUrl = avatarUrl });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating avatar: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update the user's banner URL
        /// </summary>
        public static bool UpdateBanner(int userId, string? bannerUrl)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "UPDATE Users SET BannerUrl = @BannerUrl WHERE UserId = @UserId;";
                connection.Execute(query, new { UserId = userId, BannerUrl = bannerUrl });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating banner: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update the user's About Me text
        /// </summary>
        public static bool UpdateAboutMe(int userId, string? aboutMe)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "UPDATE Users SET AboutMe = @AboutMe WHERE UserId = @UserId;";
                connection.Execute(query, new { UserId = userId, AboutMe = aboutMe });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating about me: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update the user's pronouns
        /// </summary>
        public static bool UpdatePronouns(int userId, string? pronouns)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = "UPDATE Users SET Pronouns = @Pronouns WHERE UserId = @UserId;";
                connection.Execute(query, new { UserId = userId, Pronouns = pronouns });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating pronouns: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update all profile fields at once
        /// </summary>
        public static bool UpdateProfile(int userId, string? avatarUrl, string? bannerUrl, string? aboutMe, string? pronouns)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var query = @"UPDATE Users 
                              SET AvatarUrl = @AvatarUrl, 
                                  BannerUrl = @BannerUrl, 
                                  AboutMe = @AboutMe, 
                                  Pronouns = @Pronouns 
                              WHERE UserId = @UserId;";

                connection.Execute(query, new
                {
                    UserId = userId,
                    AvatarUrl = avatarUrl,
                    BannerUrl = bannerUrl,
                    AboutMe = aboutMe,
                    Pronouns = pronouns
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                return false;
            }
        }
    }
}
