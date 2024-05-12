using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            return new ObservableCollection<Server>()
            {
                new Server(name: "Test name 1", description: "Main test server", ipAddress: "127.0.0.1", port: 8091),
                new Server(name: "Test name 2", description: "Test server 2", ipAddress: "127.0.0.1", port: 8092),
                new Server(name: "Test name 3", description: "Test server 3", ipAddress: "127.0.0.1", port: 8093),
                new Server(name: "Test name 4", description: "Test server 4", ipAddress: "127.0.0.1", port: 8094),
                new Server(name: "Test name 5", description: "Test server 5", ipAddress: "127.0.0.1", port: 8096),
                new Server(name: "Test name 6", description: "Test server 5", ipAddress: "127.0.0.1", port: 8097)
            };
        }

        public ObservableCollection<Friend>? GetUsersFriends()
        {
            return new ObservableCollection<Friend>()
            {
                new Friend() {
                    Name = "Mike"
                }
            };
        }
    }
}
