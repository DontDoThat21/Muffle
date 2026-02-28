# Muffle Agent Task Queue

Rules:
- Pick the FIRST ⬜ task only.
- Do exactly what it says, nothing more.
- Run the build command shown in the task (default: `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`).
- If exit 0: mark ⬜ → ✅, apply any FEATURES.md update noted, stop.
- If exit 1: mark ⬜ → ❌, log error to memory/YYYY-MM-DD.md, stop.
- Maximum 5 tool calls per task. Stop after build result.

Legend: ⬜ pending · ✅ done · ❌ failed

---

## Phase 3 — User Management (verify existing code, update FEATURES.md)

Phase 3 services, models, ViewModels, and Views already exist. These tasks verify the data-layer build and update FEATURES.md status.

⬜ TASK-001: Verify registration. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 3.1 from 📋 to ✅. Note: AuthenticationService.RegisterUser, RegistrationViewModel, RegistrationView, AuthenticationPage all exist.

⬜ TASK-002: Verify remember-login. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 3.2 from 📋 to ✅. Note: TokenStorageService with SecureStorage, App.xaml.cs auto-login, AuthToken model all exist.

⬜ TASK-003: Verify multiple-account support. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 3.3 from 📋 to ✅. Note: StoredAccount model, AccountSwitcherViewModel, AccountSwitcherView, TokenStorageService multi-account methods all exist.

⬜ TASK-004: Verify add-friend functionality. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 3.4 from 📋 to ✅. Note: AddFriendViewModel, AddFriendView, FriendRequestService.SearchUsers, FriendRequestService.SendFriendRequest all exist.

⬜ TASK-005: Verify friend-requests flow. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 3.5 from 📋 to ✅. Note: FriendRequestService (send/accept/decline/cancel), FriendRequestsViewModel, FriendRequestsView all exist.

⬜ TASK-006: Verify user discriminator. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 3.6 from 📋 to ✅. Note: User.Discriminator, User.FullUsername, AuthenticationService.GenerateDiscriminator all exist.

⬜ TASK-007: Verify block-users. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 3.7 from 📋 to ✅. Note: BlockService (Block/Unblock/IsBlocked/GetBlockedUsers), BlockedUsersViewModel, BlockedUsersView, BlockedUser model all exist.

⬜ TASK-008: Verify disable-account. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 3.8 from 📋 to ✅. Note: AccountManagementService.DisableAccount/EnableAccount, AccountSettingsViewModel, AccountSettingsView all exist.

⬜ TASK-009: Verify delete-account. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 3.9 from 📋 to ✅. Note: AccountManagementService.DeleteAccount, AccountSettingsView all exist. Also update FEATURES.md summary — Phase 3 from 0/9 to 9/9, status ✅ Complete.

---

## Phase 2 — Voice & Video (complete remaining gaps)

WebRTCManager, signaling, and message routing are implemented. SDP offer/answer, ICE exchange, and media tracks work. Remaining: verify existing, then build call-overlay UI.

⬜ TASK-010: Verify WebRTC peer connection management. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 2.3 from 🔧 to ✅. Note: WebRTCManager has full STUN config, CreateOffer, CreateAnswer, ICE handlers.

⬜ TASK-011: Verify SDP offer/answer negotiation. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 2.4 from 📋 to ✅. Note: WebRTCManager.StartCallAsync creates offer, AcceptCallAsync creates answer, HandleAnswerAsync applies answer. FriendDetailsContentViewModel routes WebRtcOffer/WebRtcAnswer messages.

⬜ TASK-012: Verify ICE candidate exchange. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 2.5 from 📋 to ✅. Note: WebRTCManager.OnIceCandidateHandler sends candidates, HandleIceCandidateAsync adds remote candidates. FriendDetailsContentViewModel routes IceCandidate messages.

⬜ TASK-013: Verify media track management. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 2.6 from 📋 to ✅. Note: WebRTCManager.InitializeAsync calls GetUserMedia with audio/video constraints, adds tracks via AddTrack, handles OnTrack for remote streams.

⬜ TASK-014: Verify voice calls. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 2.1 from 🔧 to ✅. Note: FriendDetailsContentViewModel.StartVoiceCallAsync creates WebRTCManager and calls StartCallAsync(includeVideo: false).

⬜ TASK-015: Verify video calls. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: update FEATURES.md — feature 2.2 from 🔧 to ✅. Note: FriendDetailsContentViewModel.StartVideoCallAsync creates WebRTCManager and calls StartCallAsync(includeVideo: true).

⬜ TASK-016: Create Muffle/ViewModels/CallOverlayViewModel.cs — ViewModel with CallState property (bound to WebRTCManager.CurrentCallState), CallerName string, MuteCommand (toggles IsMuted bool), EndCallCommand (calls WebRTCManager.EndCallAsync), ToggleVideoCommand (toggles IsVideoEnabled bool). Implement BindableObject. One file only.
On success: update FEATURES.md — feature 2.7 from 📋 to 🔧.

⬜ TASK-017: Create Muffle/Views/CallOverlayView.xaml — MAUI ContentView showing: call state Label bound to CallState, caller name Label bound to CallerName, three ImageButtons (Mute, End Call, Toggle Video) bound to commands. Use a semi-transparent background overlay. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-018: Create Muffle/Views/CallOverlayView.xaml.cs — code-behind setting BindingContext to new CallOverlayViewModel(). One file only.
On success: update FEATURES.md — feature 2.7 from 🔧 to ✅. Also update FEATURES.md summary — Phase 2 from 2/7 to 7/7, status ✅ Complete.

---

## Phase 4 — Server Features

### 4.1 Server channels (text + voice)

Channel model, ChannelType enum, and ChannelService already exist with CreateChannel, GetServerChannels, DeleteChannel, UpdateChannel. Channels table DDL exists.

