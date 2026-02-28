using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public static class MessageReactionService
    {
        public static void AddReaction(int messageId, int userId, string emoji)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(@"
                INSERT OR IGNORE INTO MessageReactions (MessageId, UserId, Emoji, CreatedAt)
                VALUES (@MessageId, @UserId, @Emoji, @CreatedAt)",
                new { MessageId = messageId, UserId = userId, Emoji = emoji, CreatedAt = DateTime.UtcNow });
        }

        public static void RemoveReaction(int messageId, int userId, string emoji)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(@"
                DELETE FROM MessageReactions
                WHERE MessageId = @MessageId AND UserId = @UserId AND Emoji = @Emoji",
                new { MessageId = messageId, UserId = userId, Emoji = emoji });
        }

        public static List<MessageReaction> GetReactionsForMessage(int messageId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.Query<MessageReaction>(
                "SELECT * FROM MessageReactions WHERE MessageId = @MessageId",
                new { MessageId = messageId }).ToList();
        }
    }
}
