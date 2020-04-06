using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.OTP.Model;

namespace Kachuwa.OTP.Services
{
    public interface IOTPService
    {
        CrudService<OTPSetting> SettingService { get; set; }
        CrudService<UserSecretKey> UserSecretService { get; set; }
        CrudService<UserOTP> OTPLogService { get; set; }
        Task<string> Generate(long userId);
        Task<bool> Verify(string otpCode, long userId);

    }
}