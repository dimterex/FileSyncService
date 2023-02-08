namespace Common.DatabaseProject._Interfaces_
{
    using System;

    using Dto;

    using Microsoft.EntityFrameworkCore;

    public interface IDataBaseContext : IDisposable
    {
        DbSet<SyncState> SyncStates { get; }
        DbSet<User> Users { get; }
        DbSet<HistoryDto> History { get; }
        void ApplyChanges();
    }
}
