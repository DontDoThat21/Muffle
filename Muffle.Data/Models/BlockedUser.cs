namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents a blocked user relationship
    /// </summary>
    public class BlockedUser
    {
        public int BlockId { get; set; }
        public int BlockerId { get; set; }
        public int BlockedUserId { get; set; }
        public string BlockedUserName { get; set; } = string.Empty;
        public string BlockedUserEmail { get; set; } = string.Empty;
        public int BlockedUserDiscriminator { get; set; }
        public DateTime BlockedAt { get; set; }

        /// <summary>
        /// Gets the full blocked username with discriminator (e.g., "John#1234")
        /// </summary>
        public string BlockedUserFullUsername => $"{BlockedUserName}#{BlockedUserDiscriminator:D4}";
    }
}
