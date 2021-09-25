namespace Service.DataBase
{
    public class DataBaseFactory
    {
        public RootDbContext Create()
        {
            return new RootDbContext();
        }
    }
}