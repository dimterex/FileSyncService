namespace DataBaseProject
{
    internal interface IDataBaseFactory
    {
        IDataBaseContext Create();
    }
}