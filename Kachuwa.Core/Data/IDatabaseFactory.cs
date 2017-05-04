using System;
using System.Data;
using Kachuwa.Data.Crud;
using Kachuwa.Log;

namespace Kachuwa.Data
{
    public interface IDatabaseFactory : IDisposable
    {
        IDbConnection Db { get; }
        Dialect Dialect { get; }
        QueryBuilder QueryBuilder { get; }
        IDbConnection GetConnection();
        ILogger DbLogger { get; set; }
     }
}