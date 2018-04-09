using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using MXTires.Microdata.Core.Actions.TransferActions;

namespace Kachuwa.RTC
{
    public  class RTCUserService: IRTCUserService
    {
        public CrudService<RTCUser> CrudService { get; set; }=new CrudService<RTCUser>();
        public async Task UpdateGroupName(string groupName, long identityUserId)
        {

            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db =(DbConnection) dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("update dbo.RTCUser set GroupName=@group where IdentityUserId=@IdentityUserId", new
                {
                    group= groupName ,
                    IdentityUserId= identityUserId
                });
            }
           
        }

        public async Task UpdateGroupName(string groupName, string connectionId)
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

        public async Task<IEnumerable<string>> GetUserConnectionIds(long identityUserId)
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

        public async Task<int> GetOnlineUserByHub(string hubName)
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

        public async Task<int> GetOnlineUser()
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.ExecuteScalarAsync<int>(
                    "select count(1) as TotalOnlineUser from dbo.RTCUser");
            }
        }
    }
}
