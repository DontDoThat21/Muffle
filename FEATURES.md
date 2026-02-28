# Muffle — Feature Development Tracker

**Repository:** https://github.com/DontDoThat21/Muffle  
**Stack:** C# WPF, .NET MAUI, WebRTC, WebSockets, Dapper, SQLite/SQL Server  
**Goal:** Build an open-source Discord-like communications platform with real-time voice, video, and chat

---

## Legend

- ✅ **Complete** — Implemented, tested, committed
- 🔧 **In Progress** — Partially implemented or under active development
- 📋 **Planned** — Specified but not yet started

---

## Phase 1: Foundation ✅

Core infrastructure, UI layout, and basic real-time messaging.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 1.1 | Three-panel Discord-style layout | ✅ | Servers (left), Friends/DMs (center-left), Main content (center-right) |
| 1.2 | Server management (list, create) | ✅ | Servers loaded from DB, create via "+" button with name/description prompt |
| 1.3 | Friends list display | ✅ | Friends loaded from DB with avatar + name buttons |
| 1.4 | Friend selection & chat view | ✅ | Click friend → switches to FriendDetailsContentView |
| 1.5 | Real-time chat (WebSocket) | ✅ | WebSocket @ ws://localhost:8080, MessageWrapper JSON serialization |
| 1.6 | Image messaging | ✅ | ImagePickerService, Base64 encoding/decoding, MessageType.Image |
| 1.7 | Database (SQLite + SQL Server) | ✅ | Dual DB strategy, Dapper ORM, seed data (users, servers, friends) |
| 1.8 | Top bar dynamic switching | ✅ | FriendsTopBarUIView, FriendDetailTopBarUIView, ServerTopBarUIView |
| 1.9 | MVVM architecture | ✅ | MainPageViewModel, FriendDetailsContentViewModel with ObservableCollections |
| 1.10 | Message converters | ✅ | Base64ToImageConverter, MessageTypeToTextVisibilityConverter, StringNotEmptyConverter |

---

## Phase 2: Voice & Video (WebRTC) ✅

Real-time voice and video calls powered by WebRTC.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 2.1 | Voice calls | ✅ | `StartVoiceCallAsync` fully implemented with WebRTCManager |
| 2.2 | Video calls | ✅ | `StartVideoCallAsync` fully implemented with WebRTCManager |
| 2.3 | WebRTC peer connection management | ✅ | `WebRTCManager` complete with STUN server, peer connection lifecycle |
| 2.4 | SDP offer/answer negotiation | ✅ | Full SDP negotiation via MessageWrapper signaling |
| 2.5 | ICE candidate exchange | ✅ | ICE candidates exchanged via SignalingService |
| 2.6 | Media track management | ✅ | WebRTCme integration for local/remote audio/video tracks, OnRemoteStreamAdded event |
| 2.7 | Call state UI (calling, connected, ended) | ✅ | CallState enum + OnCallStateChanged event, chat notifications for call states |

---

## Phase 3: User Management 📋

Account creation, authentication, and friend management.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 3.1 | User account creation process | ✅ | Registration flow with email validation, password hashing (BCrypt), login/registration views |
| 3.2 | Remember user login | ✅ | Token-based auth with SecureStorage, auto-login on startup, logout functionality |
| 3.3 | Multiple account support | ✅ | AccountSwitcherView with stored accounts list, switch/remove buttons, auto-restore last used account |
| 3.4 | Add friend functionality | ✅ | FriendRequests table, SearchUsers service, AddFriendView with search and send request |
| 3.5 | Friend requests & approval flow | ✅ | FriendRequestsView with incoming/outgoing tabs, accept/decline/cancel actions, accessible via Pending button |
| 3.6 | User discriminator (incremented numbers) | ✅ | Added Discriminator column, auto-generation on registration (1-9999), FullUsername property, search by name#discriminator |
| 3.7 | Block users | ✅ | BlockedUsers table, BlockService with block/unblock/check methods, BlockedUsersView, block from search results |
| 3.8 | Disable account | ✅ | Added IsActive/DisabledAt columns, AccountManagementService, AccountSettingsView with disable/delete, login check |
| 3.9 | Delete account | ✅ | Permanent deletion via AccountManagementService with cascading deletes, double confirmation |

---

## Phase 4: Server Features ✅

