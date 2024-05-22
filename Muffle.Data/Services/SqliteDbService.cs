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
                    var dropServersTable = "DROP TABLE IF EXISTS Servers";
                    connection.Execute(dropServersTable);
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
            }
            catch (Exception ex)
            {

            }           

            
        }

    }
}