⬜ TASK-019: Verify ChannelService builds. Run `dotnet build Muffle.Data/Muffle.Data.csproj 2>&1`.
On success: no FEATURES.md change yet (need UI).

⬜ TASK-020: Create Muffle/ViewModels/ChannelListViewModel.cs — ObservableCollection<Channel> Channels, SelectedChannel property, LoadChannelsAsync(int serverId) calling ChannelService.GetServerChannels, CreateChannelCommand. Implement BindableObject. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-021: Create Muffle/Views/ChannelListView.xaml — MAUI ContentView with CollectionView of channels showing DisplayName property, and a "+" Button for creating channels. Bind to ChannelListViewModel. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-022: Create Muffle/Views/ChannelListView.xaml.cs — code-behind setting BindingContext to new ChannelListViewModel(). One file only.
On success: update FEATURES.md — feature 4.1 from 📋 to ✅.

### 4.2 Public/private server toggle

✅ TASK-023: In Muffle.Data/Models/Server.cs — add `public bool IsPublic { get; set; }` property after the Port property. One file only.
On success: no FEATURES.md change needed.

✅ TASK-024: In Muffle.Data/Services/SqliteDbService.cs — add `IsPublic INTEGER NOT NULL DEFAULT 0` column to the Servers CREATE TABLE statement, after the Port column. One file only.
On success: no FEATURES.md change needed.

✅ TASK-025: In Muffle.Data/Services/SqliteDbService.cs — update the Servers seed INSERT to include IsPublic values. Set server Id=0 to IsPublic=1, others to 0. Add IsPublic to the column list and VALUES. One file only.
On success: update FEATURES.md — feature 4.2 from 📋 to ✅.

### 4.3 Invite links

✅ TASK-026: Create Muffle.Data/Models/InviteLink.cs — properties: InviteLinkId (int), ServerId (int), Code (string), CreatedBy (int), CreatedAt (DateTime), ExpiresAt (DateTime?), MaxUses (int?), UseCount (int, default 0). One file only.
On success: no FEATURES.md change needed.

✅ TASK-027: In Muffle.Data/Services/SqliteDbService.cs — add CREATE TABLE IF NOT EXISTS InviteLinks DDL (InviteLinkId INTEGER PRIMARY KEY AUTOINCREMENT, ServerId INTEGER NOT NULL, Code TEXT NOT NULL UNIQUE, CreatedBy INTEGER NOT NULL, CreatedAt DATETIME NOT NULL, ExpiresAt DATETIME, MaxUses INTEGER, UseCount INTEGER NOT NULL DEFAULT 0, FK to Servers and Users). Also add `DROP TABLE IF EXISTS InviteLinks` to DisposeDatabase method. One file only.
On success: no FEATURES.md change needed.

✅ TASK-028: Create Muffle.Data/Services/InviteLinkService.cs — static class with CreateInviteLink(int serverId, int createdBy, DateTime? expiresAt, int? maxUses) returning InviteLink?. Generate Code via Guid.NewGuid().ToString("N")[..8]. INSERT into InviteLinks, return created object. Use Dapper + SQLiteDbService.CreateConnection(). One file only.
On success: no FEATURES.md change needed.

✅ TASK-029: In Muffle.Data/Services/InviteLinkService.cs — add GetInviteLinkByCode(string code) returning InviteLink? and ValidateInviteLink(string code) returning bool (checks ExpiresAt > now and UseCount < MaxUses or MaxUses is null). One file only.
On success: no FEATURES.md change needed.

✅ TASK-030: In Muffle.Data/Services/InviteLinkService.cs — add UseInviteLink(string code, int userId) method. Validates link, increments UseCount, inserts into ServerMembers table (ServerId, UserId, JoinedAt=now). Returns bool success. One file only.
On success: update FEATURES.md — feature 4.3 from 📋 to ✅.

### 4.4 Server browser (public servers)

⬜ TASK-031: Create Muffle.Data/Services/ServerBrowserService.cs — static class with GetPublicServers() returning List<Server>: `SELECT * FROM Servers WHERE IsPublic = 1`. Use Dapper + SQLiteDbService.CreateConnection(). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-032: In Muffle.Data/Services/ServerBrowserService.cs — add SearchServers(string query) returning List<Server>: `SELECT * FROM Servers WHERE IsPublic = 1 AND (Name LIKE @q OR Description LIKE @q)`. Parameter @q = $"%{query}%". One file only.
On success: no FEATURES.md change needed.

⬜ TASK-033: Create Muffle/ViewModels/ServerBrowserViewModel.cs — ObservableCollection<Server> PublicServers, SearchText string, SearchCommand calling ServerBrowserService.SearchServers, LoadCommand calling GetPublicServers, JoinServerCommand. Implement BindableObject. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-034: Create Muffle/Views/ServerBrowserView.xaml — MAUI ContentPage with SearchBar bound to SearchText, CollectionView of PublicServers showing Name + Description, and a "Join" Button per item. Bind to ServerBrowserViewModel. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-035: Create Muffle/Views/ServerBrowserView.xaml.cs — code-behind setting BindingContext to new ServerBrowserViewModel(). One file only.
On success: update FEATURES.md — feature 4.4 from 📋 to ✅.

### 4.5 Join server from browser

⬜ TASK-036: In Muffle.Data/Services/ServerBrowserService.cs — add JoinServer(int serverId, int userId) static method. INSERT INTO ServerMembers (ServerId, UserId, JoinedAt) VALUES (...) if not already a member. Return bool success. One file only.
On success: update FEATURES.md — feature 4.5 from 📋 to ✅.

### 4.6 Server icons

⬜ TASK-037: In Muffle.Data/Models/Server.cs — add `public string? IconUrl { get; set; }` property after IsPublic. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-038: In Muffle.Data/Services/SqliteDbService.cs — add `IconUrl TEXT` column to the Servers CREATE TABLE statement. One file only.
On success: update FEATURES.md — feature 4.6 from 📋 to ✅.

