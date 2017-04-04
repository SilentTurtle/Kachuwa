using System;
using System.Data;
using System.Data.SqlClient;
using Kachuwa.Data.Crud;

namespace Kachuwa.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class MsSQLFactory : IDatabaseFactory
    {
        public IDbConnection Db { get; set; }
        public Dialect Dialect => Dialect.SQLServer;

        public QueryBuilder QueryBuilder { get; }

        private readonly string _connectionString;

        public IDbConnection GetConnection()
        {
            Db = new SqlConnection(_connectionString);
            return Db;
        }
        //in appsetting file
  //      "DBInfo": {
  //  "Name": "coresample",
  //  "ConnectionString": "User ID=postgres;Password=xxxxxx;Host=localhost;Port=5432;Database=coresample;Pooling=true;"
  //}
        public MsSQLFactory(string conString)
        {
            //IConfiguration configuration
           // connectionString = configuration.GetValue<string>("DBInfo:ConnectionString");
            _connectionString = conString;// ConfigurationManager.ConnectionStrings[connectionStringName].ToString();
            Db = new SqlConnection(_connectionString);
            QueryBuilder = new MsSqlQueryBuilder(new MsSQLTemplate());
        }

        public void Dispose()
        {
            if (Db.State == ConnectionState.Open)
                Db.Close();
            Db.Close();
            //db.Dispose();
        }
    }


}