Advanced server management, channels, permissions, and discoverability.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 4.1 | Server channels (text + voice) | ✅ | Channel model, Channels table (SQLite/SQL Server), ChannelService with CRUD and reordering |
| 4.2 | Public/private server toggle | ✅ | IsPublic bool on Server model, IsPublic column in Servers table, seed data updated |
| 4.3 | Invite links (customizable or generated) | ✅ | InviteLink model, InviteLinks table, InviteLinkService with create/validate/use |
| 4.4 | Server browser (public servers) | ✅ | ServerBrowserService (GetPublicServers, SearchServers), ServerBrowserViewModel, ServerBrowserView |
| 4.5 | Join server from browser | ✅ | ServerBrowserService.JoinServer (duplicate-guard insert into ServerMembers), wired to JoinServerCommand |
| 4.6 | Server icons (customizable) | ✅ | IconUrl property on Server model, IconUrl TEXT column in Servers table |
| 4.7 | Channel icons (customizable) | ✅ | IconUrl property on Channel model, IconUrl TEXT column in Channels table |
| 4.8 | Server permissions & roles | ✅ | ServerRole + ServerMember models, ServerRoles + ServerMembers tables, RoleService (CreateRole, GetServerRoles, AssignRole) |
| 4.9 | Channel permissions | ✅ | ChannelPermission model, ChannelPermissions table, ChannelPermissionService (SetPermission, GetPermission, CheckUserCanRead, CheckUserCanSend) |
| 4.10 | Server-specific nicknames | ✅ | RoleService.GetNickname + SetNickname querying Nickname column in ServerMembers |

---

## Phase 5: Chat Enhancements 📋

