using FakeRaspberryAwards.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FakeRaspberryAwards.Infrastructure.Database
{
    public class DatabaseContext : DbContext
    {
        private static SqliteConnection SqliteConnection { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (SqliteConnection == null)
            {
                SqliteConnection = new SqliteConnection("Data Source=:memory:");
                SqliteConnection.Open();
            }

            optionsBuilder
                .UseSqlite(SqliteConnection)
                .UseLazyLoadingProxies();

            EnableLogWhenDebugging(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            {
                var builder = modelBuilder.Entity<Studio>();
                builder.ToTable("Studio");
                builder.HasKey(o => o.Id);
            }

            {
                var builder = modelBuilder.Entity<Producer>();
                builder.ToTable("Producer");
                builder.HasKey(o => o.Id);
            }

            {
                var builder = modelBuilder.Entity<Movie>();
                builder.ToTable("Movie");
                builder.HasKey(o => o.Id);
                builder.HasMany(o => o.Studios).WithMany(o => o.Movies);
                builder.HasMany(o => o.Producers).WithMany(o => o.Movies);
            }
        }

        [Conditional("DEBUG")]
        private static void EnableLogWhenDebugging(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(message => Debug.WriteLine(message));
        }

        public static void CloseConnection()
        {
            if (SqliteConnection != null)
            {
                SqliteConnection.Close();
                SqliteConnection.Dispose();
            }
        }
    }
}
