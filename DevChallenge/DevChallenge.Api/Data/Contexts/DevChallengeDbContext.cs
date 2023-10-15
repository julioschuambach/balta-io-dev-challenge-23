using DevChallenge.Api.Data.Mappings;
using DevChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace DevChallenge.Data.Contexts
{
    public class DevChallengeDbContext : DbContext
    {
        private readonly string _connectionString = "Data Source = DevChallenge.db";
        public DbSet<Location> Locations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LocationMapping());
            modelBuilder.ApplyConfiguration(new UserMapping());
        }
    }
}
