namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents a stored account with authentication token
    /// Used for multiple account support
    /// </summary>
    public class StoredAccount
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime LastUsed { get; set; }
    }
}
