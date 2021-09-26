using System;
using System.Collections.Generic;
using DataBaseProject.Dto;
using Microsoft.EntityFrameworkCore;

namespace DataBaseProject
{
    internal interface IDataBaseContext : IDisposable
    {
        DbSet<SyncState> SyncStates { get; set; }
        DbSet<User> Users { get; set; }
        void ApplyChanges();
    }
}