﻿namespace Common.DatabaseProject
{
    using System.IO;

    using _Interfaces_;

    using Dto;

    using Microsoft.EntityFrameworkCore;

    internal class RootDbContext : DbContext, IDataBaseContext
    {
        private const string DB_NAME = "sync_service.db";

        private readonly string _dbPath;

        public RootDbContext(string dbPath)
        {
            _dbPath = dbPath;
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<SyncState> SyncStates { get; set; }

        public DbSet<HistoryDto> History { get; set; }

        public void ApplyChanges()
        {
            SaveChanges();
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={Path.Combine(_dbPath, DB_NAME)}");
        }
    }
}
