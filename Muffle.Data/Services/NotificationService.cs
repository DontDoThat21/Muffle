using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public static class NotificationService
    {
        public static void CreateNotification(int userId, string title, string body, NotificationType type, int? relatedId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(@"
                INSERT INTO Notifications (UserId, Title, Body, Type, IsRead, CreatedAt, RelatedId)
                VALUES (@UserId, @Title, @Body, @Type, 0, @CreatedAt, @RelatedId)",
                new { UserId = userId, Title = title, Body = body, Type = (int)type, CreatedAt = DateTime.UtcNow, RelatedId = relatedId });
        }

        public static List<AppNotification> GetUnreadNotifications(int userId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.Query<AppNotification>(
                "SELECT * FROM Notifications WHERE UserId = @UserId AND IsRead = 0 ORDER BY CreatedAt DESC",
                new { UserId = userId }).ToList();
        }

        public static void MarkAsRead(int notificationId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(
                "UPDATE Notifications SET IsRead = 1 WHERE NotificationId = @NotificationId",
                new { NotificationId = notificationId });
        }

        public static int GetUnreadCount(int userId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM Notifications WHERE UserId = @UserId AND IsRead = 0",
                new { UserId = userId });
        }
    }
}
