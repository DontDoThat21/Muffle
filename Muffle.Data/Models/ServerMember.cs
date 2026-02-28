namespace Muffle.Data.Models
{
    public class ServerMember
    {
        public int ServerId { get; set; }
        public int UserId { get; set; }
        public int? RoleId { get; set; }
        public string? Nickname { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