### 4.7 Channel icons

⬜ TASK-039: In Muffle.Data/Models/Channel.cs — add `public string? IconUrl { get; set; }` property after CreatedBy. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-040: In Muffle.Data/Services/SqliteDbService.cs — add `IconUrl TEXT` column to the Channels CREATE TABLE statement. One file only.
On success: update FEATURES.md — feature 4.7 from 📋 to ✅.

### 4.8 Server permissions & roles

⬜ TASK-041: Create Muffle.Data/Models/ServerRole.cs — properties: RoleId (int), ServerId (int), Name (string), Permissions (int, bitflags: 1=Read, 2=Send, 4=Manage, 8=Admin), Position (int), Color (string?). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-042: Create Muffle.Data/Models/ServerMember.cs — properties: ServerId (int), UserId (int), RoleId (int?), Nickname (string?), JoinedAt (DateTime). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-043: In Muffle.Data/Services/SqliteDbService.cs — add CREATE TABLE IF NOT EXISTS ServerRoles DDL (RoleId INTEGER PRIMARY KEY AUTOINCREMENT, ServerId INTEGER NOT NULL, Name TEXT NOT NULL, Permissions INTEGER NOT NULL DEFAULT 1, Position INTEGER NOT NULL DEFAULT 0, Color TEXT, FK to Servers). Also add DROP TABLE IF EXISTS ServerRoles to DisposeDatabase. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-044: In Muffle.Data/Services/SqliteDbService.cs — add CREATE TABLE IF NOT EXISTS ServerMembers DDL (ServerId INTEGER NOT NULL, UserId INTEGER NOT NULL, RoleId INTEGER, Nickname TEXT, JoinedAt DATETIME NOT NULL, PRIMARY KEY (ServerId, UserId), FK to Servers, Users, ServerRoles). Also add DROP TABLE IF EXISTS ServerMembers to DisposeDatabase. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-045: Create Muffle.Data/Services/RoleService.cs — static class with CreateRole(int serverId, string name, int permissions) returning ServerRole?, GetServerRoles(int serverId) returning List<ServerRole>, AssignRole(int serverId, int userId, int roleId) returning bool. Use Dapper + SQLiteDbService.CreateConnection(). One file only.
On success: update FEATURES.md — feature 4.8 from 📋 to ✅.

### 4.9 Channel permissions

⬜ TASK-046: Create Muffle.Data/Models/ChannelPermission.cs — properties: ChannelId (int), RoleId (int), AllowRead (bool), AllowSend (bool), AllowManage (bool). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-047: In Muffle.Data/Services/SqliteDbService.cs — add CREATE TABLE IF NOT EXISTS ChannelPermissions DDL (ChannelId INTEGER NOT NULL, RoleId INTEGER NOT NULL, AllowRead INTEGER NOT NULL DEFAULT 1, AllowSend INTEGER NOT NULL DEFAULT 1, AllowManage INTEGER NOT NULL DEFAULT 0, PRIMARY KEY (ChannelId, RoleId), FK to Channels and ServerRoles). Also add DROP TABLE IF EXISTS ChannelPermissions to DisposeDatabase. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-048: Create Muffle.Data/Services/ChannelPermissionService.cs — static class with SetPermission(int channelId, int roleId, bool read, bool send, bool manage), GetPermission(int channelId, int roleId) returning ChannelPermission?, CheckUserCanRead(int channelId, int userId) returning bool, CheckUserCanSend(int channelId, int userId) returning bool. Use Dapper. One file only.
On success: update FEATURES.md — feature 4.9 from 📋 to ✅.

### 4.10 Server-specific nicknames

⬜ TASK-049: In Muffle.Data/Services/RoleService.cs — add GetNickname(int serverId, int userId) returning string? and SetNickname(int serverId, int userId, string? nickname) returning bool. Query/update the Nickname column in ServerMembers table. One file only.
On success: update FEATURES.md — feature 4.10 from 📋 to ✅. Also update FEATURES.md summary — Phase 4 from 0/10 to 10/10, status ✅ Complete.

---

## Phase 5 — Chat Enhancements

### 5.1 Emoji support

⬜ TASK-050: Create Muffle.Data/Models/Emoji.cs — properties: Code (string, e.g. ":smile:"), Unicode (string, e.g. "😄"), Category (string), Name (string). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-051: Create Muffle.Data/Services/EmojiService.cs — static class with GetAllEmojis() returning a hardcoded List<Emoji> of ~30 common emoji (smile, heart, thumbsup, fire, laugh, cry, etc. with their Unicode chars). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-052: Create Muffle/ViewModels/EmojiPickerViewModel.cs — ObservableCollection<Emoji> Emojis loaded from EmojiService.GetAllEmojis(), SelectEmojiCommand of type Command<Emoji>, event Action<string> EmojiSelected that fires with Unicode. Implement BindableObject. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-053: Create Muffle/Views/EmojiPickerView.xaml — MAUI ContentView with CollectionView using GridItemsLayout (4 columns) showing emoji Unicode as Button text. Bind to EmojiPickerViewModel. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-054: Create Muffle/Views/EmojiPickerView.xaml.cs — code-behind setting BindingContext to new EmojiPickerViewModel(). One file only.
On success: update FEATURES.md — feature 5.1 from 📋 to ✅.

### 5.2 Tenor API integration (GIFs)

⬜ TASK-055: Create Muffle.Data/Models/GifResult.cs — properties: Id (string), PreviewUrl (string), FullUrl (string), Title (string). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-056: Create Muffle.Data/Services/GifSearchService.cs — static class with SearchGifsAsync(string query) returning Task<List<GifResult>>. Use HttpClient to GET `https://tenor.googleapis.com/v2/search?q={query}&key=PLACEHOLDER&limit=20`. Parse JSON results. One file only.
On success: update FEATURES.md — feature 5.2 from 📋 to ✅.

