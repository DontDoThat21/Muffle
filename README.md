# Muffle

An open-source, Discord-like communications platform for communities, friends, and teams. Muffle focuses on real-time voice, video, and chat experiences with privacy-first features and a modern UX.

Status: Feature-complete (v1.0 candidate). Contributions and feedback are welcome.

## Key capabilities

- WebRTC-based voice and video calls with screenshare and picture-in-picture
- Real-time chat with image sharing, GIF support, emoji, reactions, threads, and rich link previews
- Server management with channels, roles, permissions, and public server browser
- Full user authentication (registration, 2FA/MFA, email verification, password reset)
- Profile customization, themes, status, and external service connections (Spotify, Steam, etc.)
- Privacy, accessibility, voice, video, and developer settings
- Mobile and tablet-optimized layouts
- Subscription model with gifting

## Vision

Muffle aims to deliver a familiar, high-quality community chat experience with:
- Low-latency voice and video powered by WebRTC
- Server- and friend-centric conversations
- Searchable chat history with rich media
- Privacy- and safety-first defaults
- Extensibility and theme-ability

Not affiliated with Discord Inc.

<img width="2557" height="1388" alt="image" src="https://github.com/user-attachments/assets/080aeb70-326b-45b1-a442-4eeb03a957d7" />

## Roadmap and backlog

All 75 planned features across 9 development phases are now complete. The list below reflects the full implementation history.

### ✅ Phase 1 — Foundation
- [x] Add image support to chat
- [x] Add video support to chat
- [x] Real-time chat via WebSockets
- [x] Add dynamic server creations
- [x] Three-panel Discord-style layout (servers, friends, main content)
- [x] Database support (SQLite + SQL Server with Dapper ORM)

### ✅ Phase 2 — Voice & Video (WebRTC)
- [x] Add voice calls
- [x] Add video calls
- [x] WebRTC peer connection management (STUN, SDP negotiation, ICE exchange)
- [x] Call state UI (calling, connected, ended)

### ✅ Phase 3 — User Management
- [x] Add [User] account creation process
- [x] Remember user login
- [x] Add support for multiple [User] accounts
- [x] Add user incremented numbers to uniquely identify the same account names
- [x] Add 'add a friend' functionality from list of users or user id/tag
- [x] Add friend requests settings before adding
- [x] Add ability to block users
- [x] Disable account
- [x] Delete account

### ✅ Phase 4 — Server Features
- [x] Add server channels (separate web socket and rtc channels)
- [x] Modify server to be public or private with a customizable access link or (generated, chrono-expiring) invite link
- [x] Add server icons (customizable if owner)
- [x] Add server channel icons (customizable if channel or server owner)
- [x] Add/join a server from list of servers
- [x] Create a server and have it hidden from other users
- [x] Server browser — allow server searching for public servers
- [x] Server permissions & roles
- [x] Channel permissions
- [x] Add specific to server nicknames

### ✅ Phase 5 — Chat Enhancements
- [x] Add emoji support for chat messages
- [x] Add Tenor API integration (GIFs)
- [x] Add mentions/notifications
- [x] Add searching through all friend messages
- [x] Add link searching from chats
- [x] Add file searching (local)
- [x] Add search filters
- [x] Message reactions
- [x] Message threads/replies
- [x] Rich link previews

### ✅ Phase 6 — User Profile & Customization
- [x] Add profile customization
- [x] Add profile status changes
- [x] Custom status messages
- [x] Add profile status based off other app APIs (Spotify, Steam, etc.)
- [x] Add profile connections to other services (Steam, Battle.net, etc.)
- [x] Add themes
- [x] Add profile settings page
- [x] Add profile active status settings

### ✅ Phase 7 — Settings & Configuration
- [x] Add voice detailed settings
- [x] Add video settings
- [x] Add accessibility settings
- [x] Add developer settings
- [x] Add privacy and safety settings
- [x] Add devices connected to account
- [x] Add patch notes
- [x] Add usage acknowledgements of libraries used

### ✅ Phase 8 — Security & Account
- [x] Add optional 2FA/MFA (TOTP with backup codes)
- [x] Password change flow
- [x] Email verification on signup
- [x] Password reset (forgot password)
- [x] Session management (last-active, IP display, remote logout)

### ✅ Phase 9 — Additional Features
- [x] Add friend groups with separated RTC/sockets
- [x] Add unimplemented subscription model
- [x] Add subscription gifting
- [x] Add mobile-specific views
- [x] Tablet-optimized layouts
- [x] Screenshare (desktop)
- [x] Picture-in-picture mode (mobile)

## Architecture overview (high level)

- Real-time media: WebRTC for voice/video
- Signaling and messaging: WebSockets
- Servers and channels: Distinct RTC and socket channels per server/channel for isolation
- Clients: Desktop and mobile UIs planned, with mobile-specific views

More detailed architecture docs and diagrams will be added as the project evolves.

## Getting started

Detailed setup instructions will be documented as the codebase stabilizes. High-level steps:

1. Clone the repository
2. Configure environment variables (RTC, signaling, storage, auth)
3. Run the backend signaling/media services
4. Run the client(s)
5. Verify local voice/video calls and chat

If you run into issues, please open an issue with logs, environment, and reproduction steps.

## Contributing

We welcome contributions:
- Discuss ideas in issues
- Tackle items from the roadmap/backlog
- Submit pull requests with clear descriptions and test coverage where applicable

Before contributing:
- Follow the repository’s coding standards and commit conventions
- Keep features behind flags if experimental
- Avoid breaking existing call/chat flows

## Security and privacy

- Optional 2FA/MFA (TOTP with single-use backup codes) — implemented
- Privacy and safety settings — implemented
- Session management with last-active tracking and remote logout — implemented
- Please disclose security issues responsibly via a private channel (see SECURITY.md if available)

## License

TBD. See the LICENSE file if present, or open an issue to discuss licensing.

## Acknowledgements

This project will include a usage acknowledgements section for libraries and services used (e.g., media, signaling, emoji/tenor integrations).
