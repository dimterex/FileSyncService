namespace PublicProject._Interfaces_
{
    public interface IConnectionStateManager
    {
        void Add(string login, string token);
        void Remove(string token);
        string GetLoginByToken(string token);
    }
}
