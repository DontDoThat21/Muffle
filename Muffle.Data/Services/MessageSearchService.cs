using System.Text.RegularExpressions;
using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public static class MessageSearchService
    {
        private static readonly Regex LinkRegex = new(@"https?://[^\s]+", RegexOptions.Compiled);
        // TASK-061: Search through friend messages
        public static List<ChatMessage> SearchMessages(int userId, string query)
        {
            var q = $"%{query}%";
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.Query<ChatMessage>(@"
                SELECT * FROM Messages
                WHERE (SenderId = @userId OR ReceiverId = @userId)
                AND Content LIKE @q
                ORDER BY Timestamp DESC
                LIMIT 50",
                new { userId, q }).ToList();
        }

        // TASK-062: Extract links from message content
        public static List<string> ExtractLinks(string content)
        {
            return LinkRegex.Matches(content)
                .Select(m => m.Value)
                .ToList();
        }

        // TASK-063: Search shared files/images
        public static List<ChatMessage> SearchFiles(int userId, string query)
        {
            var q = $"%{query}%";
            var imageType = (int)MessageType.Image;
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.Query<ChatMessage>(@"
                SELECT * FROM Messages
                WHERE (SenderId = @userId OR ReceiverId = @userId)
                AND Type = @imageType
                AND Content LIKE @q
                LIMIT 50",
                new { userId, imageType, q }).ToList();
        }

        // TASK-064: Search with filters (user, date range, message type)
        public static List<ChatMessage> SearchMessagesFiltered(
            int userId,
            string? query,
            int? fromUserId,
            DateTime? after,
            DateTime? before,
            MessageType? type)
        {
            var parameters = new DynamicParameters();
            var clauses = new List<string>
            {
                "(SenderId = @userId OR ReceiverId = @userId)"
            };
            parameters.Add("userId", userId);

            if (!string.IsNullOrWhiteSpace(query))
            {
                clauses.Add("Content LIKE @q");
                parameters.Add("q", $"%{query}%");
            }

            if (fromUserId.HasValue)
            {
                clauses.Add("(SenderId = @fromUserId OR ReceiverId = @fromUserId)");
                parameters.Add("fromUserId", fromUserId.Value);
            }

            if (after.HasValue)
            {
                clauses.Add("Timestamp >= @after");
                parameters.Add("after", after.Value);
            }

            if (before.HasValue)
            {
                clauses.Add("Timestamp <= @before");
                parameters.Add("before", before.Value);
            }

            if (type.HasValue)
            {
                clauses.Add("Type = @type");
                parameters.Add("type", (int)type.Value);
            }

            var sql = $"SELECT * FROM Messages WHERE {string.Join(" AND ", clauses)} ORDER BY Timestamp DESC LIMIT 50";

            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.Query<ChatMessage>(sql, parameters).ToList();
        }
    }
}
