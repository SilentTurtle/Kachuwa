using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using Kachuwa.Data.Extension;
using Kachuwa.Log;
using Kachuwa.Plugin;
using Kachuwa.Web.Model;
using Kachuwa.Web.Services;

namespace Kachuwa.Web.Service
{

    public interface ISMSLogService
    {
        CrudService<SMSLog> LogCrudService { get; set; }
    }

    public class SMSLogService : ISMSLogService
    {
        public CrudService<SMSLog> LogCrudService { get; set; } = new CrudService<SMSLog>();
    }
    public class SMSService : ISMSService
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IPlugin> _plugins;

        public SMSService(IPluginProvider pluginProvider ,ILogger logger)
        {
            _logger = logger;
            _plugins = pluginProvider.GetPlugins(PluginType.SmsService);
        }
        public CrudService<SMSGateway> GatewayCrudService { get; set; }=new CrudService<SMSGateway>();
       
        public CrudService<SMSGatewaySetting> SettingCrudService { get; set; }=new CrudService<SMSGatewaySetting>();
        public async Task<ISmsSender> GetDefaultSmsSender()
        {
            var defaultProvider = await GetDefaultProviderAsync();
            if (defaultProvider == null)
            {
                return new SmsSender(_logger);
            }

            var defaultPlugin = _plugins.SingleOrDefault(plugin => plugin.Configuration.SystemName.ToLower() == defaultProvider.Name.ToLower());
            var smsService = defaultPlugin as ISmsSender;
            return smsService;
        }

        public async Task<IEnumerable<SMSGatewaySetting>> GetSettings(int smsGatewayId)
        {
            return await SettingCrudService.GetListAsync(@"Where SmsGatewayId=@smsGatewayId", new { smsGatewayId });

        }

        public T GetSettings<T>(int smsGatewayId) where T : class
        {
            try
            {

                // var dbfactory = DbFactoryProvider.GetFactory();

                IEnumerable<SMSGatewaySetting> settings = GetSettings(smsGatewayId).Result;


                var settingObj = Activator.CreateInstance<T>();
                var settingObjType = settingObj.GetType();
                PropertyInfo[] pi = settingObjType.GetProperties();
                foreach (var setting in settings)
                {
                    var prop = pi.SingleOrDefault(z => z.Name == setting.GatewayKey);
                    if (prop != null)
                    {
                        Type tPropertyType = settingObjType.GetProperty(prop.Name).PropertyType;
                        // Fix nullables...
                        Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
                        object newValue = Convert.ChangeType(setting.GatewayValue, newT);
                        settingObj.GetType().GetProperty(prop.Name).SetValue(settingObj, newValue, null);
                    }
                }


                return settingObj as T;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> SaveSetting<T>(T setting, int smsGatewayId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var type = typeof(T);
                var props = type.GetProperties();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {

                        foreach (var prop in props)
                        {
                            var esetting = new SMSGatewaySetting()
                            {
                                SMSGatewaySettingId = 0,
                                SMSGatewayId = smsGatewayId,
                                GatewayKey = prop.Name,
                                GatewayValue = prop.GetValue(setting, null).ToString()
                            };

                            await SettingCrudService.InsertAsync(esetting);
                        }
                        tran.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public async Task<bool> SetDefaultProviderAsync(int id)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync(
                    "Update dbo.SmsGateway set isdefault=@isDefault;Update SmsGateway set isDefault=@Active where SmsGatewayId=@Id",
                    new { isDefault = false, Active = true, Id = id });
                return true;
            }
        }

        public async Task<SMSGateway> GetDefaultProviderAsync()
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryFirstOrDefaultAsync<SMSGateway>(
                    "Select * from  dbo.SmsGateway  Where IsDefault=@IsDefault and IsActive=@IsActive",
                    new { IsDefault = true, IsActive = true });

            }
        }

        public async Task<bool> UpdateStatus(SMSGateway model)
        {
            if (model.IsDefault)
            {
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    await db.ExecuteAsync(
                        "update dbo.SmsGateway set isDefault=@OldValue;" +
                        "update dbo.SmsGateway set isDefault=@IsDefault ,IsActive=@IsActive  Where SMSGatewayId=@SMSGatewayId ",
                        new { IsDefault = true, IsActive = true, OldValue = false, SMSGatewayId = model.SMSGatewayId });
                    return true;
                }
            }
            else
            {
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    await db.ExecuteAsync(
                        "update dbo.SmsGateway set isDefault=@IsDefault ,IsActive=@IsActive  Where SMSGatewayId=@SMSGatewayId ",
                        new { IsDefault = model.IsDefault, IsActive = model.IsActive, SMSGatewayId = model.SMSGatewayId });
                    return true;
                }
            }
        }

        public async Task<bool> UpdateSettings(List<SMSGatewaySetting> settings)
        {
            foreach (var setting in settings)
            {
                setting.AutoFill();
                await SettingCrudService.UpdateAsync(setting);
            }

            return true;
        }
    }
}