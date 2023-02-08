﻿namespace PublicProject.Database.Actions.States
{
    using System.Collections.Generic;
    using System.Linq;

    using Common.DatabaseProject._Interfaces_;
    using Common.DatabaseProject.Dto;

    public class AddNewStateExecutor
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public AddNewStateExecutor(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(string login, string filepath)
        {
            using (IDataBaseContext dataBase = _dataBaseFactory.Create())
            {
                AddSyncState(login, filepath, dataBase);
                dataBase.ApplyChanges();
            }
        }

        private void AddSyncState(string login, string filePath, IDataBaseContext dataBase)
        {
            List<SyncState> syncStates = dataBase.SyncStates.ToList().Where(x => x.Login == login && x.FilePath == filePath).ToList();

            if (syncStates.Any())
                return;

            var state = new SyncState
            {
                Login = login,
                FilePath = filePath
            };

            dataBase.SyncStates.Add(state);
        }
    }
}
