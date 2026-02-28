using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Returns hardcoded app changelog entries, newest version first.
    /// </summary>
    public static class PatchNotesService
    {
        public static List<PatchNote> GetPatchNotes()
        {
            return new List<PatchNote>
            {
                new PatchNote
                {
                    Version = "v0.7.0",
                    ReleaseDate = new DateTime(2026, 2, 19),
                    Summary = "Settings & Configuration — voice, video, accessibility, developer, privacy, and connected device management.",
                    Entries = new List<PatchNoteEntry>
                    {
                        new PatchNoteEntry { EntryType = "New", Description = "Voice settings: input/output device picker, volume sliders, push-to-talk key binding, and noise suppression toggle." },
                        new PatchNoteEntry { EntryType = "New", Description = "Video settings: camera selection, resolution picker (360p – 4K), and FPS picker (15/30/60)." },
                        new PatchNoteEntry { EntryType = "New", Description = "Accessibility settings: font size picker, high contrast mode, and screen reader toggle." },
                        new PatchNoteEntry { EntryType = "New", Description = "Developer settings: debug mode, WebSocket message inspector, and dev tools toggle." },
                        new PatchNoteEntry { EntryType = "New", Description = "Privacy & safety settings: DM privacy, friend request filtering, and content filtering controls." },
                        new PatchNoteEntry { EntryType = "New", Description = "Connected devices view: see active login sessions and log out remotely." },
                        new PatchNoteEntry { EntryType = "New", Description = "Patch notes viewer — in-app changelog (this screen!)." },
                    }
                },
                new PatchNote
                {
                    Version = "v0.6.0",
                    ReleaseDate = new DateTime(2025, 5, 1),
                    Summary = "User Profile & Customization — rich profiles, status, external connections, and app theming.",
                    Entries = new List<PatchNoteEntry>
                    {
                        new PatchNoteEntry { EntryType = "New", Description = "Profile customization: avatar, banner, about me, and pronouns." },
                        new PatchNoteEntry { EntryType = "New", Description = "User status: Online, Away, Do Not Disturb, and Invisible modes." },
                        new PatchNoteEntry { EntryType = "New", Description = "Custom status messages with emoji." },
                        new PatchNoteEntry { EntryType = "New", Description = "External activity status from Spotify, Steam, and Xbox Live." },
                        new PatchNoteEntry { EntryType = "New", Description = "Profile connections to 10 external services with per-connection visibility controls." },
                        new PatchNoteEntry { EntryType = "New", Description = "App themes: dark/light mode toggle, 5 presets, and 8 accent color options." },
                        new PatchNoteEntry { EntryType = "New", Description = "Profile settings page with four tabs: Profile, Status, Connections, and Active Status." },
                        new PatchNoteEntry { EntryType = "New", Description = "Show/hide online status option for privacy." },
                    }
                },
                new PatchNote
                {
                    Version = "v0.5.0",
                    ReleaseDate = new DateTime(2025, 2, 1),
                    Summary = "Chat Enhancements — rich media, reactions, search, mentions, and link previews.",
                    Entries = new List<PatchNoteEntry>
                    {
                        new PatchNoteEntry { EntryType = "New", Description = "Emoji picker with 30 emoji." },
                        new PatchNoteEntry { EntryType = "New", Description = "GIF search via the Tenor v2 API." },
                        new PatchNoteEntry { EntryType = "New", Description = "@username mentions with bold display resolution." },
                        new PatchNoteEntry { EntryType = "New", Description = "Desktop and mobile push notifications with unread count tracking." },
                        new PatchNoteEntry { EntryType = "New", Description = "Message search with filters: user, date range, and message type." },
                        new PatchNoteEntry { EntryType = "New", Description = "Link extraction from chat messages." },
                        new PatchNoteEntry { EntryType = "New", Description = "Message reactions with per-user emoji tracking." },
                        new PatchNoteEntry { EntryType = "New", Description = "Message threads and replies via ParentMessageId." },
                        new PatchNoteEntry { EntryType = "New", Description = "Rich link previews using Open Graph metadata with inline preview cards." },
                    }
                },
                new PatchNote
                {
                    Version = "v0.4.0",
                    ReleaseDate = new DateTime(2024, 12, 1),
                    Summary = "Server Features — channels, permissions, roles, invite links, and public server discovery.",
                    Entries = new List<PatchNoteEntry>
                    {
                        new PatchNoteEntry { EntryType = "New", Description = "Server text and voice channels with CRUD and reordering." },
                        new PatchNoteEntry { EntryType = "New", Description = "Public/private server toggle." },
                        new PatchNoteEntry { EntryType = "New", Description = "Invite links — generate or customize, with expiry and use-limit support." },
                        new PatchNoteEntry { EntryType = "New", Description = "Public server browser with search." },
                        new PatchNoteEntry { EntryType = "New", Description = "Join server from browser (duplicate-guard enforced)." },
                        new PatchNoteEntry { EntryType = "New", Description = "Customizable server and channel icons." },
                        new PatchNoteEntry { EntryType = "New", Description = "Server roles and member role assignments." },
                        new PatchNoteEntry { EntryType = "New", Description = "Per-channel read/send permissions." },
                        new PatchNoteEntry { EntryType = "New", Description = "Server-specific nicknames." },
                    }
                },
                new PatchNote
                {
                    Version = "v0.3.0",
                    ReleaseDate = new DateTime(2024, 10, 15),
                    Summary = "User Management — registration, authentication, friend management, and account controls.",
                    Entries = new List<PatchNoteEntry>
                    {
                        new PatchNoteEntry { EntryType = "New", Description = "Account registration with email validation and BCrypt password hashing." },
                        new PatchNoteEntry { EntryType = "New", Description = "Token-based authentication with SecureStorage auto-login on startup." },
                        new PatchNoteEntry { EntryType = "New", Description = "Multiple account support with account switcher view." },
                        new PatchNoteEntry { EntryType = "New", Description = "Add friend via user search and send friend request." },
                        new PatchNoteEntry { EntryType = "New", Description = "Friend request inbox with incoming/outgoing tabs and accept/decline/cancel actions." },
                        new PatchNoteEntry { EntryType = "New", Description = "User discriminators — unique name#1234 identifiers." },
                        new PatchNoteEntry { EntryType = "New", Description = "Block users — block, unblock, and view blocked list." },
                        new PatchNoteEntry { EntryType = "New", Description = "Account disable with re-enable support, and permanent account deletion." },
                    }
                },
                new PatchNote
                {
                    Version = "v0.2.0",
                    ReleaseDate = new DateTime(2024, 8, 1),
                    Summary = "Voice & Video — WebRTC-powered real-time voice and video calls.",
                    Entries = new List<PatchNoteEntry>
                    {
                        new PatchNoteEntry { EntryType = "New", Description = "Voice calls via WebRTC with STUN server and full peer connection lifecycle." },
                        new PatchNoteEntry { EntryType = "New", Description = "Video calls via WebRTC with local and remote media track management." },
                        new PatchNoteEntry { EntryType = "New", Description = "Full SDP offer/answer negotiation via MessageWrapper signaling." },
                        new PatchNoteEntry { EntryType = "New", Description = "ICE candidate exchange via SignalingService." },
                        new PatchNoteEntry { EntryType = "New", Description = "Call state UI — Calling, Connected, and Ended states with in-chat notifications." },
                    }
                },
                new PatchNote
                {
                    Version = "v0.1.0",
                    ReleaseDate = new DateTime(2024, 6, 1),
                    Summary = "Foundation — initial release with core layout, messaging, and database.",
                    Entries = new List<PatchNoteEntry>
                    {
                        new PatchNoteEntry { EntryType = "New", Description = "Three-panel Discord-style layout: servers list, friends/DMs panel, and main content area." },
                        new PatchNoteEntry { EntryType = "New", Description = "Server list with create-server dialog." },
                        new PatchNoteEntry { EntryType = "New", Description = "Friends list with avatars and direct message view." },
                        new PatchNoteEntry { EntryType = "New", Description = "Real-time chat over WebSocket with JSON MessageWrapper serialization." },
                        new PatchNoteEntry { EntryType = "New", Description = "Image messaging with Base64 encoding and in-chat image display." },
                        new PatchNoteEntry { EntryType = "New", Description = "Dual database strategy — SQLite for development, SQL Server for production." },
                        new PatchNoteEntry { EntryType = "New", Description = "MVVM architecture with ObservableCollection bindings." },
                    }
                },
            };
        }
    }
}
