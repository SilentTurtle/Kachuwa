using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Log;

namespace Kachuwa.Web.Module
{
    public class SQLScriptRunner: IScriptRunner
    {
        private readonly ILogger _logger;

        public SQLScriptRunner(ILogger logger)
        {
            _logger = logger;
        }
        public async Task<bool> Run(string scripts)
        {
            try
            {
                bool isRun = false;
                var sqlqueries = scripts.Split(new[] {" GO "}, StringSplitOptions.RemoveEmptyEntries);

                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (SqlConnection) dbFactory.GetConnection())
                {
                    db.Open();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            var cmd = new SqlCommand("query", db,tran);
                            foreach (var query in sqlqueries)
                            {
                                cmd.CommandText = query;
                                cmd.ExecuteNonQuery();
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
    }
}