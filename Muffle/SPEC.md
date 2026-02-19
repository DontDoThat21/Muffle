# Muffle — Product Specification

## 1. Product Overview

Muffle is a cross-platform communication application inspired by Discord. It provides server-based group communication, a friends/direct-messaging system, and real-time chat powered by WebSockets. The application is built with .NET MAUI, targeting mobile and desktop platforms from a single codebase, and uses an MVVM architecture with Dapper for data access.

## 2. Target Platforms

| Platform       | TFM                           | Minimum OS Version |
|----------------|-------------------------------|--------------------|
| iOS            | `net10.0-ios`                 | 11.0               |
| Android        | `net10.0-android`             | API 21 (5.0)       |
| macOS          | `net10.0-maccatalyst`         | 13.1               |
| Windows        | `net10.0-windows10.0.19041.0` | Windows 10 1809    |

> Tizen support is defined but commented out in the project file.

## 3. Package Dependencies

### Muffle (MAUI application)

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.Maui.Controls | 10.0.40 | .NET MAUI UI framework |
| Microsoft.Maui.Controls.Compatibility | 10.0.40 | Backward-compatible MAUI renderers |
| Microsoft.Extensions.Logging.Debug | 10.0.3 | Debug-output logging provider |
| SQLitePCLRaw.bundle_e_sqlite3 | 2.1.8 | Native SQLite runtime bundle |
| WebRTCme | 2.0.0 | WebRTC abstraction for voice/video calls |

### Muffle.Data (class library — `net10.0`)

| Package | Version | Purpose |
|---------|---------|---------|
| Dapper | 2.1.35 | Micro-ORM for SQL queries |
| Microsoft.Data.SqlClient | 6.1.1 | SQL Server ADO.NET provider |
| Microsoft.Data.Sqlite.Core | 10.0.3 | SQLite ADO.NET provider |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.3 | EF Core SQL Server provider (available for future use) |
| Microsoft.Extensions.DependencyInjection | 10.0.3 | DI container |
| System.Text.Json | 9.0.8 | JSON serialization/deserialization |
| WebRTCme | 2.0.0 | WebRTC abstraction (shared with main project) |

## 4. Architecture Overview

### 4.1 Solution Structure

```
Muffle.sln
├── Muffle/                          # .NET MAUI application
│   ├── Platforms/                   # Platform-specific code (iOS, Android, macOS, Windows)
│   ├── Views/                       # XAML ContentPages and ContentViews
│   │   ├── MuffleMain.xaml(.cs)             # Root page — three-panel Discord-style layout
│   │   ├── FriendDetailsContentView.xaml(.cs)   # Chat view for a selected friend
│   │   ├── ServerDetailsContentView.xaml(.cs)   # Detail view for a selected server
│   │   ├── FriendDetailTopBarUIView.xaml(.cs)   # Top bar when viewing a friend (voice/video call buttons)
│   │   ├── FriendsUIComponents/
│   │   │   └── FriendsTopBarUIView.xaml(.cs)    # Top bar for the friends list (add-friend button)
│   │   └── ServersUIComponents/
│   │       └── ServerTopBarUIView.xaml(.cs)      # Top bar for a server view
│   ├── ViewModels/
│   │   ├── MainPageViewModel.cs             # Servers, friends, user state
│   │   └── FriendDetailsContentViewModel.cs # Chat messages, signaling, image sharing, call initiation
│   ├── Converters/
│   │   └── MessageConverters.cs             # Value converters for message type visibility & base64-to-image
│   ├── Services/
│   │   └── ImagePickerService.cs            # Image picking + byte-array/base64 conversion
│   ├── Resources/                   # Images, fonts, styles, raw assets
│   ├── MauiProgram.cs               # App entry point — DB init, logging
│   ├── App.xaml(.cs)                # Application lifecycle
│   ├── AppShell.xaml(.cs)           # Shell navigation
│   └── appsettings.json             # Connection strings
│
└── Muffle.Data/                     # Shared data-access library
    ├── Models/
    │   ├── User.cs                  # User entity (Id, Name, Email, Password)
    │   ├── Server.cs                # Server entity (Id, Name, Description, IpAddress, Port)
    │   ├── Friend.cs                # Friend entity (UserId, Id, Name, Description, Memo, Image, dates)
    │   ├── ChatMessage.cs           # Chat message (Content, Sender, Timestamp, Type, ImagePath, ImageData)
    │   └── MessageWrapper.cs        # Wire-format DTO for WebSocket messages
    ├── Services/
    │   ├── SqliteDbService.cs       # SQLite schema creation, seeding, teardown
    │   ├── SqlServerDbService.cs    # SQL Server schema creation, seeding, teardown
    │   ├── UsersService.cs          # Business logic — query users, servers, friends; create servers
    │   ├── ConfigurationLoader.cs   # Reads appsettings.json for connection strings
    │   ├── ISignalingService.cs     # Signaling abstraction (connect, send, receive, events)
    │   ├── SignalingService.cs       # WebSocket-based signaling implementation
    │   ├── WebSocketService.cs      # Low-level WebSocket wrapper
    │   └── WebRTCManager.cs         # WebRTC peer-connection manager (commented out / WIP)
    └── Tests/
        └── ImageMessageDemo.cs      # Demo/test for image message serialization round-trip
```

