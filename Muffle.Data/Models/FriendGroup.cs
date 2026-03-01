namespace Muffle.Data.Models
{
    public class FriendGroup
    {
        public int GroupId { get; set; }
        public int OwnerUserId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string RoomKey { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Friend> Members { get; set; } = new();
    }
}
