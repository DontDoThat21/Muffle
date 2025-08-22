using Dapper;
using Muffle.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muffle.Data.Services
{
    public class UsersService
    {
        public static User? GetUser()
        {
            return new User();
        }

        public static List<Server>? GetUsersServers()
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            // Create Servers table
            var selectUsersServersSql = @"Select * FROM Servers;";

            var servers = connection.Query<Server>(selectUsersServersSql).ToList();
            return servers;
        }

        public static List<Friend>? GetUsersFriends()
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            // Get Friends table
            var selectUsersFriendsSql = @"Select * from Friends;";

            var friends = connection.Query<Friend>(selectUsersFriendsSql).ToList();
            return friends;
        }

        public static Server? CreateServer(string name, string description, string ipAddress, double port, int userId = 1)
        {
            using var connection = SQLiteDbService.CreateConnection();
            connection.Open();

            // Insert new server
            var insertServerSql = @"
                INSERT INTO Servers (Name, Description, IpAddress, Port)
                VALUES (@Name, @Description, @IpAddress, @Port);
                SELECT last_insert_rowid();";

            var serverId = connection.QuerySingle<long>(insertServerSql, new { Name = name, Description = description, IpAddress = ipAddress, Port = port });

            // Insert server ownership
            var insertServerOwnerSql = @"
                INSERT INTO ServerOwners (ServerId, UserId)
                VALUES (@ServerId, @UserId);";

            connection.Execute(insertServerOwnerSql, new { ServerId = serverId, UserId = userId });

            // Return the created server
            return new Server(serverId, name, description, ipAddress, port);
        }

    }
}
