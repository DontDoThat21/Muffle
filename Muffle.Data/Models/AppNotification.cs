namespace Muffle.Data.Models
{
    public enum NotificationType
    {
        Mention,
        DirectMessage,
        FriendRequest,
        ServerInvite
    }

    public class AppNotification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? RelatedId { get; set; }
    }
}
