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

        /// <summary>
        /// Gets the full username with discriminator (e.g., "John#1234")
        /// </summary>
        public string FullUsername => $"{Name}#{Discriminator:D4}";
    }
}
