using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using Kachuwa.Identity.Extensions;
using Kachuwa.Log;
using Kachuwa.Web;
using MXTires.Microdata.Core.Actions.TransferActions;

namespace Kachuwa.RTC
{
    public class RTCPersistentConnectionManager : IRTCUserService
    {
        private readonly ILogger _logger;

        public RTCPersistentConnectionManager(ILogger logger)
        {
            _logger = logger;
        }
        public CrudService<RTCUser> CrudService { get; set; } = new CrudService<RTCUser>();


        public bool AddUser(RTCUser user)
        {
            try
            {
                CrudService.Insert(user);
                return true;

            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return false;
            }
        }

        public bool AddUser(string connectionId)
        {
            throw new NotImplementedException();
        }


        public bool RemoveUser(string connectionId)
        {
            try
            {
                CrudService.DeleteAsync("Where ConnectionId=@ConnectionId", new
                {
                    ConnectionId = connectionId
                });
                return true;
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return false;
            }
        }

        public async Task UpdateGroupName(string groupName, long identityUserId)
        {
            try
            {
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    await db.ExecuteAsync("update dbo.RTCUser set GroupName=@group where IdentityUserId=@IdentityUserId", new
                    {
                        group = groupName,
                        IdentityUserId = identityUserId
                    });
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
            }

        }

        public async Task UpdateGroupName(string groupName, string connectionId)
        {
            try
            {
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    await db.ExecuteAsync("update dbo.RTCUser set GroupName=@group where ConnectionId=@ConnectionId", new
                    {
                        group = groupName,
                        ConnectionId = connectionId
                    });
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                throw e;
            }
        }

        public async Task<IEnumerable<string>> GetUserConnectionIds(long identityUserId)
        {
            try
            {
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.QueryAsync<string>(
                        "select ConnectionId from dbo.RTCUser where IdentityUserId=@IdentityUserId", new
                        {

                            IdentityUserId = identityUserId
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e); throw e;
            }
        }

        public async Task<int> GetOnlineUserByHub(string hubName)
        {
            try
            {
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.ExecuteScalarAsync<int>(
                        "select count(1) as TotalOnlineUser from dbo.RTCUser where HubName=@HubName", new
                        {

                            HubName = hubName
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e); throw e;
            }
        }

        public async Task<int> GetOnlineUser()
        {
            try
            {
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.ExecuteScalarAsync<int>(
                        "select count(1) as TotalOnlineUser from dbo.RTCUser");
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e); throw e;
            }
        }

        public async Task<IEnumerable<string>> GetUserConnectionIdsByRoles(string role)
        {
            try
            {
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.QueryAsync<string>(
                        "select ConnectionId from dbo.RTCUser where where (select Data from dbo.Split(UserRoles,',') where Data=@Role)=@Role;", new
                        {
                            Role = role
                        });
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e); throw e;
            }
        }

        public async Task<RtcUserStatus> GetOnlineUserStatus()
        {
            try
            {
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    return await db.QueryFirstOrDefaultAsync<RtcUserStatus>(
                        "declare @totalUser int,@TotalGuestUser int ,@totalMobileUser int ,@totalWebUser int;" +
                      "delete  dbo.RTCUser where cast(addedon as date)<cast(getutcdate() as date);" +
                        "select @totalUser = count(distinct IdentityUserId)  from dbo.RTCUser  where cast(addedon as date)=cast(getutcdate() as date);" +
                      "select @TotalGuestUser = count(distinct SessionId) from dbo.RTCUser   where IdentityUserId=0 and cast(addedon as date)=cast(getutcdate() as date);" +
                      "select @totalMobileUser = count( distinct IdentityUserId)  from dbo.RTCUser  where   IdentityUserId !=0 and cast(addedon as date)=cast(getutcdate() as date) and IsFromMobile = 1;" +
                      "select @totalWebUser = count( distinct IdentityUserId)   from dbo.RTCUser    where  IdentityUserId !=0 and cast(addedon as date)=cast(getutcdate() as date) and IsFromWeb = 1;" +
                      "select @totalUser as TotalUser ,@TotalGuestUser as TotalGuestUser ,@totalMobileUser as TotalMobileUser ,@totalWebUser as TotalWebUser; ");
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e); throw e;
            }
        }

        public Task<RTCUser> GetCurrentUser(string connectionId)
        {
            throw new NotImplementedException();
        }

        Task<RTCUser> IRTCConnectionManager.UpdateGroupName(string groupName, long identityUserId)
        {
            throw new NotImplementedException();
        }

        Task<RTCUser> IRTCConnectionManager.UpdateGroupName(string groupName, string connectionId)
        {
            throw new NotImplementedException();
        }

        public Task<List<RTCUser>> GetUsersByGroup(string groupName)
        {
            throw new NotImplementedException();
        }

        public Task<RTCUser> GetUser(long identityUserid, string connectionId)
        {
            throw new NotImplementedException();
        }

        public Task<List<RTCUser>> GetUsersByHub(string hubName)
        {
            throw new NotImplementedException();
        }
    }
}
