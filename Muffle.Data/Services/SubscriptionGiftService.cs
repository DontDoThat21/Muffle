using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public class SubscriptionGiftService
    {
        public static SubscriptionGift SendGift(int senderId, int recipientId, SubscriptionTier tier, string message)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var now = DateTime.UtcNow;
            var id = connection.QuerySingle<int>(@"
                INSERT INTO SubscriptionGifts (SenderId, RecipientId, Tier, Status, Message, SentAt)
                VALUES (@SenderId, @RecipientId, @Tier, @Status, @Message, @SentAt);
                SELECT last_insert_rowid();",
                new
                {
                    SenderId = senderId,
                    RecipientId = recipientId,
                    Tier = (int)tier,
                    Status = (int)GiftStatus.Pending,
                    Message = message ?? string.Empty,
                    SentAt = now
                });

            return GetGiftById(id);
        }

        public static void AcceptGift(int giftId, int recipientId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var gift = connection.QueryFirstOrDefault<SubscriptionGift>(
                "SELECT * FROM SubscriptionGifts WHERE GiftId = @GiftId AND RecipientId = @RecipientId AND Status = @Pending;",
                new { GiftId = giftId, RecipientId = recipientId, Pending = (int)GiftStatus.Pending });

            if (gift == null) return;

            connection.Execute(@"
                UPDATE SubscriptionGifts SET Status = @Accepted, RedeemedAt = @Now
                WHERE GiftId = @GiftId;",
                new { Accepted = (int)GiftStatus.Accepted, Now = DateTime.UtcNow, GiftId = giftId });

            // Activate the subscription for the recipient
            SubscriptionService.Subscribe(recipientId, gift.Tier);
        }

        public static void DeclineGift(int giftId, int recipientId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(@"
                UPDATE SubscriptionGifts SET Status = @Declined, RedeemedAt = @Now
                WHERE GiftId = @GiftId AND RecipientId = @RecipientId AND Status = @Pending;",
                new
                {
                    Declined = (int)GiftStatus.Declined,
                    Now = DateTime.UtcNow,
                    GiftId = giftId,
                    RecipientId = recipientId,
                    Pending = (int)GiftStatus.Pending
                });
        }

        public static void CancelGift(int giftId, int senderId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(@"
                UPDATE SubscriptionGifts SET Status = @Cancelled, RedeemedAt = @Now
                WHERE GiftId = @GiftId AND SenderId = @SenderId AND Status = @Pending;",
                new
                {
                    Cancelled = (int)GiftStatus.Cancelled,
                    Now = DateTime.UtcNow,
                    GiftId = giftId,
                    SenderId = senderId,
                    Pending = (int)GiftStatus.Pending
                });
        }

        public static List<SubscriptionGift> GetReceivedGifts(int recipientId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.Query<SubscriptionGift>(@"
                SELECT g.*, s.Name AS SenderName, r.Name AS RecipientName
                FROM SubscriptionGifts g
                LEFT JOIN Users s ON s.UserId = g.SenderId
                LEFT JOIN Users r ON r.UserId = g.RecipientId
                WHERE g.RecipientId = @RecipientId
                ORDER BY g.SentAt DESC;",
                new { RecipientId = recipientId }).ToList();
        }

        public static List<SubscriptionGift> GetSentGifts(int senderId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.Query<SubscriptionGift>(@"
                SELECT g.*, s.Name AS SenderName, r.Name AS RecipientName
                FROM SubscriptionGifts g
                LEFT JOIN Users s ON s.UserId = g.SenderId
                LEFT JOIN Users r ON r.UserId = g.RecipientId
                WHERE g.SenderId = @SenderId
                ORDER BY g.SentAt DESC;",
                new { SenderId = senderId }).ToList();
        }

        public static int GetPendingReceivedCount(int recipientId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.QuerySingle<int>(
                "SELECT COUNT(*) FROM SubscriptionGifts WHERE RecipientId = @RecipientId AND Status = @Pending;",
                new { RecipientId = recipientId, Pending = (int)GiftStatus.Pending });
        }

        private static SubscriptionGift GetGiftById(int giftId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.QueryFirstOrDefault<SubscriptionGift>(@"
                SELECT g.*, s.Name AS SenderName, r.Name AS RecipientName
                FROM SubscriptionGifts g
                LEFT JOIN Users s ON s.UserId = g.SenderId
                LEFT JOIN Users r ON r.UserId = g.RecipientId
                WHERE g.GiftId = @GiftId;",
                new { GiftId = giftId });
        }
    }
}
