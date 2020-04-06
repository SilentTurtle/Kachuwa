using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Kachuwa.Data.Crud;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Kachuwa.Log;

namespace Kachuwa.Data
{
    public class NpgSqlFactory : IDatabaseFactory
    {
        public IDbConnection Db { get; set; }
        public Dialect Dialect => Dialect.PostgreSQL;

        public QueryBuilder QueryBuilder { get; }

        private readonly string _connectionString;

        public IDbConnection GetConnection()
        {
            Db = new NpgsqlConnection(_connectionString);
            return Db;
        }

        public NpgSqlFactory(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            //IConfiguration configuration
            // connectionString = configuration.GetValue<string>("DBInfo:ConnectionString");
            _connectionString = configuration.GetConnectionString("DefaultConnection");// ConfigurationManager.ConnectionStrings[connectionStringName].ToString();
            Db = new NpgsqlConnection(_connectionString);
            QueryBuilder = new NpgSqlQueryBuilder(new NpgSQLTemplate());
            var hostingenv = serviceProvider.GetService<IHostingEnvironment>();
            DbLogger = new DbLogger(hostingenv, new DefaultDbLoggerSetting());
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
