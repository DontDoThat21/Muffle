namespace Muffle.Data.Models
{
    public class ServerRole
    {
        public int RoleId { get; set; }
        public int ServerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Permissions { get; set; } // bitflags: 1=Read, 2=Send, 4=Manage, 8=Admin
        public int Position { get; set; }
        public string? Color { get; set; }
    }
}
