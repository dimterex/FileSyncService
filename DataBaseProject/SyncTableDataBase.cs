﻿using System.Collections.Generic;
using System.Linq;
using DataBaseProject.Dto;

namespace DataBaseProject
{
    public class SyncTableDataBase : ISyncTableDataBase
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public SyncTableDataBase()
        {
            _dataBaseFactory = new DataBaseFactory();
        }

        public IList<string> GetSyncStates(string login)
        {
            var result = new List<string>();
            using (var dataBase = _dataBaseFactory.Create())
            {
                var syncStates = dataBase.SyncStates.ToList().Where(x => x.Login == login);
                foreach (var syncState in syncStates)
                {
                    result.Add(syncState.FilePath);
                }
            }

            return result;
        }

        public void AddState(string login, string filePath)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                dataBase.SyncStates.Add(new SyncState()
                {
                    Login = login,
                    FilePath = filePath
                });

                dataBase.ApplyChanges();
            }
        }

        public void RemoveSyncStates(string login, IList<string> paths)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                var syncState = from b in dataBase.SyncStates
                    where b.Login == login && paths.Contains(b.FilePath)
                    select b;
              

                dataBase.SyncStates.RemoveRange(syncState);
                dataBase.ApplyChanges();
            }
        }
    }
}