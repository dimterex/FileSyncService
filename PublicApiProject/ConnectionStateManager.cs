namespace PublicProject
{
    using System.Collections.Concurrent;

    using _Interfaces_;

    public class ConnectionStateManager : IConnectionStateManager
    {
        private readonly ConcurrentDictionary<string, string> _loginToTokenMap;
        private readonly ConcurrentDictionary<string, string> _tokenToLoginMap;

        public ConnectionStateManager()
        {
            _tokenToLoginMap = new ConcurrentDictionary<string, string>();
            _loginToTokenMap = new ConcurrentDictionary<string, string>();
        }

        public void Add(string login, string token)
        {
            _tokenToLoginMap.TryAdd(token, login);
            _loginToTokenMap.TryAdd(login, token);
        }

        public void Remove(string token)
        {
            if (_tokenToLoginMap.TryRemove(token, out string login))
                _loginToTokenMap.TryRemove(login, out _);
        }

        public string GetLoginByToken(string token)
        {
            if (_tokenToLoginMap.TryGetValue(token, out string login))
                return login;

            return string.Empty;
        }
    }
}