### 4.2 Design Patterns

| Pattern | Usage |
|---------|-------|
| **MVVM** | Views bind to ViewModels; ViewModels expose `ObservableCollection`s, `ICommand`s, and `BindableObject` properties. |
| **Service Layer** | `UsersService`, `SignalingService`, `ImagePickerService` encapsulate business and I/O logic. |
| **Dual Database Strategy** | SQLite for local/development, SQL Server for production. Both share the same schema shape and seed data. In Debug builds the database is dropped and recreated on every launch. |
| **WebSocket Signaling** | `ISignalingService` / `SignalingService` abstracts real-time messaging over WebSockets with JSON-serialized `MessageWrapper` payloads. |

### 4.3 Database Strategy

- **Development (Debug)**: SQLite file (`muffle_localdatabase.db3`). The database is disposed and re-initialized on every app launch.
- **Production (Release)**: SQL Server via connection string in `appsettings.json`. Initialized once; tables are created with `IF NOT EXISTS` guards.
- **ORM**: Dapper — all queries are raw SQL executed through `IDbConnection`.

## 5. Data Model

### 5.1 Users

| Column | Type | Notes |
|--------|------|-------|
| UserId | INTEGER (PK, auto-increment) | Primary key |
| Name | TEXT | Required |
| Description | TEXT | Optional |
| CreationDate | DATETIME | Required |

### 5.2 Servers

| Column | Type | Notes |
|--------|------|-------|
| Id | INTEGER (PK, auto-increment) | Primary key |
| Name | TEXT | Required |
| Description | TEXT | Optional |
| IpAddress | TEXT | Required |
| Port | INTEGER | Required |

### 5.3 ServerOwners (join table)

| Column | Type | Notes |
|--------|------|-------|
| ServerId | INTEGER (PK, FK → Servers) | Composite primary key |
| UserId | INTEGER (PK, FK → Users) | Composite primary key |

### 5.4 Friends

| Column | Type | Notes |
|--------|------|-------|
| Id | INTEGER (PK, auto-increment) | Primary key |
| UserId | INTEGER | Owner of the friendship |
| Name | TEXT | Required |
| Description | TEXT | Optional |
| Memo | TEXT | Optional |
| Image | TEXT | Avatar filename |
| FriendshipDate | DATETIME | When the friendship was established |
| CreationDate | DATETIME | Record creation time |

### 5.5 ChatMessage (in-memory model)

| Property | Type | Notes |
|----------|------|-------|
| Content | string | Message text or image caption |
| Sender | User | Message author |
| Timestamp | DateTime | Send time |
| Type | MessageType | `Text` or `Image` |
| ImagePath | string? | Local file path (images only) |
| ImageData | string? | Base64-encoded image payload (images only) |

### 5.6 MessageWrapper (wire-format DTO)

| Property | Type | Notes |
|----------|------|-------|
| Type | MessageType | `Text` or `Image` |
| Content | string | Display text |
| ImageData | string? | Base64 payload |
| Timestamp | DateTime | Send time |
| SenderName | string | Display name |
| SenderId | int | User ID |

## 6. Features Specification

### 6.1 Three-Panel Discord-Style Layout

| ID | Requirement |
|----|-------------|
| UI-1 | The main page displays a three-column layout: **Servers** (left rail), **Friends / Direct Messages** (center-left panel), and **Main Content** (center-right area). |
| UI-2 | A top bar spans the content area and dynamically switches between friend detail, server detail, and friends-list views based on the current selection context. |
| UI-3 | The Muffle logo is displayed at the top of the Servers column. |
| UI-4 | A search `Entry` ("Find or start a discussion") is displayed at the top of the Friends panel. |

### 6.2 Server Management

| ID | Requirement |
|----|-------------|
| SRV-1 | The left rail lists all servers the current user belongs to, loaded from the database at startup. |
| SRV-2 | Clicking a server button selects it, updates the main content area with `ServerDetailsContentView`, and clears the top bar. |
| SRV-3 | A **"+"** button in the Servers column header opens a prompt flow (server name → optional description) to create a new server. |
| SRV-4 | Created servers are persisted to the `Servers` and `ServerOwners` tables and the server list is refreshed in the UI. |
| SRV-5 | Server creation defaults to IP `127.0.0.1` and port `8080`. |
| SRV-6 | Success/failure alerts are displayed after server creation. |

### 6.3 Friends & Direct Messages

| ID | Requirement |
|----|-------------|
| FRI-1 | A **"Friends"** button in the center-left panel switches the view to the friends list context and displays `FriendsTopBarUIView` in the top bar. |
| FRI-2 | Below the Friends button, a **"Direct Messages"** header with a **"+"** button is shown. |
| FRI-3 | Friends are listed as `CollectionView` items with avatar `ImageButton` and name `Button`. |
| FRI-4 | Clicking a friend selects them, switches the main content to `FriendDetailsContentView`, and displays `FriendDetailTopBarUIView` in the top bar. |
| FRI-5 | The Add Friend button raises an event; the main content frame currently displays a placeholder ("Friend Added"). |

