using Dapper;
using Microsoft.Data.SqlClient;
using Muffle.Data.Models;
using System.Collections.ObjectModel;
using System.Data;

namespace Muffle.Data.Services
{
    public class SqlServerDbService
    {
        private static readonly string _connectionString = ConfigurationLoader.GetConnectionString("SqlServerConnection");

        public SqlServerDbService()
        {

        }

        public static IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public static void InitializeDatabase()
        {
            using var connection = CreateConnection();
            connection.Open();

            // Create Users table
            var createUsersTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' and xtype='U')
        CREATE TABLE Users (
            UserId INT PRIMARY KEY IDENTITY(1,1),
            Name NVARCHAR(100) NOT NULL,
            Email NVARCHAR(255) UNIQUE NOT NULL,
            PasswordHash NVARCHAR(255) NOT NULL,
            Description NVARCHAR(MAX),
            CreationDate DATETIME NOT NULL,
            Discriminator INT NOT NULL DEFAULT 0,
            IsActive BIT NOT NULL DEFAULT 1,
            DisabledAt DATETIME,
            AvatarUrl NVARCHAR(500),
            BannerUrl NVARCHAR(500),
            AboutMe NVARCHAR(MAX),
            Pronouns NVARCHAR(50),
            Status INT NOT NULL DEFAULT 0,
            CustomStatusText NVARCHAR(255),
            CustomStatusEmoji NVARCHAR(50),
            ShowOnlineStatus BIT NOT NULL DEFAULT 1,
            IsEmailVerified BIT NOT NULL DEFAULT 1
        );";

            connection.Execute(createUsersTableQuery);

            // Create index for username + discriminator lookups
            var createUsernameDiscriminatorIndexQuery = @"
        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='idx_users_name_discriminator' AND object_id = OBJECT_ID('Users'))
        CREATE INDEX idx_users_name_discriminator ON Users(Name, Discriminator);";

            connection.Execute(createUsernameDiscriminatorIndexQuery);

            // Create Servers table
            var createServersTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Servers' and xtype='U')
        CREATE TABLE Servers (
            ServerId UNIQUEIDENTIFIER PRIMARY KEY,
            Name NVARCHAR(100) NOT NULL,
            Description NVARCHAR(MAX),
            Image VARBINARY(MAX),
            CreationDate DATETIME NOT NULL,
            AddDate DATETIME NOT NULL
        );";

            connection.Execute(createServersTableQuery);

            // Create Channels table
            var createChannelsTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Channels' and xtype='U')
        CREATE TABLE Channels (
            ChannelId INT PRIMARY KEY IDENTITY(1,1),
            ServerId UNIQUEIDENTIFIER NOT NULL,
            Name NVARCHAR(100) NOT NULL,
            Description NVARCHAR(MAX),
            Type INT NOT NULL DEFAULT 0,
            Position INT NOT NULL DEFAULT 0,
            CreatedAt DATETIME NOT NULL,
            CreatedBy INT NOT NULL,
            FOREIGN KEY (ServerId) REFERENCES Servers(ServerId),
            FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
        );";

            connection.Execute(createChannelsTableQuery);

            // Create ServerOwners table
            var createServerOwnersTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ServerOwners' and xtype='U')
        CREATE TABLE ServerOwners (
            ServerId UNIQUEIDENTIFIER,
            UserId INT,
            FOREIGN KEY (ServerId) REFERENCES Servers(ServerId),
            FOREIGN KEY (UserId) REFERENCES Users(UserId),
            PRIMARY KEY (ServerId, UserId)
        );";

            connection.Execute(createServerOwnersTableQuery);

            // Create AuthTokens table
            var createAuthTokensTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AuthTokens' and xtype='U')
        CREATE TABLE AuthTokens (
            TokenId INT PRIMARY KEY IDENTITY(1,1),
            UserId INT NOT NULL,
            Token NVARCHAR(255) NOT NULL UNIQUE,
            DeviceName NVARCHAR(255) NOT NULL DEFAULT 'Unknown Device',
            Platform NVARCHAR(100) NOT NULL DEFAULT 'Unknown',
            CreatedAt DATETIME NOT NULL,
            ExpiresAt DATETIME NOT NULL,
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );";

