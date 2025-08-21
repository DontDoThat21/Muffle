# Muffle

An open-source, Discord-like communications platform for communities, friends, and teams. Muffle focuses on real-time voice, video, and chat experiences with privacy-first features and a modern UX.

Status: Early development (pre-alpha). Contributions and feedback are welcome.

## Key capabilities

- WebRTC-based server calls
- Video calls
- Image sharing
- Rich, extensible chat and server constructs (channels, permissions, presence) planned

Note: Some capabilities are partially implemented and under active development.

## Vision

Muffle aims to deliver a familiar, high-quality community chat experience with:
- Low-latency voice and video powered by WebRTC
- Server- and friend-centric conversations
- Searchable chat history with rich media
- Privacy- and safety-first defaults
- Extensibility and theme-ability

Not affiliated with Discord Inc.

## Roadmap and backlog

This backlog lists upcoming and existing features under active development. Some items may already be partially implemented; they are tracked here for improvements and remaining work.

- [ ] Add image support to chat
- [ ] Add video support to chat
- [ ] Add voice calls
- [ ] Add video calls
- [ ] Add dynamic server creations
- [ ] Add [User] account creation process
- [ ] Remember user login
- [ ] Add support for multiple [User] accounts
- [ ] Add emoji support for chat messages
- [ ] Add tenor API if free
- [ ] Add mobile specific views
- [ ] Add profile Customization
- [ ] Add server channels (separate web socket and rtc channels)
- [ ] Modify server to be public or not tag with a customizable access link or (generated. Chrono expiring) invite link
- [ ] Add user incremented numbers to uniquely identify the same account names
- [ ] Add ‘add a friend’ functionality from list of users or maybe user id/tag
- [ ] Add server icons (customizable if owner)
- [ ] Add server chanel icons (customizable if channel or server owner)
- [ ] Add/join a server from list of servers
- [ ] Create a server and have it hidden from other users
- [ ] Server browser - Allow server searching for public servers
- [ ] Add friend groups with separated rtc/sockets (custom view model constructor?)
- [ ] Add searching through all friend messages
- [ ] Add link searching from chats?
- [ ] Add file searching (local?)
- [ ] Add search filters
- [ ] Add mentions/notifications
- [ ] Add profile status changes
- [ ] Add profile status based off other app APIs (Spotify, steam etc)
- [ ] Add profile connections to other services (steam, bnet, etc)
- [ ] Add specific to server nicknames
- [ ] Add profile settings page
- [ ] Finish settings implementations
- [ ] Add friend requests settings before adding
- [ ] Add subscription gifting
- [ ] Add themes
- [ ] Add voice detailed settings
- [ ] Add accessibility settings
- [ ] Add developer settings
- [ ] Add patch notes
- [ ] Add usage acknowledgements of libraries used
- [ ] Add optional 2fa/mfa
- [ ] Add ability to block users
- [ ] Disable account
- [ ] Delete account
- [ ] Add privacy and safety settings
- [ ] Add profile active status settings
- [ ] Add devices connected to account
- [ ] Add unimplemented subscription model

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

- Optional 2FA/MFA planned
- Privacy and safety settings planned
- Please disclose security issues responsibly via a private channel (see SECURITY.md if available)

## License

TBD. See the LICENSE file if present, or open an issue to discuss licensing.

## Acknowledgements

This project will include a usage acknowledgements section for libraries and services used (e.g., media, signaling, emoji/tenor integrations).
