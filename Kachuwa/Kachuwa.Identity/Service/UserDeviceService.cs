using Dapper;
using Kachuwa.Data;
using Kachuwa.Data.Extension;
using Kachuwa.Identity.Models;
using System.Data.Common;
using System.Threading.Tasks;

namespace Kachuwa.Identity.Service
{
    public class UserDeviceService : IUserDeviceService
    {
        public CrudService<UserDevice> DeviceService { get; set; } = new CrudService<UserDevice>();


        public async Task<UserDeviceStatus> GetUserDeviceStatus(long userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SendDeviceVerification(long userId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<DeviceVerificationStatus> VerifyDevice(long userId, long userDeviceId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();

                await
                    db.ExecuteAsync("Update UserDevice Set IsVerified=1 where UserId=@UserId and UserDeviceId=@UserDeviceId", new
                    {
                        UserDeviceId = userDeviceId,
                        UserId = userId
                    }
                    );
                return new DeviceVerificationStatus {IsVerified = true, Message = "Verified successfully!."};


            }
        }



        public async Task<UserDeviceStatus> CheckAndSaveDevice(UserDevice userDevice)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();

                int exist = await
                    db.ExecuteScalarAsync<int>(
                        " if(@IsWeb=1) begin select 1 from UserDevice Where IsDeleted=0 and IsActive=1 and UserId=@UserId and DeviceId=@DeviceId and  IsWeb=1 " +
                        " end else begin select 1 from UserDevice Where IsDeleted=0 and IsActive=1 and UserId=@UserId and DeviceId=@DeviceId and IsMobile=1 end",
                        new
                        {
                            userDevice.Browser,
                            userDevice.BrowserVersion,
                            userDevice.OS,
                            userDevice.Version,
                            userDevice.DeviceId,
                            userDevice.IsWeb,
                            userDevice.IsMobile,
                            userDevice.UserId
                        }
                    );

                if (exist == 0)
                {
                    userDevice.AutoFill();
                    await DeviceService.InsertAsync<long>(userDevice);
                }

                return await db.QueryFirstOrDefaultAsync<UserDeviceStatus>(
                       " declare @md bit =0,@IsThisUnverifiedLogin bit=1,@totaldevice int =0;" +
                       " Select @totaldevice=count(1) from UserDevice Where IsDeleted = 0 and IsVerified=1 and UserId=@UserId;" +
                       " Select @md=IsMobile from UserDevice Where IsDeleted = 0 and IsMobile=1 and IsVerified=1 and UserId=@UserId;" +
                       " if(@IsWeb=1) begin select @IsThisUnverifiedLogin=~IsVerified from UserDevice Where IsDeleted=0 and IsActive=1 and UserId=@UserId and DeviceId=@DeviceId and  IsWeb=1 " +
                       " end else begin select  @IsThisUnverifiedLogin=~IsVerified from UserDevice Where IsDeleted=0 and IsActive=1 and UserId=@UserId and DeviceId=@DeviceId and IsMobile=1 end" +
                       " declare @MobileDeviceCount int,@BrowserCount int " +
                       " SElect @MobileDeviceCount = count(1) from UserDevice Where IsMobile = 1 and IsDeleted = 0 and IsActive = 1 and IsVerified = 1 and UserId = @UserId " +
                       " SElect @BrowserCount = count(1) from UserDevice Where IsWeb = 1 and IsDeleted = 0 and IsActive = 1 and IsVerified = 1 and UserId = @UserId  " +
                       " SElect @MobileDeviceCount MobileCount,@BrowserCount BrowserCount, @md MobileDevice ,@totaldevice VerifiedDeviceCount, @IsThisUnverifiedLogin IsThisUnverifiedLogin ",
                       new
                       {

                           userDevice.Browser,
                           userDevice.BrowserVersion,
                           userDevice.OS,
                           userDevice.Version,
                           userDevice.DeviceId,
                           userDevice.IsWeb,
                           userDevice.IsMobile,
                           userDevice.UserId
                       }
                   );
            }

        }

        public async Task<bool> RemoveDeviceAsync(long userDeviceId, long userId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();

                await
                    db.ExecuteAsync("Update UserDevice Set IsDeleted=1,IsActive=0 where UserId=@UserId and UserDeviceId=@UserDeviceId", new
                        {
                            UserDeviceId = userDeviceId,
                            UserId = userId
                        }
                    );
                return true;


            }
        }
    }
}