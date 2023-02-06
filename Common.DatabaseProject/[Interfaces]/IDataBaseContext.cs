using System;
using Common.DatabaseProject.Dto;
using Microsoft.EntityFrameworkCore;

namespace Common.DatabaseProject._Interfaces_
{
    public interface IDataBaseContext : IDisposable
    {
        DbSet<SyncState> SyncStates { get; }
        DbSet<User> Users { get; }
        DbSet<HistoryDto> History { get; }
        void ApplyChanges();
    }
}