            connection.Execute(createAuthTokensTableQuery);

            // Migration: add DeviceName/Platform to existing AuthTokens tables
            var migrateAuthTokensQuery = @"
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AuthTokens') AND name = 'DeviceName')
            ALTER TABLE AuthTokens ADD DeviceName NVARCHAR(255) NOT NULL DEFAULT 'Unknown Device';
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AuthTokens') AND name = 'Platform')
            ALTER TABLE AuthTokens ADD Platform NVARCHAR(100) NOT NULL DEFAULT 'Unknown';";

            try { connection.Execute(migrateAuthTokensQuery); } catch { }

            // Create FriendRequests table
            var createFriendRequestsTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='FriendRequests' and xtype='U')
        CREATE TABLE FriendRequests (
            RequestId INT PRIMARY KEY IDENTITY(1,1),
            SenderId INT NOT NULL,
            ReceiverId INT NOT NULL,
            Status INT NOT NULL DEFAULT 0,
            CreatedAt DATETIME NOT NULL,
            RespondedAt DATETIME,
            FOREIGN KEY (SenderId) REFERENCES Users(UserId),
            FOREIGN KEY (ReceiverId) REFERENCES Users(UserId),
            CONSTRAINT UQ_FriendRequest UNIQUE (SenderId, ReceiverId)
        );";

            connection.Execute(createFriendRequestsTableQuery);

            // Create BlockedUsers table
            var createBlockedUsersTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BlockedUsers' and xtype='U')
        CREATE TABLE BlockedUsers (
            BlockId INT PRIMARY KEY IDENTITY(1,1),
            BlockerId INT NOT NULL,
            BlockedUserId INT NOT NULL,
            BlockedAt DATETIME NOT NULL,
            FOREIGN KEY (BlockerId) REFERENCES Users(UserId),
            FOREIGN KEY (BlockedUserId) REFERENCES Users(UserId),
            CONSTRAINT UQ_BlockedUser UNIQUE (BlockerId, BlockedUserId)
        );";

            connection.Execute(createBlockedUsersTableQuery);

            // Create ProfileConnections table
            var createProfileConnectionsTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProfileConnections' and xtype='U')
        CREATE TABLE ProfileConnections (
            ConnectionId INT PRIMARY KEY IDENTITY(1,1),
            UserId INT NOT NULL,
            ServiceName NVARCHAR(100) NOT NULL,
            ServiceUsername NVARCHAR(255) NOT NULL,
            ServiceUrl NVARCHAR(500),
            IsVisible BIT NOT NULL DEFAULT 1,
            ConnectedAt DATETIME NOT NULL,
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );";

            connection.Execute(createProfileConnectionsTableQuery);

            // Create UserThemePreferences table
            var createUserThemePreferencesTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserThemePreferences' and xtype='U')
        CREATE TABLE UserThemePreferences (
            UserId INT PRIMARY KEY,
            ThemeName NVARCHAR(50) NOT NULL DEFAULT 'Dark',
            IsDarkMode BIT NOT NULL DEFAULT 1,
            AccentColor NVARCHAR(20) NOT NULL DEFAULT '#7289DA',
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );";

            connection.Execute(createUserThemePreferencesTableQuery);

