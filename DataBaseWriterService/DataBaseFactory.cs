using DataBaseWriterService._Interfaces_;

namespace DataBaseWriterService
{
    internal class DataBaseFactory : IDataBaseFactory
    {
        public IDataBaseContext Create()
        {
            return new RootDbContext();
        }
    }
}