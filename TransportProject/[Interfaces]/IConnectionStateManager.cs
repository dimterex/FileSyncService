namespace TransportProject
{
    public interface IConnectionStateManager
    {
        void Add(string login, string token);
        void Remove(string token);
        string GetLoginByToken(string token);
    }
}