using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public static class RoleService
    {
        public static ServerRole? CreateRole(int serverId, string name, int permissions)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var sql = @"
                INSERT INTO ServerRoles (ServerId, Name, Permissions, Position, Color)
                VALUES (@ServerId, @Name, @Permissions, 0, NULL);
                SELECT last_insert_rowid();";

            var id = connection.ExecuteScalar<int>(sql, new { ServerId = serverId, Name = name, Permissions = permissions });

            return new ServerRole
            {
                RoleId = id,
                ServerId = serverId,
                Name = name,
                Permissions = permissions,
                Position = 0
            };
        }

        public static List<ServerRole> GetServerRoles(int serverId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.Query<ServerRole>(
                "SELECT * FROM ServerRoles WHERE ServerId = @ServerId ORDER BY Position",
                new { ServerId = serverId }).ToList();
        }

        public static bool AssignRole(int serverId, int userId, int roleId)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                connection.Execute(
                    "UPDATE ServerMembers SET RoleId = @RoleId WHERE ServerId = @ServerId AND UserId = @UserId",
                    new { RoleId = roleId, ServerId = serverId, UserId = userId });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning role: {ex.Message}");
                return false;
            }
        }

        public static string? GetNickname(int serverId, int userId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.ExecuteScalar<string?>(
                "SELECT Nickname FROM ServerMembers WHERE ServerId = @ServerId AND UserId = @UserId",
                new { ServerId = serverId, UserId = userId });
        }

        public static bool SetNickname(int serverId, int userId, string? nickname)
        {
            try
            {
                using var connection = SQLiteDbService.CreateConnection();
                connection.Open();

                connection.Execute(
                    "UPDATE ServerMembers SET Nickname = @Nickname WHERE ServerId = @ServerId AND UserId = @UserId",
                    new { Nickname = nickname, ServerId = serverId, UserId = userId });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting nickname: {ex.Message}");
                return false;
            }
        }
    }
}
