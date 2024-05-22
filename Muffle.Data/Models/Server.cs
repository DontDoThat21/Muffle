namespace Muffle.Data.Models
{
    public class Server
    {
        public Server() 
        {

        }
        public Server(double id, string name, string? description, string ipAddress, double port)
        {
            Id = Id;
            Name = name;
            Description = description;
            IpAddress = ipAddress;
            Port = port;
        }

        public double Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string IpAddress { get; set; }
        public double Port { get; set; }
    }
}
