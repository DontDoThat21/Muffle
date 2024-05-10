using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muffle.Data.Models
{
    public class Server
    {
        public Server()
        {
        }

        public Server(string name, string? description, string ipAddress, int port)
        {
            Name = name;
            Description = description;
            IpAddress = ipAddress;
            Port = port;
        }

        public string Name { get; set; }
        public string? Description { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}
