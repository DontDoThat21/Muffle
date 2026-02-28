namespace Muffle.Data.Models
{
    public class InviteLink
    {
        public int InviteLinkId { get; set; }
        public int ServerId { get; set; }
        public string Code { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUses { get; set; }
        public int UseCount { get; set; } = 0;
    }
}
