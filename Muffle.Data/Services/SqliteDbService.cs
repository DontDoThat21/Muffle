using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;
using Muffle.Data.Models;
using System.Collections.ObjectModel;

namespace Muffle.Data.Services
{
    public class SQLiteDbService
    {
        private static readonly string _connectionString = ConfigurationLoader.GetConnectionString("SqliteConnection");

        public SQLiteDbService()
        {

        }
        
        public static IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public static void InitializeDatabase()
        {
            using var connection = CreateConnection();
            connection.Open();

            // Create Users table
            var createUsersTableQuery = @"
            CREATE TABLE IF NOT EXISTS Users (
                UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT UNIQUE NOT NULL,
                PasswordHash TEXT NOT NULL,
                Description TEXT,
                CreationDate DATETIME NOT NULL,
                Discriminator INTEGER NOT NULL DEFAULT 0,
                IsActive INTEGER NOT NULL DEFAULT 1,
                DisabledAt DATETIME,
                AvatarUrl TEXT,
                BannerUrl TEXT,
                AboutMe TEXT,
                Pronouns TEXT,
                Status INTEGER NOT NULL DEFAULT 0,
                CustomStatusText TEXT,
                CustomStatusEmoji TEXT,
                ShowOnlineStatus INTEGER NOT NULL DEFAULT 1,
                IsEmailVerified INTEGER NOT NULL DEFAULT 1
            );";

            connection.Execute(createUsersTableQuery);

            // Create index for username + discriminator lookups
            var createUsernameDiscriminatorIndexQuery = @"
            CREATE INDEX IF NOT EXISTS idx_users_name_discriminator 
            ON Users(Name, Discriminator);";

            connection.Execute(createUsernameDiscriminatorIndexQuery);

            // Create Servers table
            var createServersTableQuery = @"
            CREATE TABLE IF NOT EXISTS Servers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                IpAddress TEXT NOT NULL,
                Port INTEGER NOT NULL,
                IsPublic INTEGER NOT NULL DEFAULT 0,
                IconUrl TEXT
            );";

            connection.Execute(createServersTableQuery);

            // Create Channels table
            var createChannelsTableQuery = @"
            CREATE TABLE IF NOT EXISTS Channels (
                ChannelId INTEGER PRIMARY KEY AUTOINCREMENT,
                ServerId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Description TEXT,
                Type INTEGER NOT NULL DEFAULT 0,
                Position INTEGER NOT NULL DEFAULT 0,
                CreatedAt DATETIME NOT NULL,
                CreatedBy INTEGER NOT NULL,
                IconUrl TEXT,
                FOREIGN KEY (ServerId) REFERENCES Servers(Id),
                FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
            );";

            connection.Execute(createChannelsTableQuery);

            // Create ServerOwners table
            var createServerOwnersTableQuery = @"
            CREATE TABLE IF NOT EXISTS ServerOwners (
                ServerId INTEGER,
                UserId INTEGER,
                FOREIGN KEY (ServerId) REFERENCES Servers(Id),
                PRIMARY KEY (ServerId, UserId)
            );";

            connection.Execute(createServerOwnersTableQuery);

            // Create Friends table
            var createFriendsTableQuery = @"
            CREATE TABLE IF NOT EXISTS Friends (
                UserId INTEGER NOT NULL,
                Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                Memo TEXT,
                Image TEXT,
                FriendshipDate DATETIME NOT NULL,
                CreationDate DATETIME NOT NULL,
                UNIQUE (UserId, Id)
            );";

            // need to manually handle auto increments for composite primary keys
            connection.Execute(createFriendsTableQuery);

            // Create AuthTokens table
            var createAuthTokensTableQuery = @"
            CREATE TABLE IF NOT EXISTS AuthTokens (
                TokenId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Token TEXT NOT NULL UNIQUE,
                DeviceName TEXT NOT NULL DEFAULT 'Unknown Device',
                Platform TEXT NOT NULL DEFAULT 'Unknown',
                IpAddress TEXT NOT NULL DEFAULT '',
                CreatedAt DATETIME NOT NULL,
                ExpiresAt DATETIME NOT NULL,
                LastUsedAt DATETIME,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createAuthTokensTableQuery);