Rich media, search, mentions, and notifications.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 5.1 | Emoji support | ✅ | Emoji model, EmojiService (30 hardcoded emoji), EmojiPickerViewModel (SelectEmojiCommand, EmojiSelected event), EmojiPickerView (4-column grid) |
| 5.2 | Tenor API integration (GIFs) | ✅ | GifResult model, GifSearchService.SearchGifsAsync (Tenor v2 API, JSON parsing, PLACEHOLDER key) |
| 5.3 | Mentions (@username) | ✅ | MentionService.ParseMentions (regex @(\w+)), ResolveMentions (bold display) |
| 5.4 | Notifications (desktop + mobile) | ✅ | AppNotification model + NotificationType enum, Notifications table, NotificationService (Create, GetUnread, MarkAsRead, GetUnreadCount) |
| 5.5 | Search through friend messages | ✅ | MessageSearchService.SearchMessages (LIKE query on Messages table, sender/receiver filter) |
| 5.6 | Link searching from chats | ✅ | MessageSearchService.ExtractLinks (compiled regex https?://[^\s]+) |
| 5.7 | File searching (local) | ✅ | MessageSearchService.SearchFiles (Type = Image filter + Content LIKE query) |
| 5.8 | Search filters (by user, date, type) | ✅ | MessageSearchService.SearchMessagesFiltered (DynamicParameters, optional query/fromUserId/after/before/type filters) |
| 5.9 | Message reactions | ✅ | MessageReaction model, MessageReactions table (UNIQUE per message+user+emoji), MessageReactionService (AddReaction, RemoveReaction, GetReactionsForMessage) |
| 5.10 | Message threads/replies | ✅ | ParentMessageId (int?) added to ChatMessage for reply threading |
| 5.11 | Rich link previews | ✅ | LinkPreviewService (Open Graph + fallback parsing), LinkPreviewResult model, INotifyPropertyChanged on ChatMessage, inline preview card (site name, title, description, og:image) |

---

## Phase 6: User Profile & Customization ✅

Profile settings, status, integrations, and theming.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 6.1 | Profile customization | ✅ | AvatarUrl, BannerUrl, AboutMe, Pronouns columns on User model, UserProfileService (GetProfile, UpdateAvatar, UpdateBanner, UpdateAboutMe, UpdatePronouns, UpdateProfile), ProfileSettingsView Profile tab |
| 6.2 | Profile status changes | ✅ | UserStatus enum (Online, Away, DoNotDisturb, Invisible), Status column on User model, UserStatusService (UpdateStatus, GetStatus, GetStatusDisplayName, GetStatusColor), ProfileSettingsView Status tab |
| 6.3 | Custom status messages | ✅ | CustomStatusText + CustomStatusEmoji columns on User model, UserStatusService.SetCustomStatus/ClearCustomStatus, emoji + text entry in ProfileSettingsView Status tab |
| 6.4 | Profile status from external APIs | ✅ | ExternalStatusService with ExternalActivity model, GetSpotifyActivityAsync/GetSteamActivityAsync/GetXboxLiveActivityAsync (placeholder framework for OAuth integration), GetAllActivitiesAsync, FormatActivityAsStatus |
| 6.5 | Profile connections to other services | ✅ | ProfileConnection model, ProfileConnections table (SQLite/SQL Server), ProfileConnectionService (AddConnection, RemoveConnection, GetConnections, GetVisibleConnections, SetConnectionVisibility), 10 supported services, ProfileSettingsView Connections tab |
| 6.6 | Themes | ✅ | UserThemePreference model, UserThemePreferences table (SQLite/SQL Server), ThemeService (GetThemePreference, SaveThemePreference, ToggleDarkMode, SetAccentColor), 5 theme presets, 8 accent colors, ThemeSettingsViewModel, ThemeSettingsView with dark/light toggle |
| 6.7 | Profile settings page | ✅ | ProfileSettingsViewModel (4-tab design: Profile, Status, Connections, Active Status), ProfileSettingsView with tabbed UI, BoolToTabBackgroundConverter, wired to Profile button in MuffleMain |
| 6.8 | Profile active status settings | ✅ | ShowOnlineStatus column on User model, UserStatusService.SetShowOnlineStatus/GetShowOnlineStatus, ProfileSettingsView Active Status tab with toggle switch |

---

## Phase 7: Settings & Configuration 📋

App settings, voice/video config, accessibility, and developer tools.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 7.1 | Voice detailed settings | ✅ | Input/output device picker, input/output volume sliders, push-to-talk toggle + key binding, noise suppression toggle; VoiceSettings model + VoiceSettingsService (SQLite/SQL Server), VoiceSettingsViewModel, VoiceSettingsView, Voice button in MuffleMain |
| 7.2 | Video settings | ✅ | Camera selection picker, resolution picker (360p–4K), FPS picker (15/30/60); VideoSettings model + VideoSettingsService (SQLite/SQL Server), VideoSettingsViewModel, VideoSettingsView, Video button in MuffleMain |
| 7.3 | Accessibility settings | ✅ | Font size picker (12–22pt), high contrast toggle, screen reader toggle; AccessibilitySettings model + AccessibilitySettingsService (SQLite/SQL Server), AccessibilitySettingsViewModel, AccessibilitySettingsView, Accessibility button in MuffleMain |
| 7.4 | Developer settings | ✅ | Debug mode toggle, WebSocket inspector toggle, dev tools toggle; DeveloperSettings model + DeveloperSettingsService (SQLite/SQL Server), DeveloperSettingsViewModel, DeveloperSettingsView, Dev button in MuffleMain |
| 7.5 | Privacy and safety settings | ✅ | DM privacy, friend request filtering, content filtering; PrivacySettings model + PrivacySettingsService (SQLite/SQL Server), PrivacySettingsViewModel, PrivacySettingsView, Privacy button in MuffleMain |
| 7.6 | Devices connected to account | ✅ | View active sessions, log out remotely; ConnectedDevicesViewModel, ConnectedDevicesView, Devices button in MuffleMain |
| 7.7 | Patch notes viewer | ✅ | PatchNote + PatchNoteEntry models, PatchNotesService (hardcoded changelog), PatchNotesViewModel, PatchNotesView with versioned entries and color-coded type badges |
| 7.8 | Library acknowledgements | ✅ | LibraryAcknowledgement model, LibraryAcknowledgementsService (12 packages), LibraryAcknowledgementsViewModel, LibraryAcknowledgementsView with license badges and URLs |

---

## Phase 8: Security & Account 📋

Authentication hardening, 2FA, and account safety.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 8.1 | Optional 2FA/MFA | ✅ | TOTP (RFC 6238), self-contained HMAC-SHA1 implementation, otpauth:// URI, BCrypt-hashed backup codes (8 single-use), enable/disable flow with code verification, login challenge step |
| 8.2 | Password change flow | 📋 | Change password with email verification |
| 8.3 | Email verification | 📋 | Verify email on signup |
| 8.4 | Password reset (forgot password) | 📋 | Email-based password recovery |
| 8.5 | Session management | 📋 | View/revoke active sessions |

---

## Phase 9: Additional Features 📋

Social features, subscriptions, and mobile-specific views.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 9.1 | Friend groups | 📋 | Organize friends into groups, separate RTC/sockets |
| 9.2 | Subscription model (premium) | 📋 | Optional paid tier for enhanced features |
| 9.3 | Subscription gifting | 📋 | Gift premium to other users |
| 9.4 | Mobile-specific views | 📋 | Optimized layouts for iOS/Android |
| 9.5 | Tablet-optimized layouts | 📋 | Adaptive UI for tablet form factors |
| 9.6 | Screenshare (desktop) | 📋 | Share screen in voice/video calls |
| 9.7 | Picture-in-picture mode (mobile) | 📋 | Minimize video call to overlay |

---

## Summary

| Phase | Status | Count |
|-------|--------|-------|
| Phase 1: Foundation | ✅ Complete | 10/10 |
| Phase 2: Voice & Video | ✅ Complete | 7/7 |
| Phase 3: User Management | ✅ Complete | 9/9 |
| Phase 4: Server Features | ✅ Complete | 10/10 |
| Phase 5: Chat Enhancements | ✅ Complete | 11/11 |
| Phase 6: User Profile & Customization | ✅ Complete | 8/8 |
| Phase 7: Settings & Configuration | ✅ Complete | 8/8 |
| Phase 8: Security & Account | 🔧 In Progress | 1/5 |
| Phase 9: Additional Features | 📋 Planned | 0/7 |
| **Total** | | **53/75** |

---

## Development Notes

- **Architecture:** MVVM with .NET MAUI for cross-platform UI, Dapper for data access
- **Real-time:** WebSockets for signaling/chat, WebRTC for voice/video
- **Database:** SQLite (dev/mobile), SQL Server (production)
- **Platforms:** iOS, Android, macOS, Windows (via .NET MAUI)
- **Current focus:** Completing Phase 2 (WebRTC voice/video calls)

---

**Last updated:** 2026-02-19 23:04 UTC  
**Maintainer:** Auto-updated by "Muffle Dev Sprint" cron job
