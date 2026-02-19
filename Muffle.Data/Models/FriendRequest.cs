namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents a friend request between two users
    /// </summary>
    public class FriendRequest
    {
        public int RequestId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public int SenderDiscriminator { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverEmail { get; set; } = string.Empty;
        public int ReceiverDiscriminator { get; set; }
        public FriendRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }

        /// <summary>
        /// Gets the full sender username with discriminator (e.g., "John#1234")
        /// </summary>
        public string SenderFullUsername => $"{SenderName}#{SenderDiscriminator:D4}";

        /// <summary>
        /// Gets the full receiver username with discriminator (e.g., "John#1234")
        /// </summary>
        public string ReceiverFullUsername => $"{ReceiverName}#{ReceiverDiscriminator:D4}";
    }

    /// <summary>
    /// Status of a friend request
    /// </summary>
    public enum FriendRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2,
        Cancelled = 3
    }
}
