using Common.DatabaseProject._Interfaces_;

namespace Common.DatabaseProject
{
    public class DataBaseFactory : IDataBaseFactory
    {
        private readonly string _dbPath;

        public DataBaseFactory(string dbPath)
        {
            _dbPath = dbPath;
        }

        public IDataBaseContext Create()
        {
            return new RootDbContext(_dbPath);
        }
    }
}