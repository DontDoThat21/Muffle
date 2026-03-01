using Dapper;
using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    public class FriendGroupService
    {
        public static int CreateGroup(int ownerUserId, string name)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var roomKey = Guid.NewGuid().ToString("N");
            var sql = @"
                INSERT INTO FriendGroups (OwnerUserId, Name, SortOrder, RoomKey, CreatedAt)
                VALUES (@OwnerUserId, @Name, 0, @RoomKey, @CreatedAt);
                SELECT last_insert_rowid();";

            return connection.QuerySingle<int>(sql, new
            {
                OwnerUserId = ownerUserId,
                Name = name,
                RoomKey = roomKey,
                CreatedAt = DateTime.UtcNow
            });
        }

        public static void DeleteGroup(int groupId, int ownerUserId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(
                "DELETE FROM FriendGroupMembers WHERE GroupId = @GroupId;",
                new { GroupId = groupId });

            connection.Execute(
                "DELETE FROM FriendGroups WHERE GroupId = @GroupId AND OwnerUserId = @OwnerUserId;",
                new { GroupId = groupId, OwnerUserId = ownerUserId });
        }

        public static void RenameGroup(int groupId, int ownerUserId, string newName)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(
                "UPDATE FriendGroups SET Name = @Name WHERE GroupId = @GroupId AND OwnerUserId = @OwnerUserId;",
                new { Name = newName, GroupId = groupId, OwnerUserId = ownerUserId });
        }

        public static List<FriendGroup> GetGroupsForUser(int ownerUserId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            var groups = connection.Query<FriendGroup>(
                "SELECT * FROM FriendGroups WHERE OwnerUserId = @OwnerUserId ORDER BY SortOrder, Name;",
                new { OwnerUserId = ownerUserId }).ToList();

            foreach (var group in groups)
            {
                group.Members = connection.Query<Friend>(@"
                    SELECT f.* FROM Friends f
                    INNER JOIN FriendGroupMembers m ON f.Id = m.FriendId
                    WHERE m.GroupId = @GroupId;",
                    new { GroupId = group.GroupId }).ToList();
            }

            return groups;
        }

        public static void AddFriendToGroup(int groupId, int friendId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            try
            {
                connection.Execute(@"
                    INSERT INTO FriendGroupMembers (GroupId, FriendId, AddedAt)
                    VALUES (@GroupId, @FriendId, @AddedAt);",
                    new { GroupId = groupId, FriendId = friendId, AddedAt = DateTime.UtcNow });
            }
            catch { }
        }

        public static void RemoveFriendFromGroup(int groupId, int friendId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            connection.Execute(
                "DELETE FROM FriendGroupMembers WHERE GroupId = @GroupId AND FriendId = @FriendId;",
                new { GroupId = groupId, FriendId = friendId });
        }

        public static List<Friend> GetFriendsNotInGroup(int groupId, int ownerUserId)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            return connection.Query<Friend>(@"
                SELECT f.* FROM Friends f
                WHERE f.UserId = @OwnerUserId
                AND f.Id NOT IN (
                    SELECT FriendId FROM FriendGroupMembers WHERE GroupId = @GroupId
                );",
                new { GroupId = groupId, OwnerUserId = ownerUserId }).ToList();
        }
    }
}
