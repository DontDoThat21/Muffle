using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public static class InviteLinkService
    {
        // TASK-028: Create invite link
        public static InviteLink? CreateInviteLink(int serverId, int createdBy, DateTime? expiresAt, int? maxUses)
        {
            var code = Guid.NewGuid().ToString("N")[..8];
            var now = DateTime.UtcNow;

            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var sql = @"
                INSERT INTO InviteLinks (ServerId, Code, CreatedBy, CreatedAt, ExpiresAt, MaxUses, UseCount)
                VALUES (@ServerId, @Code, @CreatedBy, @CreatedAt, @ExpiresAt, @MaxUses, 0);
                SELECT last_insert_rowid();";

            var id = connection.ExecuteScalar<int>(sql, new
            {
                ServerId = serverId,
                Code = code,
                CreatedBy = createdBy,
                CreatedAt = now,
                ExpiresAt = expiresAt,
                MaxUses = maxUses
            });

            return new InviteLink
            {
                InviteLinkId = id,
                ServerId = serverId,
                Code = code,
                CreatedBy = createdBy,
                CreatedAt = now,
                ExpiresAt = expiresAt,
                MaxUses = maxUses,
                UseCount = 0
            };
        }

        // TASK-029: Lookup and validation
        public static InviteLink? GetInviteLinkByCode(string code)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.QuerySingleOrDefault<InviteLink>(
                "SELECT * FROM InviteLinks WHERE Code = @Code",
                new { Code = code });
        }

        public static bool ValidateInviteLink(string code)
        {
            var link = GetInviteLinkByCode(code);
            if (link == null) return false;
            if (link.ExpiresAt.HasValue && link.ExpiresAt.Value < DateTime.UtcNow) return false;
            if (link.MaxUses.HasValue && link.UseCount >= link.MaxUses.Value) return false;
            return true;
        }

        // TASK-030: Use invite link (join server)
        public static bool UseInviteLink(string code, int userId)
        {
            if (!ValidateInviteLink(code)) return false;

            var link = GetInviteLinkByCode(code);
            if (link == null) return false;

            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(
                "UPDATE InviteLinks SET UseCount = UseCount + 1 WHERE Code = @Code",
                new { Code = code });

            try
            {
                connection.Execute(
                    "INSERT INTO ServerMembers (ServerId, UserId, JoinedAt) VALUES (@ServerId, @UserId, @JoinedAt)",
                    new { ServerId = link.ServerId, UserId = userId, JoinedAt = DateTime.UtcNow });
            }
            catch (Exception)
            {
                // Already a member or table not yet created
            }

            return true;
        }
    }
}