### 6.4 Real-Time Chat (WebSocket Messaging)

| ID | Requirement |
|----|-------------|
| CHAT-1 | When a friend is selected, `FriendDetailsContentViewModel` establishes a WebSocket connection to `ws://localhost:8080`. |
| CHAT-2 | Outgoing text messages are serialized as `MessageWrapper` JSON and sent via `SignalingService.SendMessageWrapperAsync`. |
| CHAT-3 | Incoming messages are received in a background loop, deserialized from JSON, and added to `ChatMessages` (`ObservableCollection<ChatMessage>`). |
| CHAT-4 | Legacy plain-text messages (non-JSON) are handled gracefully and displayed as text. |
| CHAT-5 | The chat `Entry` supports submitting a message via the **Completed** event (keyboard Enter). |
| CHAT-6 | After sending, the input field is cleared. |

### 6.5 Image Messaging

| ID | Requirement |
|----|-------------|
| IMG-1 | A **Send Image** command is available in the friend chat view. |
| IMG-2 | `ImagePickerService` provides image selection (placeholder implementation using `FilePicker` pattern). |
| IMG-3 | Selected images are converted to a byte array and then Base64-encoded for transmission. |
| IMG-4 | Image messages use `MessageType.Image` and include `ImageData` (Base64) in the `MessageWrapper`. |
| IMG-5 | `Base64ToImageConverter` decodes Base64 data to `ImageSource` for display; placeholder data is detected and hidden. |
| IMG-6 | `MessageTypeToTextVisibilityConverter` and `MessageTypeToImageVisibilityConverter` control which template elements are visible based on `MessageType`. |
| IMG-7 | `StringNotEmptyConverter` validates that image data is non-empty and non-placeholder before displaying. |

### 6.6 Voice & Video Calls (WebRTC — Work in Progress)

| ID | Requirement |
|----|-------------|
| RTC-1 | `FriendDetailTopBarUIView` exposes **Voice Call** and **Video Call** buttons that raise events consumed by the main page. |
| RTC-2 | `FriendDetailsContentViewModel.StartVoiceCallAsync` and `StartVideoCallAsync` are invoked on button click (currently placeholder implementations). |
| RTC-3 | `WebRTCManager` (commented out) defines the intended architecture: STUN server config, ICE candidate exchange, offer/answer SDP negotiation, and media track management via `WebRTCme`. |
| RTC-4 | The signaling channel (`ISignalingService`) is designed to carry SDP offers, answers, and ICE candidates for WebRTC negotiation. |

### 6.7 Database Initialization & Seeding

| ID | Requirement |
|----|-------------|
| DB-1 | In **Debug** mode, both SQL Server and SQLite databases are dropped and re-created on every app launch via `DisposeDatabase()` → `InitializeDatabase()`. |
| DB-2 | In **Release** mode, only SQLite is initialized (tables created if not existing). |
| DB-3 | Seed data includes three users (Alice, Bob, Charlie), three servers, three server-ownership records, and four friends with avatar images. |
| DB-4 | `ConfigurationLoader` reads connection strings from `appsettings.json` by walking the directory tree from the entry assembly location. |

### 6.8 Data Access

| ID | Requirement |
|----|-------------|
| DAT-1 | `UsersService.GetUsersServers()` returns all rows from the `Servers` table via Dapper. |
| DAT-2 | `UsersService.GetUsersFriends()` returns all rows from the `Friends` table via Dapper. |
| DAT-3 | `UsersService.CreateServer()` inserts a new server, creates a `ServerOwners` association, and returns the created `Server`. |
| DAT-4 | `UsersService.GetUser()` returns a new empty `User` (placeholder). |

### 6.9 Configuration

| ID | Requirement |
|----|-------------|
| CFG-1 | Connection strings are stored in `Muffle/appsettings.json` under `ConnectionStrings.SqlServerConnection` and `ConnectionStrings.SqliteConnection`. |
| CFG-2 | SQLite connection string: `Data Source=muffle_localdatabase.db3`. |
| CFG-3 | SQL Server connection string: `Server=(localdb)\MSSQLLocalDB;Database=Muffle;Trusted_Connection=True;TrustServerCertificate=True;`. |

## 7. Seed Data Reference

### Users
| UserId | Name | Description |
|--------|------|-------------|
| 1 | Alice | First user |
| 2 | Bob | Second user |
| 3 | Charlie | Third user |

### Servers
| Id | Name | IpAddress | Port |
|----|------|-----------|------|
| 0 | Dynamic database test | 192.168.1.1 | 8080 |
| 1 | Example 2 | 192.168.1.2 | 8081 |
| 2 | Tyler | 192.168.1.3 | 8082 |

### Friends
| Id | Name | Image | Description |
|----|------|-------|-------------|
| 0 | Gabe | gabe.png | Starcraft 2 Bro test |
| 1 | Tylor | tom.jpg | Best Programmer NA C# |
| 2 | Nick | nick.png | Army Motorcycling Bro test |
| 3 | Tyler | murky.png | Best 1DGer in da land test |
