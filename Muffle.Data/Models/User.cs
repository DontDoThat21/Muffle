using Dapper;
using Muffle.Data.Services;

namespace Muffle.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public User? GetUser()
        {
            return new User();
        }

        public List<Server> GetUsersServers()
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            // Create Servers table
            var selectUsersServersSql = @"Select * FROM Servers;";

            var servers = connection.Query<Server>(selectUsersServersSql).ToList();
            return servers;
        }

        public List<Friend>? GetUsersFriends()
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            // Get Friends table
            var selectUsersFriendsSql = @"Select * from Friends;";

            var friends = connection.Query<Friend>(selectUsersFriendsSql).ToList();
            return friends;
        }
    }
}
