using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Web.Model;
using Kachuwa.Web.Service;

namespace Kachuwa.Web.Services
{
    public interface ISMSService
    {
       
       // CrudService<SMSLog> LogCrudService { get; set; }
        CrudService<SMSGateway> GatewayCrudService { get; set; }
        CrudService<SMSGatewaySetting> SettingCrudService { get; set; }
        Task<ISmsSender> GetDefaultSmsSender();
        Task<IEnumerable<SMSGatewaySetting>> GetSettings(int smsGatewayId);
        T GetSettings<T>(int smsGatewayId) where T : class;
        Task<bool> SaveSetting<T>(T setting, int smsGatewayId);
        Task<bool> SetDefaultProviderAsync(int id);
        Task<SMSGateway> GetDefaultProviderAsync();
        Task<bool> UpdateStatus(SMSGateway model);
        Task<bool> UpdateSettings(List<SMSGatewaySetting> settings);
    }
}