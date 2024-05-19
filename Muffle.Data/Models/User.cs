using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public ObservableCollection<Server>? GetUsersServers()
        {
            return null;
            //return new ObservableCollection<Server>()
            //{
            //    new Server(name: "Main 1", description: "Main test server", ipAddress: "127.0.0.1", port: 8091),
            //    new Server(name: "Test 2", description: "Test server 2", ipAddress: "127.0.0.1", port: 8092),
            //    new Server(name: "Test 3", description: "Test server 3", ipAddress: "127.0.0.1", port: 8093),
            //    new Server(name: "Test 4", description: "Test server 4", ipAddress: "127.0.0.1", port: 8094),
            //    new Server(name: "Test 5", description: "Test server 5", ipAddress: "127.0.0.1", port: 8096),
            //    new Server(name: "Test 6", description: "Test server 5", ipAddress: "127.0.0.1", port: 8097)
            //};
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
