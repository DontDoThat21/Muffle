using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muffle.Data.Models
{
    public class Friend
    {

        public Friend(string Name, string description, string image)
        {
            this.Name = Name;
            this.Description = description;
            this.Image = image;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

    }
}
