namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents an authentication token for persistent login sessions
    /// </summary>
    public class AuthToken
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
