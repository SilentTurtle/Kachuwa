using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Data.Crud;
using Kachuwa.Log;
using MySql.Data.MySqlClient;
using Npgsql;

namespace Kachuwa.Web.Module
{
    public class SQLScriptRunner : IScriptRunner
    {
        private readonly ILogger _logger;

        public SQLScriptRunner(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Run script for mssql server
        /// </summary>
        /// <param name="scripts"></param>
        /// <returns></returns>
        public async Task<bool> Run(string scripts)
        {
            try
            {
                bool isRun = false;
                var sqlqueries = scripts.Split(new[] { " GO " }, StringSplitOptions.RemoveEmptyEntries);

                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (SqlConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            var cmd = new SqlCommand("query", db, tran);
                            foreach (var query in sqlqueries)
                            {
                                cmd.CommandText = query;
                                await cmd.ExecuteNonQueryAsync();
                            }
                            tran.Commit();
                            isRun = true;
                        }
                        catch (Exception e)
                        {
                            _logger.Log(LogType.Error, () => e.Message, e);
                            isRun = false;
                            tran.Rollback();
                        }
                    }


                    return isRun;
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialect"></param>
        /// <param name="connectionString"></param>
        /// <param name="scripts"></param>
        /// <returns></returns>
        public async Task<bool> Run(Dialect dialect, string connectionString, string scripts)
        {
            try
            {

                switch (dialect)
                {
                    case Dialect.SQLServer:
                        return await RunMsSql(connectionString, scripts);

                    case Dialect.MySQL:
                        return await RunMySql(connectionString, scripts);

                    case Dialect.PostgreSQL:
                        return await RunPostgres(connectionString, scripts);
                }

                return false;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private async Task<bool> RunMsSql(string connectionString, string scripts)
        {
            var sqlqueries = scripts.Split(new[] { " GO " }, StringSplitOptions.RemoveEmptyEntries);

            var dbFactory = new SqlConnection(connectionString);
            using (var db = dbFactory)
            {
                await db.OpenAsync();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        var cmd = new SqlCommand("query", db, tran);
                        foreach (var query in sqlqueries)
                        {
                            cmd.CommandText = query;
                            await cmd.ExecuteNonQueryAsync();
                        }
                        tran.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.Log(LogType.Error, () => e.Message, e);

                        tran.Rollback();
                    }
                }
            }
            return false;

        }

        private async Task<bool> RunPostgres(string connectionString, string scripts)
        {
            var sqlqueries = scripts.Split(new[] { " GO " }, StringSplitOptions.RemoveEmptyEntries);

            var dbFactory = new NpgsqlConnection(connectionString);
            using (var db = dbFactory)
            {
                await db.OpenAsync();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        var cmd = new NpgsqlCommand("", db, tran);
                        foreach (var query in sqlqueries)
                        {
                            cmd.CommandText = query;
                            await cmd.ExecuteNonQueryAsync();
                        }
                        tran.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.Log(LogType.Error, () => e.Message, e);

                        tran.Rollback();
                    }
                }
            }
            return false;

        }

        private async Task<bool> RunMySql(string connectionString, string scripts)
        {
            var sqlqueries = scripts.Split(new[] { " GO " }, StringSplitOptions.RemoveEmptyEntries);

            var dbFactory = new MySqlConnection(connectionString);
            using (var db = dbFactory)
            {
                await db.OpenAsync();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        var cmd = new MySqlCommand("", db, tran);
                        foreach (var query in sqlqueries)
                        {
                            cmd.CommandText = query;
                            await cmd.ExecuteNonQueryAsync();
                        }
                        tran.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.Log(LogType.Error, () => e.Message, e);

                        tran.Rollback();
                    }
                }
            }
            return false;

        }
        /// <summary>
        /// Check if connection is valid
        /// </summary>
        /// <param name="dialect">Database Server Dialect</param>
        /// <param name="connectionString">Connection String</param>
        /// <returns>True : if valid connection, False : if not</returns>
        public async Task<bool> CheckConnection(Dialect dialect, string connectionString)
        {
            try
            {
                switch (dialect)
                {
                    case Dialect.SQLServer:
                        return CheckMsSql(connectionString);

                    case Dialect.MySQL:
                        return CheckMySql(connectionString);

                    case Dialect.PostgreSQL:
                        return CheckPostgres(connectionString);
                }

                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Check MS-SQL Server Connection
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <returns>True : if Valid, False : if Invalid</returns>
        private bool CheckMsSql(string connectionString)
        {
            try
            {
                var dbFactory = new SqlConnection(connectionString);
                try
                {
                    dbFactory.Open();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    dbFactory.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Check MySQL Server Connection
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <returns>True : if Valid, False : if Invalid</returns>
        private bool CheckMySql(string connectionString)
        {
            try
            {
                var dbFactory = new MySqlConnection(connectionString);
                try
                {
                    dbFactory.Open();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    dbFactory.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Check Postgres Server Connection
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <returns>True : if Valid, False : if Invalid</returns>
        private bool CheckPostgres(string connectionString)
        {
            try
            {
                var dbFactory = new NpgsqlConnection(connectionString);
                try
                {
                    dbFactory.Open();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    dbFactory.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}