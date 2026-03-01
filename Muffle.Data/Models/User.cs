using Dapper;
using Muffle.Data.Services;

namespace Muffle.Data.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int Discriminator { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? DisabledAt { get; set; }

        // Profile customization (Phase 6)
        public string? AvatarUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? AboutMe { get; set; }
        public string? Pronouns { get; set; }

        // Status (Phase 6)
        public int Status { get; set; } = (int)UserStatus.Online;
        public string? CustomStatusText { get; set; }
        public string? CustomStatusEmoji { get; set; }
        public bool ShowOnlineStatus { get; set; } = true;

        // Security (Phase 8)
        public bool IsTwoFactorEnabled { get; set; } = false;
        public bool IsEmailVerified { get; set; } = false;

        /// <summary>
        /// Gets the full username with discriminator (e.g., "John#1234")
        /// </summary>
        public string FullUsername => $"{Name}#{Discriminator:D4}";

        /// <summary>
        /// Gets the current user status as the enum type
        /// </summary>
        public UserStatus UserStatus
        {
            get => (UserStatus)Status;
            set => Status = (int)value;
        }
    }
}