### 5.3 Mentions (@username)

⬜ TASK-057: Create Muffle.Data/Services/MentionService.cs — static class with ParseMentions(string messageContent) returning List<string> of mentioned usernames (regex: @(\w+)), and ResolveMentions(string content) that replaces @name with bolded display. One file only.
On success: update FEATURES.md — feature 5.3 from 📋 to ✅.

### 5.4 Notifications

⬜ TASK-058: Create Muffle.Data/Models/AppNotification.cs — enum NotificationType { Mention, DirectMessage, FriendRequest, ServerInvite }. Class properties: NotificationId (int), UserId (int), Title (string), Body (string), Type (NotificationType), IsRead (bool), CreatedAt (DateTime), RelatedId (int?, e.g. senderId or serverId). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-059: In Muffle.Data/Services/SqliteDbService.cs — add CREATE TABLE IF NOT EXISTS Notifications DDL (NotificationId INTEGER PRIMARY KEY AUTOINCREMENT, UserId INTEGER NOT NULL, Title TEXT NOT NULL, Body TEXT, Type INTEGER NOT NULL, IsRead INTEGER NOT NULL DEFAULT 0, CreatedAt DATETIME NOT NULL, RelatedId INTEGER, FK to Users). Also add DROP TABLE IF EXISTS Notifications to DisposeDatabase. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-060: Create Muffle.Data/Services/NotificationService.cs — static class with CreateNotification(int userId, string title, string body, NotificationType type, int? relatedId), GetUnreadNotifications(int userId) returning List<AppNotification>, MarkAsRead(int notificationId), GetUnreadCount(int userId). Use Dapper. One file only.
On success: update FEATURES.md — feature 5.4 from 📋 to ✅.

### 5.5 Search through friend messages

⬜ TASK-061: Create Muffle.Data/Services/MessageSearchService.cs — static class with SearchMessages(int userId, string query) returning List<ChatMessage>. SQL: `SELECT * FROM Messages WHERE (SenderId = @userId OR ReceiverId = @userId) AND Content LIKE @q ORDER BY Timestamp DESC LIMIT 50`. Use Dapper. One file only.
On success: update FEATURES.md — feature 5.5 from 📋 to ✅.

### 5.6 Link searching from chats

⬜ TASK-062: In Muffle.Data/Services/MessageSearchService.cs — add ExtractLinks(string content) static method using Regex `https?://[^\s]+` to find URLs, return List<string>. One file only.
On success: update FEATURES.md — feature 5.6 from 📋 to ✅.

### 5.7 File searching (local)

⬜ TASK-063: In Muffle.Data/Services/MessageSearchService.cs — add SearchFiles(int userId, string query) static method. SQL: `SELECT * FROM Messages WHERE (SenderId = @userId OR ReceiverId = @userId) AND Type = @imageType AND Content LIKE @q LIMIT 50`. One file only.
On success: update FEATURES.md — feature 5.7 from 📋 to ✅.

### 5.8 Search filters (by user, date, type)

⬜ TASK-064: In Muffle.Data/Services/MessageSearchService.cs — add SearchMessagesFiltered(int userId, string? query, int? fromUserId, DateTime? after, DateTime? before, MessageType? type) returning List<ChatMessage>. Build WHERE clause dynamically with Dapper DynamicParameters. One file only.
On success: update FEATURES.md — feature 5.8 from 📋 to ✅.

### 5.9 Message reactions

⬜ TASK-065: Create Muffle.Data/Models/MessageReaction.cs — properties: ReactionId (int), MessageId (int), UserId (int), Emoji (string), CreatedAt (DateTime). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-066: In Muffle.Data/Services/SqliteDbService.cs — add CREATE TABLE IF NOT EXISTS MessageReactions DDL (ReactionId INTEGER PRIMARY KEY AUTOINCREMENT, MessageId INTEGER NOT NULL, UserId INTEGER NOT NULL, Emoji TEXT NOT NULL, CreatedAt DATETIME NOT NULL, UNIQUE(MessageId, UserId, Emoji)). Also add DROP TABLE IF EXISTS MessageReactions to DisposeDatabase. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-067: Create Muffle.Data/Services/MessageReactionService.cs — static class with AddReaction(int messageId, int userId, string emoji), RemoveReaction(int messageId, int userId, string emoji), GetReactionsForMessage(int messageId) returning List<MessageReaction>. Use Dapper. One file only.
On success: update FEATURES.md — feature 5.9 from 📋 to ✅.

### 5.10 Message threads/replies

⬜ TASK-068: In Muffle.Data/Models/ChatMessage.cs — add `public int? ParentMessageId { get; set; }` property for reply threading. One file only.
On success: update FEATURES.md — feature 5.10 from 📋 to ✅.

### 5.11 Rich link previews

⬜ TASK-069: Create Muffle.Data/Models/LinkPreview.cs — properties: Url (string), Title (string?), Description (string?), ImageUrl (string?), SiteName (string?). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-070: Create Muffle.Data/Services/LinkPreviewService.cs — static class with FetchPreviewAsync(string url) returning Task<LinkPreview?>. Use HttpClient to GET url, parse HTML with Regex for `<meta property="og:title" content="(.*?)"/>`, og:description, og:image, og:site_name. One file only.
On success: update FEATURES.md — feature 5.11 from 📋 to ✅. Also update FEATURES.md summary — Phase 5 from 0/11 to 11/11, status ✅ Complete.

---

## Phase 6 — User Profile & Customization

### 6.1 Profile customization

