using System;

namespace Service.Transport
{
    public class UnathorizedConnection
    {
        private const int UNACTIVITY_CONNECT_TIMEOUT = 3000;
        
        #region Fields

        private readonly IClient _connection;

        private readonly int _connectionTimestamp;

        #endregion Fields

        #region Constructors

        public UnathorizedConnection(IClient connection)
        {
            _connection = connection;
            _connectionTimestamp = Environment.TickCount;
        }

        #endregion Constructors

        #region Methods

        public bool Is(IClient connection)
        {
            return _connection == connection;
        }

        public bool Check()
        {
            if (Environment.TickCount - _connectionTimestamp > UNACTIVITY_CONNECT_TIMEOUT)
            {
                _connection.Close();
                return true;
            }

            return false;
        }

        #endregion Methods
    }
}