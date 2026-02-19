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
                Discriminator INTEGER NOT NULL DEFAULT 0
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
                Port INTEGER NOT NULL
            );";

            connection.Execute(createServersTableQuery);

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
                CreatedAt DATETIME NOT NULL,
                ExpiresAt DATETIME NOT NULL,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            connection.Execute(createAuthTokensTableQuery);

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

            // Seed Users data (password is 'password123' hashed with BCrypt)
            var seedUsersQuery = @"
                INSERT INTO Users (UserId, Name, Email, PasswordHash, Description, CreationDate, Discriminator)
                VALUES 
                (1, 'Alice', 'alice@example.com', '$2a$11$XZKDqGKqV3F6z.6YyKJ8JOZq0YLKQmJ8qX9L3jYVZ8n8.5Kl6vJYm', 'First user', datetime('now'), 1001),
                (2, 'Bob', 'bob@example.com', '$2a$11$XZKDqGKqV3F6z.6YyKJ8JOZq0YLKQmJ8qX9L3jYVZ8n8.5Kl6vJYm', 'Second user', datetime('now'), 1002),
                (3, 'Charlie', 'charlie@example.com', '$2a$11$XZKDqGKqV3F6z.6YyKJ8JOZq0YLKQmJ8qX9L3jYVZ8n8.5Kl6vJYm', 'Third user', datetime('now'), 1003);";

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
                INSERT INTO Servers (Id, Name, Description, IpAddress, Port)
                VALUES 
                (0, 'Dynamic database test', 'Description for Server1', '192.168.1.1', 8080),
                (1, 'Example 2', 'Description for Server2', '192.168.1.2', 8081),
                (2, 'Tyler', 'Description for Server3', '192.168.1.3', 8082);";

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
                    var dropServerOwnersTable = "DROP TABLE IF EXISTS ServerOwners";
                    connection.Execute(dropServerOwnersTable);
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
            }
            catch (Exception ex)
            {

            }           

            
        }

    }
}
