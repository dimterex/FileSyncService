namespace DataBaseWriterService._Interfaces_
{
    internal interface IDataBaseFactory
    {
        IDataBaseContext Create();
    }
}