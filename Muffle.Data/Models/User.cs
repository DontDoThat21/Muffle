using Dapper;
using Muffle.Data.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

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
            using var connection = SqliteDbService.CreateConnection();
            connection.Open();

            // Create Servers table
            var selectUsersServersSql = @"Select * from Servers;";

            var servers = connection.Query<Server>(selectUsersServersSql).ToList();
            return servers;
        }

        public List<Friend>? GetUsersFriends()
        {
            using var connection = SqliteDbService.CreateConnection();
            connection.Open();

            // Get Friends table
            var selectUsersFriendsSql = @"Select * from Friends;";

            var friends = connection.Query<Friend>(selectUsersFriendsSql).ToList();
            return friends;
        }
    }
}
