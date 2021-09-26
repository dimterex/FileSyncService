﻿using System;
using System.IO;
using DataBaseProject.Dto;
using Microsoft.EntityFrameworkCore;

namespace DataBaseProject
{
    internal class RootDbContext : DbContext, IDataBaseContext
    {
        public DbSet<User> Users { get; set; }
        
        public DbSet<SyncState> SyncStates { get; set; }
        
        public string DbPath { get; }

        public RootDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Combine(path, "sync_service.db");
            
            Database.EnsureCreated();
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        public void ApplyChanges()
        {
            SaveChanges();
        }
    }
}