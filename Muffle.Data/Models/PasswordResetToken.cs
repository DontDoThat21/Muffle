namespace Muffle.Data.Models
{
    /// <summary>
    /// Represents a one-time verification code issued during a password change flow.
    /// </summary>
    public class PasswordResetToken
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
