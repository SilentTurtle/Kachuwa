using System;
using System.Data;
using System.Data.SqlClient;
using Kachuwa.Data.Crud;
using Kachuwa.Log;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace Kachuwa.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class MySQLFactory : IDatabaseFactory
    {
        public IDbConnection Db { get; set; }
        public Dialect Dialect => Dialect.MySQL;

        public QueryBuilder QueryBuilder { get; }

        private readonly string _connectionString;

        public IDbConnection GetConnection()
        {
            Db = new MySqlConnection(_connectionString);
            return Db;
        }
        
        public MySQLFactory(IConfiguration configuration, IServiceProvider serviceProvider)
        {
           _connectionString = configuration.GetConnectionString("DefaultConnection");
            Db = new MySqlConnection(_connectionString);
            QueryBuilder = new MySqlQueryBuilder(new MySQLTemplate());
            var hostingenv = serviceProvider.GetService<IHostingEnvironment>();
            DbLogger = new DbLogger(hostingenv,new DefaultDbLoggerSetting());
        }

        public void Dispose()
        {
            if (Db.State == ConnectionState.Open)
                Db.Close();
            Db.Close();
            //db.Dispose();
        }

        public ILogger DbLogger { get; set; }
    }


}