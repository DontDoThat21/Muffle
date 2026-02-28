using Muffle.Data.Models;

namespace Muffle.Data.Services
{
    /// <summary>
    /// Returns the full list of open-source libraries used by Muffle, with license and attribution details.
    /// </summary>
    public static class LibraryAcknowledgementsService
    {
        public static List<LibraryAcknowledgement> GetAcknowledgements()
        {
            return new List<LibraryAcknowledgement>
            {
                new LibraryAcknowledgement
                {
                    Name = "Microsoft.Maui.Controls",
                    Version = "10.0.40",
                    License = "MIT",
                    Description = "Cross-platform UI framework for building native mobile and desktop apps with C# and XAML.",
                    Url = "https://github.com/dotnet/maui"
                },
                new LibraryAcknowledgement
                {
                    Name = "Microsoft.Maui.Controls.Compatibility",
                    Version = "10.0.40",
                    License = "MIT",
                    Description = "Compatibility layer for .NET MAUI, providing legacy renderer support during migration.",
                    Url = "https://github.com/dotnet/maui"
                },
                new LibraryAcknowledgement
                {
                    Name = "Microsoft.Extensions.Logging.Debug",
                    Version = "10.0.3",
                    License = "MIT",
                    Description = "Debug output logger provider for Microsoft.Extensions.Logging.",
                    Url = "https://github.com/dotnet/runtime"
                },
                new LibraryAcknowledgement
                {
                    Name = "Microsoft.Extensions.DependencyInjection",
                    Version = "10.0.3",
                    License = "MIT",
                    Description = "Default dependency injection container for .NET applications.",
                    Url = "https://github.com/dotnet/runtime"
                },
                new LibraryAcknowledgement
                {
                    Name = "SQLitePCLRaw.bundle_e_sqlite3",
                    Version = "2.1.8",
                    License = "Apache-2.0",
                    Description = "Native SQLite bindings for .NET — bundles the e_sqlite3 native library for all platforms.",
                    Url = "https://github.com/ericsink/SQLitePCL.raw"
                },
                new LibraryAcknowledgement
                {
                    Name = "Microsoft.Data.Sqlite.Core",
                    Version = "10.0.3",
                    License = "MIT",
                    Description = "Lightweight ADO.NET provider for SQLite, used for local development database access.",
                    Url = "https://github.com/dotnet/efcore"
                },
                new LibraryAcknowledgement
                {
                    Name = "Microsoft.Data.SqlClient",
                    Version = "6.1.1",
                    License = "MIT",
                    Description = "Microsoft SQL Server data provider for .NET, used for production database access.",
                    Url = "https://github.com/dotnet/SqlClient"
                },
                new LibraryAcknowledgement
                {
                    Name = "Microsoft.EntityFrameworkCore.SqlServer",
                    Version = "10.0.3",
                    License = "MIT",
                    Description = "Entity Framework Core provider for SQL Server — used for schema migrations and initialization.",
                    Url = "https://github.com/dotnet/efcore"
                },
                new LibraryAcknowledgement
                {
                    Name = "Dapper",
                    Version = "2.1.35",
                    License = "Apache-2.0",
                    Description = "Lightweight micro-ORM for .NET. Extends IDbConnection with convenient query and execute methods.",
                    Url = "https://github.com/DapperLib/Dapper"
                },
                new LibraryAcknowledgement
                {
                    Name = "BCrypt.Net-Next",
                    Version = "4.0.3",
                    License = "MIT",
                    Description = "BCrypt password hashing library for .NET, used for secure user credential storage.",
                    Url = "https://github.com/BcryptNet/bcrypt.net"
                },
                new LibraryAcknowledgement
                {
                    Name = "System.Text.Json",
                    Version = "9.0.8",
                    License = "MIT",
                    Description = "High-performance JSON serialization and deserialization library for .NET.",
                    Url = "https://github.com/dotnet/runtime"
                },
                new LibraryAcknowledgement
                {
                    Name = "WebRTCme",
                    Version = "2.0.0",
                    License = "MIT",
                    Description = "WebRTC implementation for .NET MAUI and Blazor — powers Muffle's voice and video calls.",
                    Url = "https://github.com/melihercan/WebRTCme"
                },
            };
        }
    }
}
