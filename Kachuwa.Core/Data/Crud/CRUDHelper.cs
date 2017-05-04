using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Log;

namespace Kachuwa.Data.Crud
{
    public static class CRUDHelper
    {

        public static IDatabaseFactory GetFactory(this IDbConnection connection)
        {
            return DbFactoryProvider.GetFactory();
        }

        public static T Get<T>(this IDbConnection connection, object id, IDbTransaction transaction = null,
            int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();

            var sql = factory.QueryBuilder.Get<T>(id);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.Query<T>(sql.QuerySql, sql.Parameters, transaction, true, commandTimeout).FirstOrDefault();
        }

        public static async Task<T> GetAsync<T>(this IDbConnection connection, object id,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Get<T>(id);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            var result = await connection.QueryAsync<T>(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
            return result.FirstOrDefault();
        }
        public static T Get<T>(this IDbConnection connection, string condition, object parameters,
          IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Get<T>(condition, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            var result = connection.Query<T>(sql.QuerySql, sql.Parameters);
            return result.FirstOrDefault();
        }
        public static async Task<T> GetAsync<T>(this IDbConnection connection, string condition, object parameters,
           IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Get<T>(condition, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            var result = await connection.QueryAsync<T>(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
            return result.FirstOrDefault();
        }

        public static IEnumerable<T> GetList<T>(this IDbConnection connection, object whereConditions,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.GetList<T>(whereConditions);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.Query<T>(sql.QuerySql, sql.Parameters, transaction, true, commandTimeout);

        }

        public static Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, object whereConditions,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.GetList<T>(whereConditions);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.QueryAsync<T>(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }

        public static IEnumerable<T> GetList<T>(this IDbConnection connection, string conditions,
            object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.GetList<T>(conditions, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.Query<T>(sql.QuerySql, sql.Parameters, transaction, true, commandTimeout);
        }

        public static Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, string conditions,
            object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.GetList<T>(conditions, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.QueryAsync<T>(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }
        public static Task<IEnumerable<T>> GetJoinedList<T>(this IDbConnection connection, string conditions,
          object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.GetJoinedList<T>(conditions, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.QueryAsync<T>(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }
        public static IEnumerable<T> GetList<T>(this IDbConnection connection)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.GetList<T>(new { });
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.Query<T>(sql.QuerySql);
        }

        public static Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection)
        {
            return connection.GetListAsync<T>(new { });
        }

        public static IEnumerable<T> GetListPaged<T>(this IDbConnection connection, int pageNumber, int rowsPerPage,
            int pageSize, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null,
            int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.GetPaginatedList<T>(pageNumber, rowsPerPage, conditions, orderby, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.Query<T>(sql.QuerySql, sql.Parameters, transaction, true, commandTimeout);
        }

        public static Task<IEnumerable<T>> GetListPagedAsync<T>(this IDbConnection connection, int pageNumber, int rowsPerPage,
            int pageSize, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null,
            int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.GetPaginatedList<T>(pageNumber, rowsPerPage, conditions, orderby, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.QueryAsync<T>(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }


        public static int? Insert(this IDbConnection connection, object entityToInsert,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return Insert<int?>(connection, entityToInsert, transaction, commandTimeout);
        }

        public static Task<int?> InsertAsync(this IDbConnection connection, object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return InsertAsync<int?>(connection, entityToInsert, transaction, commandTimeout);
        }

        public static TKey Insert<TKey>(this IDbConnection connection, object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Insert<TKey>(entityToInsert);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            var result = connection.Query(sql.QuerySql, sql.Parameters, transaction, true, commandTimeout);
            if (sql.IsKeyGuidType || sql.KeyHasPredefinedValue)
            {
                return (TKey)sql.Id;
            }
            return (TKey)result.First().id;
        }

        public static async Task<TKey> InsertAsync<TKey>(this IDbConnection connection, object entityToInsert,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Insert<TKey>(entityToInsert);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            var result = await connection.QueryAsync(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
            if (sql.IsKeyGuidType || sql.KeyHasPredefinedValue)
            {
                return (TKey)sql.Id;
            }
            return (TKey)result.First().id;
        }


        public static int Update(this IDbConnection connection, object entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Update(entityToUpdate);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.Execute(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
        }

        public static Task<int> UpdateAsync(this IDbConnection connection, object entityToUpdate,
            IDbTransaction transaction = null, int? commandTimeout = null,
            System.Threading.CancellationToken? token = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Update(entityToUpdate);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            System.Threading.CancellationToken cancelToken = token ?? default(System.Threading.CancellationToken);
            return connection.ExecuteAsync(new CommandDefinition(sql.QuerySql, sql.Parameters, transaction, commandTimeout, cancellationToken: cancelToken));

            //return connection.Execute(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }
        //new added
        public static Task<int> UpdateAsync(this IDbConnection connection, object entityToUpdate,
          string condition, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null,
           System.Threading.CancellationToken? token = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Update(entityToUpdate, condition, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            System.Threading.CancellationToken cancelToken = token ?? default(System.Threading.CancellationToken);
            return connection.ExecuteAsync(new CommandDefinition(sql.QuerySql, sql.Parameters, transaction, commandTimeout, cancellationToken: cancelToken));

            //return connection.Execute(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }

        public static int Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Delete<T>(entityToDelete);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.Execute(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
        }

        public static Task<int> DeleteAsync<T>(this IDbConnection connection, T entityToDelete,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Delete<T>(entityToDelete);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.ExecuteAsync(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }
        public static Task<int> DeleteAsync<T>(this IDbConnection connection, string condition, object parameters = null,
           IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Delete<T>(condition, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.ExecuteAsync(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }

        public static int Delete<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Delete<T>(id);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.Execute(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
        }

        public static Task<int> DeleteAsync<T>(this IDbConnection connection, object id,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Delete<T>(id);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.ExecuteAsync(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }
        public static int Delete<T>(this IDbConnection connection, int[] ids,
           IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Delete<T>(ids);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.Execute(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }

        public static Task<int> DeleteAsync<T>(this IDbConnection connection, int[] ids,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.Delete<T>(ids);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.ExecuteAsync(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }
        public static int DeleteList<T>(this IDbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.DeleteList<T>(whereConditions);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.Execute(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
        }
        public static int DeleteList<T>(this IDbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.DeleteList<T>(conditions, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.Execute(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
        }

        public static Task<int> DeleteListAsync<T>(this IDbConnection connection, object whereConditions,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.DeleteList<T>(whereConditions);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.ExecuteAsync(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
        }

        public static Task<int> DeleteListAsync<T>(this IDbConnection connection, string conditions,
            object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.DeleteList<T>(conditions, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.ExecuteAsync(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }



        public static int RecordCount<T>(this IDbConnection connection, string conditions = "", object parameters = null,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.RecordCount<T>(conditions, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.ExecuteScalar<int>(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
        }
        public static int RecordCount<T>(this IDbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.RecordCount<T>(whereConditions);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.ExecuteScalar<int>(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
        }

        public static Task<int> RecordCountAsync<T>(this IDbConnection connection, string conditions = "",
            object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.RecordCount<T>(conditions, parameters);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.ExecuteScalarAsync<int>(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }

        public static Task<int> RecordCountAsync<T>(this IDbConnection connection, object whereConditions,
            IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var sql = factory.QueryBuilder.RecordCount<T>(whereConditions);
            factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
            return connection.ExecuteScalarAsync<int>(sql.QuerySql, sql.Parameters, transaction, commandTimeout);

        }
        public static async Task<dynamic> GetDependents<T>(this IDbConnection connection,
           IDbTransaction transaction = null, int? commandTimeout = null)
        {
            IDatabaseFactory factory = connection.GetFactory();
            var datas = new List<object>();
            foreach (var sql in factory.QueryBuilder.GetDependents<T>())
            {
                factory.DbLogger.Log(LogType.Trace, () => $"starting--{sql.QuerySql} --datetime-->{DateTime.Now:yyyy_MM_d_hh_mm}");
                var data = await connection.QueryAsync(sql.QuerySql, sql.Parameters, transaction, commandTimeout);
                datas.Add(data);
            }

            return datas;
        }
    }
}