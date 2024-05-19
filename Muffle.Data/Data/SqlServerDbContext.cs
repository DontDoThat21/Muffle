using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Muffle.Data.Models;

namespace Muffle.Data.Data
{
    //public DbSet<example> example { get; set; }

    public class SqlServerDbContext : DbContext
    {

        public DbSet<User> users { get; set; }
        public DbSet<Server> servers { get; set; }
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed the database with default servers
            modelBuilder.Entity<Server>().HasData(
                new Server(0, "Server1", "Description for Server1", "192.168.1.1", 8080),
                new Server(1, "Server2", "Description for Server2", "192.168.1.2", 8081),
                new Server(2, "Server3", "Description for Server3", "192.168.1.3", 8082)
            );
        }
    }
}
