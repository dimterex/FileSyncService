namespace DataBaseProject
{
    internal class DataBaseFactory : IDataBaseFactory
    {
        public IDataBaseContext Create()
        {
            return new RootDbContext();
        }
    }
}