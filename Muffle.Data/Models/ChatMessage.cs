using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muffle.Data.Models
{
    public class ChatMessage
    {
        public string Content { get; set; }
        public User Sender { get; set; }
        public DateTime Timestamp { get; set; }
        // Add other properties as needed
    }
}
