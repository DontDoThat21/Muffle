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
    }
}
