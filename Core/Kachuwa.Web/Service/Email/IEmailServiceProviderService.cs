using Kachuwa.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kachuwa.Web.Services
{
    public interface IEmailServiceProviderService
    {

        CrudService<EmailServiceProvider> ProviderCrudService { get; set; }
        CrudService<EmailServiceProviderSetting> SettingCrudService { get; set; }
        Task<IEmailSender> GetDefaultEmailSender();
        Task<IEnumerable<EmailServiceProviderSetting>> GetSettings(int emailServiceProviderId);
        T GetSettings<T>(int emailServiceProviderId) where T : class;
        Task<bool> SaveSetting<T>(T setting, int emailServiceProviderId);
        Task<bool> SetDefaultProviderAsync(int id);
        Task<EmailServiceProvider> GetDefaultProviderAsync();
        Task<bool> UpdateSettings(List<EmailServiceProviderSetting> settings);
        Task<bool> UpdateStatus(EmailServiceProvider model);
    }
}