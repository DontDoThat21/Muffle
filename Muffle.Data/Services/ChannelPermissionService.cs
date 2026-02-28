using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public static class ChannelPermissionService
    {
        public static void SetPermission(int channelId, int roleId, bool read, bool send, bool manage)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(@"
                INSERT INTO ChannelPermissions (ChannelId, RoleId, AllowRead, AllowSend, AllowManage)
                VALUES (@ChannelId, @RoleId, @AllowRead, @AllowSend, @AllowManage)
                ON CONFLICT(ChannelId, RoleId) DO UPDATE SET
                    AllowRead = @AllowRead,
                    AllowSend = @AllowSend,
                    AllowManage = @AllowManage",
                new { ChannelId = channelId, RoleId = roleId, AllowRead = read, AllowSend = send, AllowManage = manage });
        }

        public static ChannelPermission? GetPermission(int channelId, int roleId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.QuerySingleOrDefault<ChannelPermission>(
                "SELECT * FROM ChannelPermissions WHERE ChannelId = @ChannelId AND RoleId = @RoleId",
                new { ChannelId = channelId, RoleId = roleId });
        }

        public static bool CheckUserCanRead(int channelId, int userId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var permission = connection.QuerySingleOrDefault<ChannelPermission>(@"
                SELECT cp.* FROM ChannelPermissions cp
                INNER JOIN ServerMembers sm ON sm.RoleId = cp.RoleId
                WHERE cp.ChannelId = @ChannelId AND sm.UserId = @UserId",
                new { ChannelId = channelId, UserId = userId });

            return permission?.AllowRead ?? true;
        }

        public static bool CheckUserCanSend(int channelId, int userId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var permission = connection.QuerySingleOrDefault<ChannelPermission>(@"
                SELECT cp.* FROM ChannelPermissions cp
                INNER JOIN ServerMembers sm ON sm.RoleId = cp.RoleId
                WHERE cp.ChannelId = @ChannelId AND sm.UserId = @UserId",
                new { ChannelId = channelId, UserId = userId });

            return permission?.AllowSend ?? true;
        }
    }
}
