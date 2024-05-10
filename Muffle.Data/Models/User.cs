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
                new Server(name: "Test name 1", description: "Test server 1", ipAddress: "127.0.0.1", port: 8091),
                new Server(name: "Test name 2", description: "Test server 2", ipAddress: "127.0.0.1", port: 8092)
            };
        }
    }
}
