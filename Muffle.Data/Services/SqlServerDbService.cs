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
            Description NVARCHAR(MAX),
            CreationDate DATETIME NOT NULL
        );";

            connection.Execute(createUsersTableQuery);

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

            // Seed data for Users
            var seedUsersQuery = @"
            INSERT INTO Users (Name, Description, CreationDate)
            VALUES 
            ('Alice', 'First user', GETDATE()),
            ('Bob', 'Second user', GETDATE()),
            ('Charlie', 'Third user', GETDATE());";

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
            }
            catch (Exception ex)
            {

            }
        }
    }
}
