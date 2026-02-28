using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public static class ServerBrowserService
    {
        // TASK-031
        public static List<Server> GetPublicServers()
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();
            return connection.Query<Server>("SELECT * FROM Servers WHERE IsPublic = 1").ToList();
        }

        // TASK-032
        public static List<Server> SearchServers(string query)
        {
            var q = $"%{query}%";
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();
            return connection.Query<Server>(
                "SELECT * FROM Servers WHERE IsPublic = 1 AND (Name LIKE @q OR Description LIKE @q)",
                new { q }).ToList();
        }

        // TASK-036
        public static bool JoinServer(int serverId, int userId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                var existing = connection.ExecuteScalar<int>(
                    "SELECT COUNT(1) FROM ServerMembers WHERE ServerId = @ServerId AND UserId = @UserId",
                    new { ServerId = serverId, UserId = userId });

                if (existing > 0) return false;

                connection.Execute(
                    "INSERT INTO ServerMembers (ServerId, UserId, JoinedAt) VALUES (@ServerId, @UserId, @JoinedAt)",
                    new { ServerId = serverId, UserId = userId, JoinedAt = DateTime.UtcNow });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error joining server: {ex.Message}");
                return false;
            }
        }
    }
}
