namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents an active login session, mapped from the AuthTokens table.
    /// </summary>
    public class UserSession
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string DeviceName { get; set; } = "Unknown Device";
        public string Platform { get; set; } = "Unknown";
        public string IpAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? LastUsedAt { get; set; }

        public bool IsExpired => ExpiresAt < DateTime.Now;

        /// <summary>
        /// Set at runtime by the service to flag the active session for the current process.
        /// Not stored in the database.
        /// </summary>
        public bool IsCurrentSession { get; set; }

        public string LastUsedDisplay => LastUsedAt.HasValue
            ? LastUsedAt.Value.ToString("g")
            : "Never";

        public string IpAddressDisplay => string.IsNullOrWhiteSpace(IpAddress)
            ? "Unknown"
            : IpAddress;
    }
}
