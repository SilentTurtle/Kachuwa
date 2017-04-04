using System.Data;
using Kachuwa.Data.Crud;

namespace Kachuwa.Data
{
    /// <summary>
    /// This class is a factory class that creates 
    /// data-base specific factories which in turn create data acces objects
    /// </summary>
    public class DatabaseFactories
    {
        /// <summary>
        ///  gets a provider specific (i.e. database specific) factory 
        /// </summary>
        /// <param name="dialect"></param>
        /// <param name="connectionString"></param>
        /// <returns>an instance of service factory of given provider.</returns>
        public static IDatabaseFactory GetFactory(Dialect dialect,string connectionString)
        {
            // return the requested DaoFactory

            switch (dialect)
            {
                //instance of corresponding provider
                //case Dialect.MySQL:
                //    break;
                //case Dialect.PostgreSQL:
                //    break;
                case Dialect.SQLServer:
                    var dbfactory= new MsSQLFactory(connectionString);
                    DbFactoryProvider.SetCurrentDbFactory(dbfactory);
                    return DbFactoryProvider.GetFactory();
                //case Dialect.SQLite:
                //    break;

                default:
                    var dbFactory = new MsSQLFactory(connectionString);
                    DbFactoryProvider.SetCurrentDbFactory(dbFactory);
                    return DbFactoryProvider.GetFactory();
            }
        }
    }
}