            // Migration: add DeviceName/Platform to existing AuthTokens tables
            try { connection.Execute("ALTER TABLE AuthTokens ADD COLUMN DeviceName TEXT NOT NULL DEFAULT 'Unknown Device';"); } catch { }
            try { connection.Execute("ALTER TABLE AuthTokens ADD COLUMN Platform TEXT NOT NULL DEFAULT 'Unknown';"); } catch { }
            try { connection.Execute("ALTER TABLE AuthTokens ADD COLUMN IpAddress TEXT NOT NULL DEFAULT '';"); } catch { }
            try { connection.Execute("ALTER TABLE AuthTokens ADD COLUMN LastUsedAt DATETIME;"); } catch { }

            // Create FriendRequests table
            var createFriendRequestsTableQuery = @"
            CREATE TABLE IF NOT EXISTS FriendRequests (
                RequestId INTEGER PRIMARY KEY AUTOINCREMENT,
                SenderId INTEGER NOT NULL,
                ReceiverId INTEGER NOT NULL,
                Status INTEGER NOT NULL DEFAULT 0,
                CreatedAt DATETIME NOT NULL,
                RespondedAt DATETIME,
                FOREIGN KEY (SenderId) REFERENCES Users(UserId),
                FOREIGN KEY (ReceiverId) REFERENCES Users(UserId),
                UNIQUE (SenderId, ReceiverId)
            );";

            connection.Execute(createFriendRequestsTableQuery);

            // Create BlockedUsers table
            var createBlockedUsersTableQuery = @"
            CREATE TABLE IF NOT EXISTS BlockedUsers (
                BlockId INTEGER PRIMARY KEY AUTOINCREMENT,
                BlockerId INTEGER NOT NULL,
                BlockedUserId INTEGER NOT NULL,
                BlockedAt DATETIME NOT NULL,
                FOREIGN KEY (BlockerId) REFERENCES Users(UserId),
                FOREIGN KEY (BlockedUserId) REFERENCES Users(UserId),
                UNIQUE (BlockerId, BlockedUserId)
            );";

            connection.Execute(createBlockedUsersTableQuery);

            // Create InviteLinks table
            var createInviteLinksTableQuery = @"
            CREATE TABLE IF NOT EXISTS InviteLinks (
                InviteLinkId INTEGER PRIMARY KEY AUTOINCREMENT,
                ServerId INTEGER NOT NULL,
                Code TEXT NOT NULL UNIQUE,
                CreatedBy INTEGER NOT NULL,
                CreatedAt DATETIME NOT NULL,
                ExpiresAt DATETIME,
                MaxUses INTEGER,
                UseCount INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (ServerId) REFERENCES Servers(Id),
                FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
            );";

            connection.Execute(createInviteLinksTableQuery);

            // Create ServerRoles table
            var createServerRolesTableQuery = @"
            CREATE TABLE IF NOT EXISTS ServerRoles (
                RoleId INTEGER PRIMARY KEY AUTOINCREMENT,
                ServerId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Permissions INTEGER NOT NULL DEFAULT 1,
                Position INTEGER NOT NULL DEFAULT 0,
                Color TEXT,
                FOREIGN KEY (ServerId) REFERENCES Servers(Id)
            );";

            connection.Execute(createServerRolesTableQuery);

            // Create ServerMembers table
            var createServerMembersTableQuery = @"
            CREATE TABLE IF NOT EXISTS ServerMembers (
                ServerId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                RoleId INTEGER,
                Nickname TEXT,
                JoinedAt DATETIME NOT NULL,
                PRIMARY KEY (ServerId, UserId),
                FOREIGN KEY (ServerId) REFERENCES Servers(Id),
                FOREIGN KEY (UserId) REFERENCES Users(UserId),
                FOREIGN KEY (RoleId) REFERENCES ServerRoles(RoleId)
            );";

            connection.Execute(createServerMembersTableQuery);

