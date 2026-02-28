namespace Muffle.Data.Models
{
    /// <summary>
    /// Stores TOTP two-factor authentication settings for a user.
    /// </summary>
    public class TwoFactorAuth
    {
        public int UserId { get; set; }
        public bool IsEnabled { get; set; }
        public string? Secret { get; set; }

        /// <summary>
        /// Semicolon-delimited BCrypt-hashed backup codes.
        /// Each code is consumed on first use.
        /// </summary>
        public string? BackupCodes { get; set; }

        public DateTime? EnabledAt { get; set; }
    }
}