⬜ TASK-071: In Muffle.Data/Models/User.cs — add properties: `public string? AvatarUrl { get; set; }`, `public string? BannerUrl { get; set; }`, `public string? AboutMe { get; set; }`, `public string? Pronouns { get; set; }`. Add after the DisabledAt property. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-072: In Muffle.Data/Services/SqliteDbService.cs — add `AvatarUrl TEXT, BannerUrl TEXT, AboutMe TEXT, Pronouns TEXT` columns to the Users CREATE TABLE statement, after the DisabledAt column. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-073: Create Muffle.Data/Services/UserProfileService.cs — static class with UpdateProfile(int userId, string? avatarUrl, string? bannerUrl, string? aboutMe, string? pronouns) and GetProfile(int userId) returning User?. Use Dapper UPDATE/SELECT on Users table. One file only.
On success: update FEATURES.md — feature 6.1 from 📋 to ✅.

### 6.2 Profile status changes

⬜ TASK-074: Create Muffle.Data/Models/UserStatus.cs — public enum UserStatus { Online = 0, Away = 1, DoNotDisturb = 2, Invisible = 3 }. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-075: In Muffle.Data/Models/User.cs — add `public UserStatus Status { get; set; } = UserStatus.Online;` property. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-076: In Muffle.Data/Services/SqliteDbService.cs — add `Status INTEGER NOT NULL DEFAULT 0` column to the Users CREATE TABLE statement. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-077: In Muffle.Data/Services/UserProfileService.cs — add UpdateStatus(int userId, UserStatus status) and GetStatus(int userId) returning UserStatus. Use Dapper. One file only.
On success: update FEATURES.md — feature 6.2 from 📋 to ✅.

### 6.3 Custom status messages

⬜ TASK-078: In Muffle.Data/Models/User.cs — add `public string? CustomStatusMessage { get; set; }` property. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-079: In Muffle.Data/Services/SqliteDbService.cs — add `CustomStatusMessage TEXT` column to the Users CREATE TABLE statement. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-080: In Muffle.Data/Services/UserProfileService.cs — add UpdateCustomStatus(int userId, string? message) method. UPDATE Users SET CustomStatusMessage = @message WHERE UserId = @userId. One file only.
On success: update FEATURES.md — feature 6.3 from 📋 to ✅.

### 6.4 Profile status from external APIs

⬜ TASK-081: Create Muffle.Data/Models/ExternalActivity.cs — properties: ServiceName (string, e.g. "Spotify"), ActivityText (string), DetailText (string?), IconUrl (string?), StartedAt (DateTime). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-082: Create Muffle.Data/Services/ExternalActivityService.cs — static class with private static Dictionary<int, ExternalActivity> _activities field. Methods: SetActivity(int userId, ExternalActivity activity), GetActivity(int userId) returning ExternalActivity?, ClearActivity(int userId). In-memory storage. One file only.
On success: update FEATURES.md — feature 6.4 from 📋 to ✅.

### 6.5 Profile connections to other services

⬜ TASK-083: Create Muffle.Data/Models/ProfileConnection.cs — properties: ConnectionId (int), UserId (int), ServiceName (string), ServiceUserId (string), ServiceUsername (string), ConnectedAt (DateTime). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-084: In Muffle.Data/Services/SqliteDbService.cs — add CREATE TABLE IF NOT EXISTS ProfileConnections DDL (ConnectionId INTEGER PRIMARY KEY AUTOINCREMENT, UserId INTEGER NOT NULL, ServiceName TEXT NOT NULL, ServiceUserId TEXT NOT NULL, ServiceUsername TEXT NOT NULL, ConnectedAt DATETIME NOT NULL, FK to Users, UNIQUE(UserId, ServiceName)). Also add DROP TABLE IF EXISTS ProfileConnections to DisposeDatabase. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-085: Create Muffle.Data/Services/ProfileConnectionService.cs — static class with AddConnection(int userId, string serviceName, string serviceUserId, string serviceUsername) returning ProfileConnection?, RemoveConnection(int connectionId), GetConnections(int userId) returning List<ProfileConnection>. Use Dapper. One file only.
On success: update FEATURES.md — feature 6.5 from 📋 to ✅.

### 6.6 Themes

⬜ TASK-086: Create Muffle.Data/Models/ThemeSettings.cs — public enum ThemeMode { Light, Dark, Custom }. Class properties: Mode (ThemeMode), AccentColor (string?, hex), FontScale (double, default 1.0). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-087: Create Muffle.Data/Services/ThemeService.cs — static class with GetTheme() returning ThemeSettings (read from Preferences keys "theme_mode", "theme_accent", "theme_fontscale") and SaveTheme(ThemeSettings). Use Microsoft.Maui.Storage.Preferences. One file only.
On success: update FEATURES.md — feature 6.6 from 📋 to ✅.

### 6.7 Profile settings page

⬜ TASK-088: Create Muffle/ViewModels/ProfileSettingsViewModel.cs — properties for AvatarUrl, BannerUrl, AboutMe, Pronouns, SelectedStatus (UserStatus), CustomStatusMessage, ShowOnlineStatus. SaveCommand calling UserProfileService.UpdateProfile and UpdateStatus. Implement BindableObject. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-089: Create Muffle/Views/ProfileSettingsView.xaml — MAUI ContentPage with Entry fields: AvatarUrl, BannerUrl, AboutMe, Pronouns, Picker for Status, Entry for CustomStatusMessage, Switch for ShowOnlineStatus, and a Save Button. Bind to ProfileSettingsViewModel. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-090: Create Muffle/Views/ProfileSettingsView.xaml.cs — code-behind setting BindingContext to new ProfileSettingsViewModel(). One file only.
On success: update FEATURES.md — feature 6.7 from 📋 to ✅.

### 6.8 Profile active status settings

