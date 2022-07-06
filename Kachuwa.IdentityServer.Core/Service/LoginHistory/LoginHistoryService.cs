using System.Data;
using System.Data.Common;
using Dapper;
using Kachuwa.Data;

namespace Kachuwa.IdentityServer.Service
{
    public class LoginHistoryService:ILoginHistoryService {
        public CrudService<UserLoginHistory> HistoryService { get; set; }=new CrudService<UserLoginHistory>();
        public Task<UserLoginHistory> GetLastLoginInfoAsync(long userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RemoveDeviceAsync(string deviceIdentifier, long userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CheckLoginDeviceAsync(string deviceIdentifier, long userId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<int> GetUserLoggedInDevicesNumber(long userId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
             
                return await
                    db.ExecuteScalarAsync<int>("select count(distinct(Browser)) from UserLoginHistory Where UserId=@UserId", new { UserId = userId }
                        );

            }
        }
    }
}