﻿using DevChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace DevChallenge.Data.Contexts
{
    public class DevChallengeDbContext : DbContext
    {
        private readonly string _connectionString = "Data Source = DevChallenge.db";
        public DbSet<Ibge> Ibges { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}