⬜ TASK-091: In Muffle.Data/Models/User.cs — add `public bool ShowOnlineStatus { get; set; } = true;` property. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-092: In Muffle.Data/Services/SqliteDbService.cs — add `ShowOnlineStatus INTEGER NOT NULL DEFAULT 1` column to the Users CREATE TABLE statement. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-093: In Muffle.Data/Services/UserProfileService.cs — add UpdateShowOnlineStatus(int userId, bool show) method. UPDATE Users SET ShowOnlineStatus = @show WHERE UserId = @userId. One file only.
On success: update FEATURES.md — feature 6.8 from 📋 to ✅. Also update FEATURES.md summary — Phase 6 from 0/8 to 8/8, status ✅ Complete.

---

## Phase 7 — Settings & Configuration

### 7.1 Voice detailed settings

⬜ TASK-094: Create Muffle.Data/Models/VoiceSettings.cs — properties: InputDeviceId (string?), OutputDeviceId (string?), InputVolume (double, default 1.0), OutputVolume (double, default 1.0), PushToTalk (bool, default false), PushToTalkKey (string?), NoiseSuppression (bool, default true), EchoCancellation (bool, default true). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-095: Create Muffle.Data/Services/SettingsService.cs — static class with GetVoiceSettings() returning VoiceSettings and SaveVoiceSettings(VoiceSettings). Serialize to JSON, store via Preferences with key "voice_settings". One file only.
On success: update FEATURES.md — feature 7.1 from 📋 to ✅.

### 7.2 Video settings

⬜ TASK-096: Create Muffle.Data/Models/VideoSettings.cs — properties: CameraDeviceId (string?), Resolution (string, default "720p"), FrameRate (int, default 30). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-097: In Muffle.Data/Services/SettingsService.cs — add GetVideoSettings() returning VideoSettings and SaveVideoSettings(VideoSettings). Serialize to JSON, store via Preferences with key "video_settings". One file only.
On success: update FEATURES.md — feature 7.2 from 📋 to ✅.

### 7.3 Accessibility settings

⬜ TASK-098: Create Muffle.Data/Models/AccessibilitySettings.cs — properties: FontScale (double, default 1.0), HighContrast (bool, default false), ReduceMotion (bool, default false), ScreenReaderOptimized (bool, default false). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-099: In Muffle.Data/Services/SettingsService.cs — add GetAccessibilitySettings() returning AccessibilitySettings and SaveAccessibilitySettings(AccessibilitySettings). Serialize to JSON, key "accessibility_settings". One file only.
On success: update FEATURES.md — feature 7.3 from 📋 to ✅.

### 7.4 Developer settings

⬜ TASK-100: Create Muffle.Data/Models/DeveloperSettings.cs — properties: DebugMode (bool, default false), ShowWebSocketInspector (bool, default false), EnableDevTools (bool, default false), VerboseLogging (bool, default false). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-101: In Muffle.Data/Services/SettingsService.cs — add GetDeveloperSettings() returning DeveloperSettings and SaveDeveloperSettings(DeveloperSettings). Serialize to JSON, key "developer_settings". One file only.
On success: update FEATURES.md — feature 7.4 from 📋 to ✅.

### 7.5 Privacy and safety settings

⬜ TASK-102: Create Muffle.Data/Models/PrivacySettings.cs — public enum ContentFilterLevel { None = 0, Low = 1, Medium = 2, High = 3 }. Class properties: AllowDMsFromAnyone (bool, default true), AllowFriendRequestsFromAnyone (bool, default true), FilterLevel (ContentFilterLevel, default Medium). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-103: In Muffle.Data/Services/SettingsService.cs — add GetPrivacySettings() returning PrivacySettings and SavePrivacySettings(PrivacySettings). Serialize to JSON, key "privacy_settings". One file only.
On success: update FEATURES.md — feature 7.5 from 📋 to ✅.

### 7.6 Devices connected to account

⬜ TASK-104: Create Muffle.Data/Models/DeviceSession.cs — properties: SessionId (int), UserId (int), DeviceName (string), Platform (string), IpAddress (string?), LastActiveAt (DateTime), CreatedAt (DateTime). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-105: In Muffle.Data/Services/SqliteDbService.cs — add CREATE TABLE IF NOT EXISTS DeviceSessions DDL (SessionId INTEGER PRIMARY KEY AUTOINCREMENT, UserId INTEGER NOT NULL, DeviceName TEXT NOT NULL, Platform TEXT NOT NULL, IpAddress TEXT, LastActiveAt DATETIME NOT NULL, CreatedAt DATETIME NOT NULL, FK to Users). Also add DROP TABLE IF EXISTS DeviceSessions to DisposeDatabase. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-106: Create Muffle.Data/Services/DeviceSessionService.cs — static class with CreateSession(int userId, string deviceName, string platform, string? ipAddress) returning DeviceSession?, GetActiveSessions(int userId) returning List<DeviceSession>, RevokeSession(int sessionId) returning bool, RevokeAllSessions(int userId). Use Dapper. One file only.
On success: update FEATURES.md — feature 7.6 from 📋 to ✅.

### 7.7 Patch notes viewer

⬜ TASK-107: Create Muffle.Data/Models/PatchNote.cs — properties: Version (string), Title (string), Content (string), ReleaseDate (DateTime). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-108: Create Muffle.Data/Services/PatchNotesService.cs — static class with GetPatchNotes() returning a hardcoded List<PatchNote> containing at least one entry: Version="0.1.0", Title="Initial Release", Content="Foundation phase complete: chat, servers, friends, WebRTC voice/video.", ReleaseDate=new DateTime(2026,1,1). One file only.
On success: update FEATURES.md — feature 7.7 from 📋 to ✅.

### 7.8 Library acknowledgements

