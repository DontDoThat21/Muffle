namespace Muffle.Data.Models
{
    /// <summary>
    /// DM privacy level controlling who can send direct messages to this user.
    /// </summary>
    public enum DmPrivacyLevel
    {
        Everyone = 0,
        FriendsOnly = 1,
        Nobody = 2
    }

    /// <summary>
    /// Controls who can send friend requests to this user.
    /// </summary>
    public enum FriendRequestFilterLevel
    {
        Everyone = 0,
        FriendsOfFriends = 1,
        Nobody = 2
    }

    /// <summary>
    /// Content filtering strength for explicit/unsafe content.
    /// </summary>
    public enum ContentFilterLevel
    {
        Off = 0,
        Medium = 1,
        High = 2
    }

    /// <summary>
    /// Represents a user's privacy and safety settings.
    /// </summary>
    public class PrivacySettings
    {
        public int UserId { get; set; }

        /// <summary>
        /// Stored as <see cref="DmPrivacyLevel"/> integer value.
        /// </summary>
        public int DmPrivacy { get; set; } = (int)DmPrivacyLevel.FriendsOnly;

        /// <summary>
        /// Stored as <see cref="FriendRequestFilterLevel"/> integer value.
        /// </summary>
        public int FriendRequestFilter { get; set; } = (int)FriendRequestFilterLevel.Everyone;

        /// <summary>
        /// Stored as <see cref="ContentFilterLevel"/> integer value.
        /// </summary>
        public int ContentFilter { get; set; } = (int)ContentFilterLevel.Medium;
    }
}
