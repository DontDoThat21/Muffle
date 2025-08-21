# Muffle - Cross-Platform Communication App

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

Muffle is a .NET MAUI cross-platform communication application similar to Discord, featuring server management, friends/direct messaging, and WebRTC-based voice communication. It targets iOS, Android, macOS, and Windows platforms.

## Working Effectively

### Prerequisites and Setup
- Install .NET 8.0 SDK (minimum version 8.0.100)
- For full platform builds: Install MAUI workloads with `dotnet workload install maui-android maui-ios maui-maccatalyst maui-windows`
- **LIMITATION**: MAUI workloads are not available in all environments (particularly Linux CI/build servers)
- SQL Server (optional, for production database) or SQLite (included, for local development)

### Core Build Commands
- Build data layer only: `dotnet build Muffle.Data/Muffle.Data.csproj` -- takes 30 seconds, always succeeds
- Build full solution (requires MAUI workloads): `dotnet build` -- takes 2-5 minutes. NEVER CANCEL. Set timeout to 10+ minutes.
- Restore packages: `dotnet restore` -- takes 20-60 seconds
- Clean build: `dotnet clean && dotnet build` -- takes 3-6 minutes. NEVER CANCEL.

### **CRITICAL BUILD LIMITATIONS**
- **CANNOT BUILD ON LINUX WITHOUT MAUI WORKLOADS**: The main Muffle project requires MAUI workloads that are not available in standard Linux .NET SDK installations
- **Muffle.Data project CAN be built independently** on any platform with .NET 8.0
- For CI/CD scenarios: Use Windows or macOS build agents with MAUI workloads installed
- **Build failure error**: `NETSDK1147: To build this project, the following workloads must be installed` indicates missing MAUI workloads

### Database Setup
- **Local Development**: Uses SQLite database (`muffle_localdatabase.db3`)
  - Database is automatically created and seeded on first application startup
  - No manual setup required for SQLite
- **Production**: Configured for SQL Server with connection string in `appsettings.json`
  - Connection string: `Server=TY\\SQLEXPRESS;Database=Muffle;Trusted_Connection=True;TrustServerCertificate=True;`
  - Update connection string for your SQL Server instance

### Running the Application
- **Cannot run directly from command line** - MAUI apps require platform-specific launchers
- Use Visual Studio or Visual Studio for Mac with MAUI workload installed
- For debugging: Use platform-specific debuggers (iOS Simulator, Android emulator, Windows app)
- Database initialization occurs automatically on app startup

## Project Structure

### Solution Layout
```
Muffle.sln                    # Solution file (2 projects)
├── Muffle/                   # Main MAUI application
│   ├── Platforms/           # Platform-specific code (iOS, Android, macOS, Windows)
│   ├── Views/               # XAML UI views and code-behind
│   ├── ViewModels/          # MVVM ViewModels
│   ├── Resources/           # Images, fonts, styles
│   └── MauiProgram.cs       # App entry point and configuration
└── Muffle.Data/             # Data access library
    ├── Models/              # Data models (User, Server, Friend, ChatMessage)
    └── Services/            # Database and business logic services
```

### Key Files
- `Muffle/Views/MuffleMain.xaml` - Main application UI (Discord-like layout)
- `Muffle.Data/Services/SqliteDbService.cs` - SQLite database initialization and access
- `Muffle.Data/Services/SqlServerDbService.cs` - SQL Server database access
- `Muffle/appsettings.json` - Database connection strings
- `Muffle/MauiProgram.cs` - App configuration and dependency injection

## Validation

### Essential Validation Steps
- **ALWAYS** build `Muffle.Data` project first to validate core data layer
- For full builds: Ensure MAUI workloads are installed before attempting to build main project
- **Database Validation**: App automatically creates and seeds database on startup - verify no SQL errors in debug output
- **UI Validation**: Check main layout renders with servers list, friends list, and main content area

### Manual Testing Scenarios
- Launch app and verify main UI loads with three-panel layout (servers, friends, content)
- Click on server buttons in left panel - verify content area updates
- Click on friend entries - verify content switches to friend view
- Check database initialization: Look for seeded users (Alice, Bob, Charlie) and servers

### Common Validation Commands
- Check build status: `dotnet build Muffle.Data --verbosity minimal`
- Verify dependencies: `dotnet list package` 
- Check for restore issues: `dotnet restore --verbosity normal`

## Common Issues and Workarounds

### Build Failures
- **"NETSDK1147 workloads must be installed"**: Install MAUI workloads or use Windows/macOS build environment
- **Missing packages**: Run `dotnet restore` first
- **Database connection errors**: Verify connection strings in `appsettings.json`

### Development Tips
- Use SQLite for local development (no SQL Server setup required)
- Database is recreated on each debug build - changes will be lost
- Check `#if DEBUG` blocks in `MauiProgram.cs` for debug-specific behavior
- WebRTC features require real device testing (not simulators)

## Architecture Notes

### MVVM Pattern
- Views (`Views/`) contain XAML UI definitions
- ViewModels (`ViewModels/`) handle UI logic and data binding
- Models (`Muffle.Data/Models/`) define data structures

### Database Strategy
- **Dual database support**: SQL Server for production, SQLite for development
- **Auto-initialization**: Both databases are created and seeded automatically
- **Dapper ORM**: Used for database access throughout the application

### Key Services
- `UsersService` - User management and friends/servers retrieval
- `SqliteDbService` / `SqlServerDbService` - Database initialization and connection management
- `WebRTCManager` - Voice communication functionality
- `ConfigurationLoader` - Configuration and connection string management

## Time Expectations

- **Initial restore**: 30-60 seconds
- **Data project build**: 20-30 seconds  
- **Full solution build**: 2-5 minutes (NEVER CANCEL - requires platform toolchains)
- **Clean build**: 3-6 minutes (NEVER CANCEL)
- **App startup**: 5-10 seconds (includes database initialization)

## Development Workflow

1. **Start with data layer**: Build and test `Muffle.Data` project first
2. **Verify database**: Check connection strings in `appsettings.json`
3. **Full build**: Ensure MAUI workloads installed, then build complete solution
4. **Platform testing**: Use appropriate emulators/simulators for target platforms
5. **Database changes**: Modify initialization code in `SqliteDbService.cs` or `SqlServerDbService.cs`

**REMEMBER**: Always work with the data project first to validate core functionality before attempting full MAUI builds.