            // Create ChannelPermissions table
            var createChannelPermissionsTableQuery = @"
            CREATE TABLE IF NOT EXISTS ChannelPermissions (
                ChannelId INTEGER NOT NULL,
                RoleId INTEGER NOT NULL,
                AllowRead INTEGER NOT NULL DEFAULT 1,
                AllowSend INTEGER NOT NULL DEFAULT 1,
                AllowManage INTEGER NOT NULL DEFAULT 0,
                PRIMARY KEY (ChannelId, RoleId),
                FOREIGN KEY (ChannelId) REFERENCES Channels(ChannelId),
                FOREIGN KEY (RoleId) REFERENCES ServerRoles(RoleId)
            );";

            connection.Execute(createChannelPermissionsTableQuery);

            // Create Notifications table
            var createNotificationsTableQuery = @"
            CREATE TABLE IF NOT EXISTS Notifications (
                NotificationId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Title TEXT NOT NULL,
                Body TEXT,
                Type INTEGER NOT NULL,
                IsRead INTEGER NOT NULL DEFAULT 0,
                CreatedAt DATETIME NOT NULL,
                RelatedId INTEGER,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createNotificationsTableQuery);

            // Create MessageReactions table
            var createMessageReactionsTableQuery = @"
            CREATE TABLE IF NOT EXISTS MessageReactions (
                ReactionId INTEGER PRIMARY KEY AUTOINCREMENT,
                MessageId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                Emoji TEXT NOT NULL,
                CreatedAt DATETIME NOT NULL,
                UNIQUE(MessageId, UserId, Emoji)
            );";

            connection.Execute(createMessageReactionsTableQuery);

            // Create ProfileConnections table
            var createProfileConnectionsTableQuery = @"
            CREATE TABLE IF NOT EXISTS ProfileConnections (
                ConnectionId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                ServiceName TEXT NOT NULL,
                ServiceUsername TEXT NOT NULL,
                ServiceUrl TEXT,
                IsVisible INTEGER NOT NULL DEFAULT 1,
                ConnectedAt DATETIME NOT NULL,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createProfileConnectionsTableQuery);

            // Create UserThemePreferences table
            var createUserThemePreferencesTableQuery = @"
            CREATE TABLE IF NOT EXISTS UserThemePreferences (
                UserId INTEGER PRIMARY KEY,
                ThemeName TEXT NOT NULL DEFAULT 'Dark',
                IsDarkMode INTEGER NOT NULL DEFAULT 1,
                AccentColor TEXT NOT NULL DEFAULT '#7289DA',
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createUserThemePreferencesTableQuery);

            // Create VoiceSettings table
            var createVoiceSettingsTableQuery = @"
            CREATE TABLE IF NOT EXISTS VoiceSettings (
                UserId INTEGER PRIMARY KEY,
                InputDevice TEXT NOT NULL DEFAULT 'Default',
                OutputDevice TEXT NOT NULL DEFAULT 'Default',
                PushToTalk INTEGER NOT NULL DEFAULT 0,
                PushToTalkKey TEXT NOT NULL DEFAULT '',
                NoiseSuppression INTEGER NOT NULL DEFAULT 1,
                InputVolume INTEGER NOT NULL DEFAULT 100,
                OutputVolume INTEGER NOT NULL DEFAULT 100,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createVoiceSettingsTableQuery);

            // Create VideoSettings table
            var createVideoSettingsTableQuery = @"
            CREATE TABLE IF NOT EXISTS VideoSettings (
                UserId INTEGER PRIMARY KEY,
                Camera TEXT NOT NULL DEFAULT 'Default',
                Resolution TEXT NOT NULL DEFAULT '1280x720',
                Fps INTEGER NOT NULL DEFAULT 30,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createVideoSettingsTableQuery);

            // Create AccessibilitySettings table
            var createAccessibilitySettingsTableQuery = @"
            CREATE TABLE IF NOT EXISTS AccessibilitySettings (
                UserId INTEGER PRIMARY KEY,
                FontSize REAL NOT NULL DEFAULT 14.0,
                HighContrast INTEGER NOT NULL DEFAULT 0,
                ScreenReader INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createAccessibilitySettingsTableQuery);

