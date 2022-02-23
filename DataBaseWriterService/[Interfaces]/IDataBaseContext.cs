using System;
using DataBaseWriterService.Dto;
using Microsoft.EntityFrameworkCore;

namespace DataBaseWriterService._Interfaces_
{
    internal interface IDataBaseContext : IDisposable
    {
        DbSet<SyncState> SyncStates { get; set; }
        DbSet<User> Users { get; set; }
        void ApplyChanges();
    }
}