⬜ TASK-109: Create Muffle.Data/Services/AcknowledgementsService.cs — static class with GetAcknowledgements() returning List<(string Name, string License, string Url)>. Include: ("Dapper","Apache-2.0","https://github.com/DapperLib/Dapper"), ("BCrypt.Net-Next","MIT","https://github.com/BcryptNet/bcrypt.net"), ("WebRTCme","MIT","https://github.com/AlessandroMartinworx/WebRTCme"), ("Microsoft.Data.Sqlite","MIT","https://github.com/dotnet/efcore"), ("System.Text.Json","MIT","https://github.com/dotnet/runtime"). One file only.
On success: update FEATURES.md — feature 7.8 from 📋 to ✅.

### 7.x Settings UI

⬜ TASK-110: Create Muffle/ViewModels/SettingsViewModel.cs — properties for each settings object (VoiceSettings, VideoSettings, AccessibilitySettings, PrivacySettings, DeveloperSettings), LoadCommand calling SettingsService getters, SaveCommand calling SettingsService setters. Implement BindableObject. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-111: Create Muffle/Views/SettingsView.xaml — MAUI ContentPage with a vertical StackLayout of expandable sections: Voice (sliders for volume, switch for push-to-talk), Video (picker for resolution), Accessibility (switch for high contrast, slider for font scale), Privacy (switches for DM/friend requests, picker for filter level), Developer (switches). Bind to SettingsViewModel. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-112: Create Muffle/Views/SettingsView.xaml.cs — code-behind setting BindingContext to new SettingsViewModel(). One file only.
On success: update FEATURES.md summary — Phase 7 from 0/8 to 8/8, status ✅ Complete.

---

## Phase 8 — Security & Account

### 8.1 Optional 2FA/MFA

⬜ TASK-113: Create Muffle.Data/Models/TwoFactorAuth.cs — properties: UserId (int), SecretKey (string), IsEnabled (bool), EnabledAt (DateTime?), BackupCodes (string?, JSON array of recovery codes). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-114: In Muffle.Data/Services/SqliteDbService.cs — add CREATE TABLE IF NOT EXISTS TwoFactorAuth DDL (UserId INTEGER PRIMARY KEY, SecretKey TEXT NOT NULL, IsEnabled INTEGER NOT NULL DEFAULT 0, EnabledAt DATETIME, BackupCodes TEXT, FK to Users). Also add DROP TABLE IF EXISTS TwoFactorAuth to DisposeDatabase. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-115: Create Muffle.Data/Services/TwoFactorAuthService.cs — static class with GenerateSecret() returning a random 20-char Base32 string, EnableTwoFactor(int userId, string secret) inserting into TwoFactorAuth, VerifyCode(int userId, string code) implementing TOTP (time-step=30s, digits=6, HMAC-SHA1 with secret), IsEnabled(int userId) returning bool. Use Dapper. One file only.
On success: update FEATURES.md — feature 8.1 from 📋 to ✅.

### 8.2 Password change flow

⬜ TASK-116: In Muffle.Data/Services/AuthenticationService.cs — add static method ChangePassword(int userId, string currentPassword, string newPassword) returning bool. Get user by ID, verify currentPassword with BCrypt.Verify, hash newPassword with BCrypt.HashPassword, UPDATE Users SET PasswordHash = @hash WHERE UserId = @userId. One file only.
On success: update FEATURES.md — feature 8.2 from 📋 to ✅.

### 8.3 Email verification

⬜ TASK-117: In Muffle.Data/Models/User.cs — add `public bool IsEmailVerified { get; set; }` and `public string? EmailVerificationCode { get; set; }` properties. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-118: In Muffle.Data/Services/SqliteDbService.cs — add `IsEmailVerified INTEGER NOT NULL DEFAULT 0` and `EmailVerificationCode TEXT` columns to the Users CREATE TABLE statement. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-119: Create Muffle.Data/Services/EmailVerificationService.cs — static class with GenerateVerificationCode(int userId) returning string (6-digit random, stored in Users.EmailVerificationCode), VerifyEmail(int userId, string code) returning bool (compare code, set IsEmailVerified=1), IsEmailVerified(int userId) returning bool. Use Dapper. One file only.
On success: update FEATURES.md — feature 8.3 from 📋 to ✅.

### 8.4 Password reset (forgot password)

⬜ TASK-120: In Muffle.Data/Models/User.cs — add `public string? PasswordResetToken { get; set; }` and `public DateTime? PasswordResetExpiry { get; set; }` properties. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-121: In Muffle.Data/Services/SqliteDbService.cs — add `PasswordResetToken TEXT` and `PasswordResetExpiry DATETIME` columns to the Users CREATE TABLE statement. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-122: Create Muffle.Data/Services/PasswordResetService.cs — static class with GenerateResetToken(string email) returning string? (Guid token, set expiry=1 hour, UPDATE Users SET PasswordResetToken, PasswordResetExpiry), ValidateResetToken(string token) returning bool (check expiry), ResetPassword(string token, string newPassword) returning bool (validate token, hash password, update PasswordHash, clear token). Use Dapper. One file only.
On success: update FEATURES.md — feature 8.4 from 📋 to ✅.

### 8.5 Session management

⬜ TASK-123: In Muffle.Data/Services/DeviceSessionService.cs (created in TASK-106) — verify GetActiveSessions and RevokeSession methods exist. If TASK-106 was completed, just run build. If not, add these methods now. One file only.
On success: update FEATURES.md — feature 8.5 from 📋 to ✅. Also update FEATURES.md summary — Phase 8 from 0/5 to 5/5, status ✅ Complete.

---

## Phase 9 — Additional Features

### 9.1 Friend groups