            // Create VoiceSettings table
            var createVoiceSettingsTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='VoiceSettings' and xtype='U')
        CREATE TABLE VoiceSettings (
            UserId INT PRIMARY KEY,
            InputDevice NVARCHAR(255) NOT NULL DEFAULT 'Default',
            OutputDevice NVARCHAR(255) NOT NULL DEFAULT 'Default',
            PushToTalk BIT NOT NULL DEFAULT 0,
            PushToTalkKey NVARCHAR(50) NOT NULL DEFAULT '',
            NoiseSuppression BIT NOT NULL DEFAULT 1,
            InputVolume INT NOT NULL DEFAULT 100,
            OutputVolume INT NOT NULL DEFAULT 100,
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );";

            connection.Execute(createVoiceSettingsTableQuery);

            // Create VideoSettings table
            var createVideoSettingsTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='VideoSettings' and xtype='U')
        CREATE TABLE VideoSettings (
            UserId INT PRIMARY KEY,
            Camera NVARCHAR(255) NOT NULL DEFAULT 'Default',
            Resolution NVARCHAR(20) NOT NULL DEFAULT '1280x720',
            Fps INT NOT NULL DEFAULT 30,
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );";

            connection.Execute(createVideoSettingsTableQuery);

            // Create AccessibilitySettings table
            var createAccessibilitySettingsTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AccessibilitySettings' and xtype='U')
        CREATE TABLE AccessibilitySettings (
            UserId INT PRIMARY KEY,
            FontSize FLOAT NOT NULL DEFAULT 14.0,
            HighContrast BIT NOT NULL DEFAULT 0,
            ScreenReader BIT NOT NULL DEFAULT 0,
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );";

            connection.Execute(createAccessibilitySettingsTableQuery);

            // Create DeveloperSettings table
            var createDeveloperSettingsTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DeveloperSettings' and xtype='U')
        CREATE TABLE DeveloperSettings (
            UserId INT PRIMARY KEY,
            DebugMode BIT NOT NULL DEFAULT 0,
            WebSocketInspector BIT NOT NULL DEFAULT 0,
            EnableDevTools BIT NOT NULL DEFAULT 0,
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );";

            connection.Execute(createDeveloperSettingsTableQuery);

            // Create PrivacySettings table
            var createPrivacySettingsTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PrivacySettings' and xtype='U')
        CREATE TABLE PrivacySettings (
            UserId INT PRIMARY KEY,
            DmPrivacy INT NOT NULL DEFAULT 1,
            FriendRequestFilter INT NOT NULL DEFAULT 0,
            ContentFilter INT NOT NULL DEFAULT 1,
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );";

            connection.Execute(createPrivacySettingsTableQuery);

            // Create TwoFactorAuth table
            var createTwoFactorAuthTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TwoFactorAuth' and xtype='U')
        CREATE TABLE TwoFactorAuth (
            UserId INT PRIMARY KEY,
            IsEnabled BIT NOT NULL DEFAULT 0,
            Secret NVARCHAR(255),
            BackupCodes NVARCHAR(MAX),
            EnabledAt DATETIME,
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );";

            connection.Execute(createTwoFactorAuthTableQuery);

            // Create PasswordResetTokens table
            var createPasswordResetTokensTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PasswordResetTokens' and xtype='U')
        CREATE TABLE PasswordResetTokens (
            TokenId INT PRIMARY KEY IDENTITY(1,1),
            UserId INT NOT NULL,
            Code NVARCHAR(10) NOT NULL,
            CreatedAt DATETIME NOT NULL,
            ExpiresAt DATETIME NOT NULL,
            IsUsed BIT NOT NULL DEFAULT 0,
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );";

            connection.Execute(createPasswordResetTokensTableQuery);

            // Create EmailVerificationTokens table
            var createEmailVerificationTokensTableQuery = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmailVerificationTokens' and xtype='U')
        CREATE TABLE EmailVerificationTokens (
            TokenId INT PRIMARY KEY IDENTITY(1,1),
            UserId INT NOT NULL,
            Code NVARCHAR(10) NOT NULL,
            CreatedAt DATETIME NOT NULL,
            ExpiresAt DATETIME NOT NULL,
            IsUsed BIT NOT NULL DEFAULT 0,
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
        );";

            connection.Execute(createEmailVerificationTokensTableQuery);

            // Migration: add IsTwoFactorEnabled column to Users if not present
            try
            {
                connection.Execute(@"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'IsTwoFactorEnabled' AND Object_ID = Object_ID('Users'))
                    ALTER TABLE Users ADD IsTwoFactorEnabled BIT NOT NULL DEFAULT 0;");
            }
            catch { }

            // Migration: add IsEmailVerified column to Users if not present (DEFAULT 1 so existing accounts are treated as verified)
            try
            {
                connection.Execute(@"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'IsEmailVerified' AND Object_ID = Object_ID('Users'))
                    ALTER TABLE Users ADD IsEmailVerified BIT NOT NULL DEFAULT 1;");
            }
            catch { }

            // Seed data for Users
            var seedUsersQuery = @"
            INSERT INTO Users (Name, Email, PasswordHash, Description, CreationDate, Discriminator, IsActive)
            VALUES 
            ('Alice', 'alice@example.com', '$2a$11$XZKDqGKqV3F6z.6YyKJ8JOZq0YLKQmJ8qX9L3jYVZ8n8.5Kl6vJYm', 'First user', GETDATE(), 1001, 1),
            ('Bob', 'bob@example.com', '$2a$11$XZKDqGKqV3F6z.6YyKJ8JOZq0YLKQmJ8qX9L3jYVZ8n8.5Kl6vJYm', 'Second user', GETDATE(), 1002, 1),
            ('Charlie', 'charlie@example.com', '$2a$11$XZKDqGKqV3F6z.6YyKJ8JOZq0YLKQmJ8qX9L3jYVZ8n8.5Kl6vJYm', 'Third user', GETDATE(), 1003, 1);";

            try
            {
                connection.Execute(seedUsersQuery);
            }
            catch (Exception) { }

            // Seed data for Servers
            var seedServersQuery = @"
            INSERT INTO Servers (ServerId, Name, Description, Image, CreationDate, AddDate)
            VALUES 
            (NEWID(), 'Server 1', 'Description for Server 1', NULL, GETDATE(), GETDATE()),
            (NEWID(), 'Server 2', 'Description for Server 2', NULL, GETDATE(), GETDATE()),
            (NEWID(), 'Server 3', 'Description for Server 3', NULL, GETDATE(), GETDATE());";

            try
            {
                connection.Execute(seedServersQuery);
            }
            catch (Exception) { }

            // In a real application, you might have logic to determine these associations
            var seedServerOwnersQuery = @"
            INSERT INTO ServerOwners (ServerId, UserId)
            VALUES 
            ((SELECT TOP 1 ServerId FROM Servers WHERE Name = 'Server 1'), (SELECT TOP 1 UserId FROM Users WHERE Name = 'Alice')),
            ((SELECT TOP 1 ServerId FROM Servers WHERE Name = 'Server 2'), (SELECT TOP 1 UserId FROM Users WHERE Name = 'Bob')),
            ((SELECT TOP 1 ServerId FROM Servers WHERE Name = 'Server 3'), (SELECT TOP 1 UserId FROM Users WHERE Name = 'Charlie'));";

            try
            {
                connection.Execute(seedServerOwnersQuery);
            }
            catch (Exception) { }
        }

        public static void DisposeDatabase()
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();
                try
                {
                    var dropServersTable = "DROP TABLE IF EXISTS Servers";
                    connection.Execute(dropServersTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropUsersTable = "DROP TABLE IF EXISTS Users";
                    connection.Execute(dropUsersTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }
                
                try
                {
                    var dropServerOwnersTable = "DROP TABLE IF EXISTS ServerOwners";
                    connection.Execute(dropServerOwnersTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropChannelsTable = "DROP TABLE IF EXISTS Channels";
                    connection.Execute(dropChannelsTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropAuthTokensTable = "DROP TABLE IF EXISTS AuthTokens";
                    connection.Execute(dropAuthTokensTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropFriendRequestsTable = "DROP TABLE IF EXISTS FriendRequests";
                    connection.Execute(dropFriendRequestsTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropBlockedUsersTable = "DROP TABLE IF EXISTS BlockedUsers";
                    connection.Execute(dropBlockedUsersTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropProfileConnectionsTable = "DROP TABLE IF EXISTS ProfileConnections";
                    connection.Execute(dropProfileConnectionsTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropUserThemePreferencesTable = "DROP TABLE IF EXISTS UserThemePreferences";
                    connection.Execute(dropUserThemePreferencesTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
