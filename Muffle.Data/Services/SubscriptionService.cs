using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public class SubscriptionService
    {
        public static UserSubscription? GetSubscription(int userId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.QueryFirstOrDefault<UserSubscription>(
                "SELECT * FROM UserSubscriptions WHERE UserId = @UserId ORDER BY StartedAt DESC LIMIT 1;",
                new { UserId = userId });
        }

        public static bool IsUserPremium(int userId)
        {
            var sub = GetSubscription(userId);
            return sub?.IsPremium ?? false;
        }

        public static bool IsUserPremiumPlus(int userId)
        {
            var sub = GetSubscription(userId);
            return sub?.IsPremiumPlus ?? false;
        }

        public static UserSubscription Subscribe(int userId, SubscriptionTier tier)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            // Cancel any existing active subscription first
            connection.Execute(@"
                UPDATE UserSubscriptions
                SET Status = @Cancelled, CancelledAt = @Now
                WHERE UserId = @UserId AND Status = @Active;",
                new
                {
                    Cancelled = (int)SubscriptionStatus.Cancelled,
                    Active = (int)SubscriptionStatus.Active,
                    Now = DateTime.UtcNow,
                    UserId = userId
                });

            var now = DateTime.UtcNow;
            var expiry = now.AddMonths(1);

            var id = connection.QuerySingle<int>(@"
                INSERT INTO UserSubscriptions (UserId, Tier, Status, StartedAt, ExpiresAt)
                VALUES (@UserId, @Tier, @Status, @StartedAt, @ExpiresAt);
                SELECT last_insert_rowid();",
                new
                {
                    UserId = userId,
                    Tier = (int)tier,
                    Status = (int)SubscriptionStatus.Active,
                    StartedAt = now,
                    ExpiresAt = expiry
                });

            return new UserSubscription
            {
                SubscriptionId = id,
                UserId = userId,
                Tier = tier,
                Status = SubscriptionStatus.Active,
                StartedAt = now,
                ExpiresAt = expiry
            };
        }

        public static void CancelSubscription(int userId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(@"
                UPDATE UserSubscriptions
                SET Status = @Cancelled, CancelledAt = @Now
                WHERE UserId = @UserId AND Status = @Active;",
                new
                {
                    Cancelled = (int)SubscriptionStatus.Cancelled,
                    Active = (int)SubscriptionStatus.Active,
                    Now = DateTime.UtcNow,
                    UserId = userId
                });
        }

        public static List<SubscriptionFeature> GetFeatureComparison()
        {
            return new List<SubscriptionFeature>
            {
                new() { Name = "Messaging & Voice Calls",     FreeValue = "✔",        PremiumValue = "✔",        PremiumPlusValue = "✔" },
                new() { Name = "Video Quality",               FreeValue = "720p",      PremiumValue = "1080p HD", PremiumPlusValue = "4K Ultra HD" },
                new() { Name = "Max File Upload Size",        FreeValue = "8 MB",      PremiumValue = "50 MB",    PremiumPlusValue = "100 MB" },
                new() { Name = "Friend Groups",               FreeValue = "5",         PremiumValue = "25",       PremiumPlusValue = "100" },
                new() { Name = "Custom Emoji",                FreeValue = "—",         PremiumValue = "✔",        PremiumPlusValue = "✔" },
                new() { Name = "Animated Avatars",            FreeValue = "—",         PremiumValue = "✔",        PremiumPlusValue = "✔" },
                new() { Name = "Server Boosts per Month",     FreeValue = "0",         PremiumValue = "2",        PremiumPlusValue = "5" },
                new() { Name = "Profile Badge",               FreeValue = "—",         PremiumValue = "Premium",  PremiumPlusValue = "Premium+" },
                new() { Name = "Priority Support",            FreeValue = "—",         PremiumValue = "—",        PremiumPlusValue = "✔" },
                new() { Name = "Early Access to Features",   FreeValue = "—",         PremiumValue = "—",        PremiumPlusValue = "✔" },
            };
        }
    }
}
