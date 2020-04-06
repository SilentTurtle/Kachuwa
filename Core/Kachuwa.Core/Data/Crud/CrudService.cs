using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data.Crud;

namespace Kachuwa.Data
{
    public class CrudService<T>
    {
        public CrudService(IDatabaseFactory dbFactory)
        {
            DbFactory = dbFactory;
        }
        public CrudService()
        {
            DbFactory = DbFactoryProvider.GetFactory();
        }

        private IDatabaseFactory DbFactory { get; }
        //public Task<int> SaveAsync(T t)
        //{
        //    using (var db = DbFactory.Open())
        //    {
        //        var x = await db.GetListAsync<T>();

        //    }
        //}

        public virtual T Query(string sql, object param = null)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                db.Open();
                var result = db.Query<T>(sql, param);
                db.Close();
                return result.FirstOrDefault();
            }
        }
        public virtual async Task<T> QueryAsync(string sql, object param = null)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result = await db.QueryAsync<T>(sql, param);
                db.Close();
                return result.FirstOrDefault();

            }
        }
        public virtual IEnumerable<T> QueryList(string sql, object param = null)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                db.Open();
                return db.Query<T>(sql, param);
            }
        }
        public virtual async Task<IEnumerable<T>> QueryListAsync(string sql, object param = null)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<T>(sql, param);

            }
        }
        public virtual T Get(object id)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                db.Open();
                return db.Get<T>(id);

            }
        }
        public virtual async Task<T> GetAsync(object id)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.GetAsync<T>(id);

            }
        }
        public virtual T Get(string condition, object parameters = null)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                db.Open();
                return db.Get<T>(condition, parameters);

            }
        }
        public virtual async Task<T> GetAsync(string condition, object parameters = null)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.GetAsync<T>(condition, parameters);

            }
        }

        public virtual IEnumerable<T> GetList(object whereConditions)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    db.Open();
                    return db.GetList<T>(whereConditions);

                }
            }
        }
        public virtual async Task<IEnumerable<T>> GetListAsync(object whereConditions)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.GetListAsync<T>(whereConditions);

                }
            }
        }

        public virtual IEnumerable<T> GetList(string conditions,
           object parameters)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    db.Open();
                    return db.GetList<T>(conditions, parameters);

                }
            }
        }
        public virtual async Task<IEnumerable<T>> GetListAsync(string conditions,
            object parameters)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.GetListAsync<T>(conditions, parameters);

                }
            }
        }
        public virtual async Task<IEnumerable<T>> GetJoinedList(string conditions,
           object parameters = null)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.GetJoinedList<T>(conditions, parameters);

                }
            }
        }

        public virtual IEnumerable<T> GetList()
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    db.Open();
                    return db.GetList<T>();

                }
            }
        }
        public virtual async Task<IEnumerable<T>> GetListAsync()
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.GetListAsync<T>();

                }
            }
        }

        public virtual IEnumerable<T> GetListPaged(int pageNumber, int rowsPerPage,
           int pageSize, string conditions, string orderby, object parameters = null)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    db.Open();
                    return db.GetListPaged<T>(pageNumber, rowsPerPage, pageSize, conditions, orderby, parameters);

                }
            }
        }
        public virtual async Task<IEnumerable<T>> GetListPagedAsync(int pageNumber, int rowsPerPage,
            int pageSize, string conditions, string orderby, object parameters = null)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.GetListPagedAsync<T>(pageNumber, rowsPerPage, pageSize, conditions, orderby, parameters);

                }
            }
        }

        public virtual int? Insert(object entityToInsert)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    db.Open();
                    return db.Insert<int?>(entityToInsert);

                }
            }
        }
        public virtual async Task<int?> InsertAsync(object entityToInsert)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.InsertAsync<int?>(entityToInsert);

                }
            }
        }

        public virtual TKey Insert<TKey>(object entityToInsert)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    db.Open();
                    return db.Insert<TKey>(entityToInsert);

                }
            }
        }
        public virtual async Task<TKey> InsertAsync<TKey>(object entityToInsert)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.InsertAsync<TKey>(entityToInsert);

                }
            }
        }
        public virtual async Task<TKey> InsertAsync<TKey>(IDbConnection db, object entityToInsert, IDbTransaction transaction, int? commandTimeout)
        {
            {

                return await db.InsertAsync<TKey>(entityToInsert, transaction, commandTimeout);

            }
        }

        public virtual int Update(object entityToUpdate)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    db.Open();
                    return db.Update(entityToUpdate);

                }
            }
        }
        public virtual int Update(IDbConnection db, object entityToUpdate, IDbTransaction transaction, int? timeout)
        {
            {

                return db.Update(entityToUpdate, transaction, timeout);


            }
        }
        public virtual async Task<int> UpdateAsync(IDbConnection db, object entityToUpdate, IDbTransaction transaction, int? commandTimeout)
        {
            {

                return await db.UpdateAsync(entityToUpdate, transaction, commandTimeout);


            }
        }
        public async Task UpdateAsync(object entityToUpdate, string condition, object paramters = null)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.UpdateAsync(entityToUpdate, condition, paramters);

            }
        }
        public virtual async Task<bool> UpdateAsync(object entityToUpdate)
        {

            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.UpdateAsync(entityToUpdate);
                return await Task.FromResult(true);
            }

        }
        public virtual int Delete(T entityToDelete)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    db.Open();
                    return db.Delete(entityToDelete);

                }
            }
        }
        public virtual async Task<int> DeleteAsync(T entityToDelete)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.DeleteAsync(entityToDelete);

                }
            }
        }
        public virtual int Delete(object id)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    db.Open();
                    return db.Delete<T>(id);

                }
            }
        }
        public virtual int Delete(IDbConnection db, object id, IDbTransaction transaction, int? timeout)
        {

            return db.Delete(id, transaction, timeout);
        }
        public virtual async Task<int> DeleteAsync(object id)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.DeleteAsync<T>(id);

            }

        }
        public virtual async Task<int> DeleteAsync(IDbConnection db, object id, IDbTransaction transaction, int? timeout)
        {
            return await db.DeleteAsync(id, transaction, timeout);
        }
        public virtual int Delete(int[] ids)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    db.Open();
                    return db.Delete<T>(ids);

                }
            }
        }
        public virtual async Task<int> DeleteAsync(int[] ids)
        {

            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.DeleteAsync<T>(ids);

            }

        }
        public virtual async Task DeleteAsync(string condition, object parameters = null)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.DeleteAsync<T>(condition, parameters);
            }
        }
        public virtual async Task DeleteAsync(IDbConnection db, IDbTransaction transaction, string condition, object parameters, int? timeout)
        {
            await db.DeleteAsync<T>(condition, parameters, transaction, timeout);
        }
        // int DeleteList(object whereConditions);
        public virtual async Task<int> DeleteListAsync(object whereConditions)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.DeleteListAsync<T>(whereConditions);

                }
            }
        }
        public virtual async Task<int> DeleteListAsync(string conditions,
            object parameters = null)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.DeleteListAsync<T>(conditions, parameters);

                }
            }
        }

        public virtual async Task<int> RecordCountAsync(string conditions = "",
            object parameters = null)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.RecordCountAsync<T>(conditions, parameters);

                }
            }
        }
        public virtual async Task<int> RecordCountAsync(object whereConditions)
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.RecordCountAsync<T>(whereConditions);

                }
            }
        }

        public virtual async Task<object> GetDependents()
        {
            {
                using (var db = (DbConnection)DbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.GetDependents<T>();
                }
            }
        }



        public virtual async Task UpdateAsDeleted(object id)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.UpdateAsDeleted<T>(id);
            }
        }

        public virtual async Task<int> UpdateStatusAsync(object id, object status)
        {
            using (var db = (DbConnection)DbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.UpdateStatus<T>(id, status);
            }
        }
    }
}