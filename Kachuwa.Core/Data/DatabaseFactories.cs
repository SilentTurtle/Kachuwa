using System;
using System.Data;
using Kachuwa.Data.Crud;
using Kachuwa.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        /// <param name="serviceProvider"></param>
        /// <returns>an instance of service factory of given provider.</returns>
        public static IDatabaseFactory GetFactory(Dialect dialect, IServiceProvider serviceProvider )
        {
            // return the requested DaoFactory
            var configuration = serviceProvider.GetService<IConfigurationRoot>();
            switch (dialect)
            {
                //instance of corresponding provider
                //case Dialect.MySQL:
                //    break;
                //case Dialect.PostgreSQL:
                //    break;
                case Dialect.SQLServer:
                   
                    var dbfactory= new MsSQLFactory(configuration, serviceProvider);
                    DbFactoryProvider.SetCurrentDbFactory(dbfactory);
                    return DbFactoryProvider.GetFactory();
                //case Dialect.SQLite:
                //    break;

                default:
                    var dbFactory = new MsSQLFactory(configuration, serviceProvider);
                    DbFactoryProvider.SetCurrentDbFactory(dbFactory);
                    return DbFactoryProvider.GetFactory();
            }
        }
    }
}