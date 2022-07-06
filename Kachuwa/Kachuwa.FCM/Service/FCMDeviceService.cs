using Dapper;
using Kachuwa.Data;
using Kachuwa.Data.Extension;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Kachuwa.FCM
{
    public class FCMDeviceService: IFCMDeviceService
    {

        public CrudService<UserFCMDevice> FCMDeviceCrudService { get; set; } = new CrudService<UserFCMDevice>();

        public async Task AddUserFcmDevice(UserFCMDevice userFCMDevice)
        {
           
            if (string.IsNullOrEmpty(userFCMDevice.DeviceId))
            {           
                throw new Exception("device id not found.");
            }
            else
            {
             
                var duplicate = 0;
                duplicate = await FCMDeviceCrudService.RecordCountAsync("where DeviceId=@token and UserId=@userId", new { token = userFCMDevice.DeviceId.Trim(),userId= userFCMDevice.UserId });
                if (duplicate>0)
                {
                    throw new Exception("device id is duplicate.");
                   
                }

                userFCMDevice.AutoFill();
                await FCMDeviceCrudService.InsertAsync(userFCMDevice);
            }
            
        } 

        public async Task<List<string>>GetFcmTokenByUserId(long userId)
        {
            var fmcTokens = await FCMDeviceCrudService.GetListAsync(new{UserId=userId});
            var ss= fmcTokens.Select(x=>x.DeviceId).ToList();
            return ss;
        }


        public async Task UpdateFCMGroupbyUserId(long userId, string groupName)
        {
            
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                
                 await db.ExecuteAsync("update  UserFCMDevice set groupName=@GroupName where userId=@UserId",
                        new {UserId= userId ,GroupName= groupName });
            }

        }
        
        public async Task<IEnumerable<string>> GetFcmTokensByUserIds(string userIds)
        {

            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();

               var data= await db.QueryAsync<string>(@"select deviceid from UserFCMDevice
                                                      where userid in ( select items from dbo.udf_Split('@userIds',',') )",
                                                      new { userIds });
                return data;
            }
        }

    }
}