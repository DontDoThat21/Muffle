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

        public ObservableCollection<Friend>? GetUsersFriends()
        {
            return new ObservableCollection<Friend>()
            {
                new Friend("Gabe", "Starcraft 2 Bro", "gabe.png"),
                new Friend("Tylor", "Best Programmer NA", "tom.jpg"),
                new Friend("Nick", "Army Motorcycling Bro", "nick.png"),
                new Friend("Tyler", "Best 1DGer in da land", "murky.png"),
            };
        }
    }
}
