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

    }
}
