using System;
using Common.DatabaseProject.Dto;
using Microsoft.EntityFrameworkCore;

namespace Common.DatabaseProject._Interfaces_
{
    public interface IDataBaseContext : IDisposable
    {
        DbSet<SyncState> SyncStates { get; set; }
        DbSet<User> Users { get; set; }
        void ApplyChanges();
    }
}