using System;

namespace Kachuwa.Data
{
    public static class DbFactoryProvider
    {
        private static IDatabaseFactory _currentDatabaseFactory;

        public static void SetCurrentDbFactory(IDatabaseFactory dbFactory)
        {
            _currentDatabaseFactory = dbFactory;
        }

        public static IDatabaseFactory GetFactory(string connectionString)
        {
            IDatabaseFactory dbfactory = _currentDatabaseFactory ?? new MsSQLFactory(connectionString);
            return dbfactory;
        }
        public static IDatabaseFactory GetFactory()
        {
            if(_currentDatabaseFactory==null)
                throw new Exception("Please set first default db factory!");
            return _currentDatabaseFactory;
        }

    }
}