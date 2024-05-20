using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Muffle;
using Muffle.Data.Data;
using System.Data;
using Dapper;

namespace Muffle.Data.Services
{
    public class SqliteDbService
    {
        private static readonly string _connectionString = ConfigurationLoader.GetConnectionString("SqliteConnection");

        public SqliteDbService()
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
            var createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Servers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                IpAddress TEXT NOT NULL,
                Port INTEGER NOT NULL
            );";

            connection.Execute(createTableQuery);

            try
            {
                // Seed data
                var seedDataQuery = @"
                    INSERT INTO Servers (Id, Name, Description, IpAddress, Port)
                    VALUES 
                    (0, 'Dynamic database test', 'Description for Server1', '192.168.1.1', 8080),
                    (1, 'Example 2', 'Description for Server2', '192.168.1.2', 8081),
                    (2, 'Tyler is black', 'Description for Server3', '192.168.1.3', 8082);";

                connection.Execute(seedDataQuery);
            }
            catch (Exception)
            {
            }            
        }

        public static void DisposeDatabase()
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

    }
}
