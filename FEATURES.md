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

## Phase 4: Server Features 📋

Advanced server management, channels, permissions, and discoverability.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 4.1 | Server channels (text + voice) | 📋 | Separate WebSocket and RTC channels per server |
| 4.2 | Public/private server toggle | 📋 | Server visibility setting |
| 4.3 | Invite links (customizable or generated) | 📋 | Chrono-expiring invite codes |
| 4.4 | Server browser (public servers) | 📋 | Searchable list of public servers |
| 4.5 | Join server from browser | 📋 | Click to join public servers |
| 4.6 | Server icons (customizable) | 📋 | Upload/change server avatar (owner only) |
| 4.7 | Channel icons (customizable) | 📋 | Upload/change channel avatar (channel/server owner) |
| 4.8 | Server permissions & roles | 📋 | Role-based access control (admin, moderator, member) |
| 4.9 | Channel permissions | 📋 | Per-channel view/send permissions |
| 4.10 | Server-specific nicknames | 📋 | Override display name per server |

---

## Phase 5: Chat Enhancements 📋

Rich media, search, mentions, and notifications.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 5.1 | Emoji support | 📋 | Emoji picker, Unicode emoji rendering |
| 5.2 | Tenor API integration (GIFs) | 📋 | GIF search + embed (if free API available) |
| 5.3 | Mentions (@username) | 📋 | @mention autocomplete, highlight mentioned users |
| 5.4 | Notifications (desktop + mobile) | 📋 | Push notifications for mentions, DMs, friend requests |
| 5.5 | Search through friend messages | 📋 | Full-text search across DM history |
| 5.6 | Link searching from chats | 📋 | Extract and search shared links |
| 5.7 | File searching (local) | 📋 | Search shared files/images |
| 5.8 | Search filters (by user, date, type) | 📋 | Advanced search with filters |
| 5.9 | Message reactions | 📋 | React to messages with emoji |
| 5.10 | Message threads/replies | 📋 | Reply to specific messages |
| 5.11 | Rich link previews | 📋 | Embed previews for URLs (title, image, description) |

---

## Phase 6: User Profile & Customization 📋

Profile settings, status, integrations, and theming.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 6.1 | Profile customization | 📋 | Avatar, banner, about me, pronouns |
| 6.2 | Profile status changes | 📋 | Online, away, do not disturb, invisible |
| 6.3 | Custom status messages | 📋 | "Playing X", "Listening to Y" |
| 6.4 | Profile status from external APIs | 📋 | Spotify, Steam, Xbox Live integration |
| 6.5 | Profile connections to other services | 📋 | Link Steam, Battle.net, Twitch, etc. |
| 6.6 | Themes | 📋 | Light/dark mode, custom theme support |
| 6.7 | Profile settings page | 📋 | Edit profile, connections, status |
| 6.8 | Profile active status settings | 📋 | Show/hide online status |

---

## Phase 7: Settings & Configuration 📋

App settings, voice/video config, accessibility, and developer tools.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 7.1 | Voice detailed settings | 📋 | Input/output device, push-to-talk, noise suppression |
| 7.2 | Video settings | 📋 | Camera selection, resolution, FPS |
| 7.3 | Accessibility settings | 📋 | Font size, high contrast, screen reader support |
| 7.4 | Developer settings | 📋 | Debug mode, WebSocket inspector, enable dev tools |
| 7.5 | Privacy and safety settings | 📋 | DM privacy, friend request filtering, content filtering |
| 7.6 | Devices connected to account | 📋 | View active sessions, log out remotely |
| 7.7 | Patch notes viewer | 📋 | Display app changelog in-app |
| 7.8 | Library acknowledgements | 📋 | Credits for open-source libraries used |

---

## Phase 8: Security & Account 📋

Authentication hardening, 2FA, and account safety.

| ID | Feature | Status | Notes |
|----|---------|--------|-------|
| 8.1 | Optional 2FA/MFA | 📋 | TOTP (Google Authenticator, Authy) |
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
| Phase 4: Server Features | 📋 Planned | 0/10 |
| Phase 5: Chat Enhancements | 📋 Planned | 0/11 |
| Phase 6: User Profile & Customization | 📋 Planned | 0/8 |
| Phase 7: Settings & Configuration | 📋 Planned | 0/8 |
| Phase 8: Security & Account | 📋 Planned | 0/5 |
| Phase 9: Additional Features | 📋 Planned | 0/7 |
| **Total** | | **26/75** |

---

## Development Notes

- **Architecture:** MVVM with .NET MAUI for cross-platform UI, Dapper for data access
- **Real-time:** WebSockets for signaling/chat, WebRTC for voice/video
- **Database:** SQLite (dev/mobile), SQL Server (production)
- **Platforms:** iOS, Android, macOS, Windows (via .NET MAUI)
- **Current focus:** Completing Phase 2 (WebRTC voice/video calls)

---

**Last updated:** 2026-02-19 19:46 UTC  
**Maintainer:** Auto-updated by "Muffle Dev Sprint" cron job