⬜ TASK-124: Create Muffle.Data/Models/FriendGroup.cs — properties: GroupId (int), OwnerId (int), Name (string), CreatedAt (DateTime). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-125: Create Muffle.Data/Models/FriendGroupMember.cs — properties: GroupId (int), FriendUserId (int), AddedAt (DateTime). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-126: In Muffle.Data/Services/SqliteDbService.cs — add CREATE TABLE IF NOT EXISTS FriendGroups DDL (GroupId INTEGER PRIMARY KEY AUTOINCREMENT, OwnerId INTEGER NOT NULL, Name TEXT NOT NULL, CreatedAt DATETIME NOT NULL, FK to Users) and CREATE TABLE IF NOT EXISTS FriendGroupMembers (GroupId INTEGER NOT NULL, FriendUserId INTEGER NOT NULL, AddedAt DATETIME NOT NULL, PRIMARY KEY(GroupId, FriendUserId), FK to FriendGroups and Users). Also add both DROP TABLEs to DisposeDatabase. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-127: Create Muffle.Data/Services/FriendGroupService.cs — static class with CreateGroup(int ownerId, string name) returning FriendGroup?, AddMember(int groupId, int friendUserId), RemoveMember(int groupId, int friendUserId), GetGroups(int ownerId) returning List<FriendGroup>, GetGroupMembers(int groupId) returning List<FriendGroupMember>. Use Dapper. One file only.
On success: update FEATURES.md — feature 9.1 from 📋 to ✅.

### 9.2 Subscription model (premium)

⬜ TASK-128: Create Muffle.Data/Models/Subscription.cs — public enum SubscriptionTier { Free = 0, Premium = 1 }. Class properties: SubscriptionId (int), UserId (int), Tier (SubscriptionTier), StartDate (DateTime), EndDate (DateTime?), IsActive (bool). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-129: In Muffle.Data/Services/SqliteDbService.cs — add CREATE TABLE IF NOT EXISTS Subscriptions DDL (SubscriptionId INTEGER PRIMARY KEY AUTOINCREMENT, UserId INTEGER NOT NULL, Tier INTEGER NOT NULL DEFAULT 0, StartDate DATETIME NOT NULL, EndDate DATETIME, IsActive INTEGER NOT NULL DEFAULT 1, FK to Users). Also add DROP TABLE IF EXISTS Subscriptions to DisposeDatabase. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-130: Create Muffle.Data/Services/SubscriptionService.cs — static class with CreateSubscription(int userId, SubscriptionTier tier) returning Subscription?, GetSubscription(int userId) returning Subscription?, IsUserPremium(int userId) returning bool, CancelSubscription(int subscriptionId) returning bool. Use Dapper. One file only.
On success: update FEATURES.md — feature 9.2 from 📋 to ✅.

### 9.3 Subscription gifting

⬜ TASK-131: Create Muffle.Data/Models/SubscriptionGift.cs — properties: GiftId (int), GiverUserId (int), ReceiverUserId (int), Tier (SubscriptionTier), GiftedAt (DateTime), RedeemedAt (DateTime?), IsRedeemed (bool). One file only.
On success: no FEATURES.md change needed.

⬜ TASK-132: In Muffle.Data/Services/SubscriptionService.cs — add GiftSubscription(int giverUserId, int receiverUserId, SubscriptionTier tier) returning SubscriptionGift? and RedeemGift(int giftId, int userId) returning bool (creates subscription for receiver). One file only.
On success: update FEATURES.md — feature 9.3 from 📋 to ✅.

### 9.4 Mobile-specific views

⬜ TASK-133: Create Muffle/Views/MobileMainView.xaml — MAUI ContentPage with a FlyoutPage-style layout: FlyoutItem for Servers list, FlyoutItem for Friends list. Use OnIdiom to detect Phone idiom. Display single-panel content area. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-134: Create Muffle/Views/MobileMainView.xaml.cs — code-behind. One file only.
On success: update FEATURES.md — feature 9.4 from 📋 to ✅.

### 9.5 Tablet-optimized layouts

⬜ TASK-135: Create Muffle/Views/TabletMainView.xaml — MAUI ContentPage with two-column Grid layout (servers/friends list on left 300px, content area on right *). Use OnIdiom for Tablet. One file only.
On success: no FEATURES.md change needed.

⬜ TASK-136: Create Muffle/Views/TabletMainView.xaml.cs — code-behind. One file only.
On success: update FEATURES.md — feature 9.5 from 📋 to ✅.

### 9.6 Screenshare (desktop)

⬜ TASK-137: Create Muffle.Data/Services/ScreenShareService.cs — static class with StartScreenShareAsync() returning Task and StopScreenShareAsync() returning Task. Both are stubs with TODO comments: "// TODO: Windows — use GraphicsCaptureSession" and "// TODO: macOS — use ScreenCaptureKit". Add IsSharing bool property. One file only.
On success: update FEATURES.md — feature 9.6 from 📋 to 🔧.

### 9.7 Picture-in-picture mode (mobile)

⬜ TASK-138: Create Muffle.Data/Services/PictureInPictureService.cs — static class with EnterPipMode() and ExitPipMode() stubs. TODO comments: "// TODO: Android — use PictureInPictureParams.Builder" and "// TODO: iOS — use AVPictureInPictureController". Add IsInPipMode bool property. One file only.
On success: update FEATURES.md — feature 9.7 from 📋 to 🔧. Also update FEATURES.md summary — Phase 9 from 0/7 to 7/7.

---

## Summary

| Range | Phase | Tasks |
|-------|-------|-------|
| TASK-001 – TASK-009 | Phase 3: User Management (verify) | 9 |
| TASK-010 – TASK-018 | Phase 2: Voice & Video (complete) | 9 |
| TASK-019 – TASK-049 | Phase 4: Server Features | 31 |
| TASK-050 – TASK-070 | Phase 5: Chat Enhancements | 21 |
| TASK-071 – TASK-093 | Phase 6: User Profile & Customization | 23 |
| TASK-094 – TASK-112 | Phase 7: Settings & Configuration | 19 |
| TASK-113 – TASK-123 | Phase 8: Security & Account | 11 |
| TASK-124 – TASK-138 | Phase 9: Additional Features | 15 |
| **Total** | | **138** |