            // Create DeveloperSettings table
            var createDeveloperSettingsTableQuery = @"
            CREATE TABLE IF NOT EXISTS DeveloperSettings (
                UserId INTEGER PRIMARY KEY,
                DebugMode INTEGER NOT NULL DEFAULT 0,
                WebSocketInspector INTEGER NOT NULL DEFAULT 0,
                EnableDevTools INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createDeveloperSettingsTableQuery);

            // Create PrivacySettings table
            var createPrivacySettingsTableQuery = @"
            CREATE TABLE IF NOT EXISTS PrivacySettings (
                UserId INTEGER PRIMARY KEY,
                DmPrivacy INTEGER NOT NULL DEFAULT 1,
                FriendRequestFilter INTEGER NOT NULL DEFAULT 0,
                ContentFilter INTEGER NOT NULL DEFAULT 1,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createPrivacySettingsTableQuery);

            // Create TwoFactorAuth table
            var createTwoFactorAuthTableQuery = @"
            CREATE TABLE IF NOT EXISTS TwoFactorAuth (
                UserId INTEGER PRIMARY KEY,
                IsEnabled INTEGER NOT NULL DEFAULT 0,
                Secret TEXT,
                BackupCodes TEXT,
                EnabledAt DATETIME,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createTwoFactorAuthTableQuery);

            // Create PasswordResetTokens table
            var createPasswordResetTokensTableQuery = @"
            CREATE TABLE IF NOT EXISTS PasswordResetTokens (
                TokenId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Code TEXT NOT NULL,
                CreatedAt DATETIME NOT NULL,
                ExpiresAt DATETIME NOT NULL,
                IsUsed INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createPasswordResetTokensTableQuery);

            // Create EmailVerificationTokens table
            var createEmailVerificationTokensTableQuery = @"
            CREATE TABLE IF NOT EXISTS EmailVerificationTokens (
                TokenId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Code TEXT NOT NULL,
                CreatedAt DATETIME NOT NULL,
                ExpiresAt DATETIME NOT NULL,
                IsUsed INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createEmailVerificationTokensTableQuery);

