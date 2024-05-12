using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muffle.Data.Models
{
    public class Friend
    {
        private string ipAddress;
        private int port;

        public Friend()
        {
        }

        public Friend(string Name, string description, string ipAddress, int port)
        {
            this.Name = Name;
            Description = description;
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
