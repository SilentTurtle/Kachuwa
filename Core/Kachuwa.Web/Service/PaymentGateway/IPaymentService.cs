using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Web.Model;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web.Service
{
    public interface IPaymentService
    {


        CrudService<PaymentGateway> GatewayCrudHellper { get; set; }
        Task<int> InsertOrSave(PaymentGateway model);
        CrudService<PaymentGatewaySetting> GatewaySettingCrudHellper { get; set; }

        Task<IEnumerable<PaymentGatewaySetting>> GetSettings(string sysName);
        //Task<T> GetSettings<T>(string sysName) where T : class;
        T GetSettings<T>(string sysName) where T : class;

        Task<bool> CheckActive(string gatewayName);
        Task UpdateGatewaySetting(List<PaymentGatewaySetting> settings);
        Task<IEnumerable<PaymentGateway>> GetAllPaymentGateways();
        Task<bool> SaveSetting<T>(T setting, int paymentGatewayId, string systemName);
        Task<bool> DeletePaymentGateway(string systemName);

        //implement pub sub to success error failed verified


        Task<PgStatus> UnzipAndInstall(IFormFile zipFile);
    }
}