            // Create FriendGroups table
            var createFriendGroupsTableQuery = @"
            CREATE TABLE IF NOT EXISTS FriendGroups (
                GroupId INTEGER PRIMARY KEY AUTOINCREMENT,
                OwnerUserId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                SortOrder INTEGER NOT NULL DEFAULT 0,
                RoomKey TEXT NOT NULL UNIQUE,
                CreatedAt DATETIME NOT NULL,
                FOREIGN KEY (OwnerUserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createFriendGroupsTableQuery);

            // Create FriendGroupMembers table
            var createFriendGroupMembersTableQuery = @"
            CREATE TABLE IF NOT EXISTS FriendGroupMembers (
                GroupId INTEGER NOT NULL,
                FriendId INTEGER NOT NULL,
                AddedAt DATETIME NOT NULL,
                PRIMARY KEY (GroupId, FriendId),
                FOREIGN KEY (GroupId) REFERENCES FriendGroups(GroupId)
            );";

            connection.Execute(createFriendGroupMembersTableQuery);

            // Create UserSubscriptions table
            var createUserSubscriptionsTableQuery = @"
            CREATE TABLE IF NOT EXISTS UserSubscriptions (
                SubscriptionId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Tier INTEGER NOT NULL DEFAULT 0,
                Status INTEGER NOT NULL DEFAULT 0,
                StartedAt DATETIME NOT NULL,
                ExpiresAt DATETIME NOT NULL,
                CancelledAt DATETIME,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createUserSubscriptionsTableQuery);

            // Migration: add IsTwoFactorEnabled column to Users if not present
            try { connection.Execute("ALTER TABLE Users ADD COLUMN IsTwoFactorEnabled INTEGER NOT NULL DEFAULT 0;"); } catch { }

            // Migration: add IsEmailVerified column to Users if not present (DEFAULT 1 so existing accounts are treated as verified)
            try { connection.Execute("ALTER TABLE Users ADD COLUMN IsEmailVerified INTEGER NOT NULL DEFAULT 1;"); } catch { }

            // Seed Users data
            var seedUsersQuery = @"
                INSERT INTO Users (UserId, Name, Email, PasswordHash, Description, CreationDate, Discriminator, IsActive)
                VALUES 
                (1, 'Alice', 'alice@example.com', '$2a$11$XZKDqGKqV3F6z.6YyKJ8JOZq0YLKQmJ8qX9L3jYVZ8n8.5Kl6vJYm', 'First user', datetime('now'), 1001, 1),
                (2, 'Bob', 'bob@example.com', '$2a$11$XZKDqGKqV3F6z.6YyKJ8JOZq0YLKQmJ8qX9L3jYVZ8n8.5Kl6vJYm', 'Second user', datetime('now'), 1002, 1),
                (3, 'Charlie', 'charlie@example.com', '$2a$11$XZKDqGKqV3F6z.6YyKJ8JOZq0YLKQmJ8qX9L3jYVZ8n8.5Kl6vJYm', 'Third user', datetime('now'), 1003, 1);";

            try
            {
                connection.Execute(seedUsersQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding Users data: {ex.Message}\n{ex.StackTrace}");
            }

                // Seed data
            var seedDataQueryServers = @"
                INSERT INTO Servers (Id, Name, Description, IpAddress, Port, IsPublic)
                VALUES
                (0, 'Dynamic database test', 'Description for Server1', '192.168.1.1', 8080, 1),
                (1, 'Example 2', 'Description for Server2', '192.168.1.2', 8081, 0),
                (2, 'Tyler', 'Description for Server3', '192.168.1.3', 8082, 0);";

            try
            {
                connection.Execute(seedDataQueryServers);
            }
            catch (Exception){}

            // Seed ServerOwners data
            var seedServerOwnersQuery = @"
                INSERT INTO ServerOwners (ServerId, UserId)
                VALUES 
                (0, 1),
                (1, 2),
                (2, 3);";

            try
            {
                connection.Execute(seedServerOwnersQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while seeding ServerOwners: {ex}");
            }

            // Insert default friends
            var friends = new ObservableCollection<Friend>()
            {
                new Friend(0, "Gabe", "Starcraft 2 Bro test", "gabe.png", "", DateTime.Now, DateTime.Now),
                new Friend(1, "Tylor", "Best Programmer NA C#", "tom.jpg", "", DateTime.Now, DateTime.Now),
                new Friend(2, "Nick", "Army Motorcycling Bro test", "nick.png", "", DateTime.Now, DateTime.Now),
                new Friend(3, "Tyler", "Best 1DGer in da land test", "murky.png", "", DateTime.Now, DateTime.Now),
            };

            // Seed data
            var seedDataQueryFriends = @"
                INSERT INTO Friends (UserId, Id, Name, Description, Memo, Image, FriendshipDate, CreationDate)
                VALUES (@UserId, @Id, @Name, @Description, @Memo, @Image, @FriendshipDate, @CreationDate);";

            // NEED TO ADD CORRECT CONSTRUCTOR and USERID

            foreach (var friend in friends)
            {
                try
                {
                    connection.Execute(seedDataQueryFriends, friend);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static void DisposeDatabase()
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();
                try
                {
                    var dropMessageReactionsTable = "DROP TABLE IF EXISTS MessageReactions";
                    connection.Execute(dropMessageReactionsTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropNotificationsTable = "DROP TABLE IF EXISTS Notifications";
                    connection.Execute(dropNotificationsTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropChannelPermissionsTable = "DROP TABLE IF EXISTS ChannelPermissions";
                    connection.Execute(dropChannelPermissionsTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropInviteLinksTable = "DROP TABLE IF EXISTS InviteLinks";
                    connection.Execute(dropInviteLinksTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropServerMembersTable = "DROP TABLE IF EXISTS ServerMembers";
                    connection.Execute(dropServerMembersTable);
                }
                catch (Exception)
                {
                    // Ignore exceptions
                }

                try
                {
                    var dropServerRolesTable = "DROP TABLE IF EXISTS ServerRoles";
                    connection.Execute(dropServerRolesTable);
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
                    var dropFriendsTable = "DROP TABLE IF EXISTS Friends";
                    connection.Execute(dropFriendsTable);
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
