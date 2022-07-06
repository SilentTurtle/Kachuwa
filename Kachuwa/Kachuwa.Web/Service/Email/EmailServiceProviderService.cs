using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using Kachuwa.Data.Extension;
using Kachuwa.Plugin;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Services
{
    public interface IEmailLogService
    {
        CrudService<EmailLog> LogCrudService { get; set; }
    }
    public class EmailLogService : IEmailLogService
    {
        public CrudService<EmailLog> LogCrudService { get; set; } = new CrudService<EmailLog>();
    }
    public class EmailServiceProviderService : IEmailServiceProviderService
    {
        private readonly IEnumerable<IEmailSender> _emailSenders;
        private readonly IEnumerable<IPlugin> _plugins;

        public EmailServiceProviderService(IPluginProvider pluginProvider,IEnumerable<IEmailSender> emailSenders)
        {
            _emailSenders = emailSenders;
            _plugins = pluginProvider.GetPlugins(PluginType.EmailService);
        }

        public async Task<IEmailSender> GetDefaultEmailSender()
        {
            var defaultProvider = await GetDefaultProviderAsync();
            if (defaultProvider == null)
            {
                return _emailSenders.First(o => o.Name.Equals(EmailProviderConstant.Default));
            }

           var defaultPlugin= _plugins.SingleOrDefault(plugin => plugin.Configuration.SystemName.ToLower() == defaultProvider.Name.ToLower());
            var emailSender= defaultPlugin as IEmailSender ;
            return emailSender;
        }
        public CrudService<EmailServiceProvider> ProviderCrudService { get; set; } = new CrudService<EmailServiceProvider>();
      
        public CrudService<EmailServiceProviderSetting> SettingCrudService { get; set; } = new CrudService<EmailServiceProviderSetting>();
        public async Task<bool> SetDefaultProviderAsync(int id)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync(
                    "Update EmailServiceProvider set isdefault=@isDefault;Update EmailServiceProvider set isDefault=@Active where EmailServiceProviderId=@Id",
                    new { isDefault = false, Active = true, Id = id });
                return true;
            }
        }
        public async Task<EmailServiceProvider> GetDefaultProviderAsync()
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
               return await db.QueryFirstOrDefaultAsync<EmailServiceProvider>(
                    "Select * from  EmailServiceProvider  Where IsDefault=@IsDefault and IsActive=@IsActive",
                    new { IsDefault = true, IsActive = true});
              
            }
        }

        public async Task<bool> UpdateSettings(List<EmailServiceProviderSetting> settings)
        {
            foreach (var setting in settings)
            {
                setting.AutoFill();
                await SettingCrudService.UpdateAsync(setting);
            }

            return true;
        }

        public async Task<bool> UpdateStatus(EmailServiceProvider model)
        {
            if (model.IsDefault)
            {
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                     await db.ExecuteAsync(
                        "update EmailServiceProvider set isDefault=@OldValue;" +
                        "update EmailServiceProvider set isDefault=@IsDefault ,IsActive=@IsActive  Where EmailServiceProviderId=@EmailServiceProviderId ",
                        new { IsDefault = true, IsActive = true, OldValue=false, EmailServiceProviderId=model.EmailServiceProviderId });
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
                        "update EmailServiceProvider set isDefault=@IsDefault ,IsActive=@IsActive  Where EmailServiceProviderId=@EmailServiceProviderId ",
                        new { IsDefault = model.IsDefault, IsActive = model.IsActive,EmailServiceProviderId = model.EmailServiceProviderId });
                    return true;
                }
            }
        }

        public async Task<IEnumerable<EmailServiceProviderSetting>> GetSettings(int emailServiceProviderId)
        {
            return await SettingCrudService.GetListAsync(@"Where EmailServiceProviderId=@emailServiceProviderId", new { emailServiceProviderId });
        }
        public async Task<IEnumerable<EmailServiceProviderSetting>> GetSettings(string name)
        {
            var emailServiceProvider = ProviderCrudService.Get("Where Name=@Name", new {Name = name});
            return await SettingCrudService.GetListAsync(@"Where EmailServiceProviderId=@EmailServiceProviderId", new { emailServiceProvider.EmailServiceProviderId });
        }
        public T GetSettings<T>(int emailServiceProviderId) where T : class
        {

            try
            {

                // var dbfactory = DbFactoryProvider.GetFactory();

                IEnumerable<EmailServiceProviderSetting> settings = GetSettings(emailServiceProviderId).Result;


                var settingObj = Activator.CreateInstance<T>();
                var settingObjType = settingObj.GetType();
                PropertyInfo[] pi = settingObjType.GetProperties();
                foreach (var setting in settings)
                {
                    var prop = pi.SingleOrDefault(z => z.Name == setting.ProviderKey);
                    if (prop != null)
                    {
                        Type tPropertyType = settingObjType.GetProperty(prop.Name).PropertyType;
                        // Fix nullables...
                        Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
                        object newValue = Convert.ChangeType(setting.ProviderValue, newT);
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
        public T GetSettings<T>(string name) where T : class
        {

            try
            {

                // var dbfactory = DbFactoryProvider.GetFactory();

                IEnumerable<EmailServiceProviderSetting> settings = GetSettings(name).Result;


                var settingObj = Activator.CreateInstance<T>();
                var settingObjType = settingObj.GetType();
                PropertyInfo[] pi = settingObjType.GetProperties();
                foreach (var setting in settings)
                {
                    var prop = pi.SingleOrDefault(z => z.Name == setting.ProviderKey);
                    if (prop != null)
                    {
                        Type tPropertyType = settingObjType.GetProperty(prop.Name).PropertyType;
                        // Fix nullables...
                        Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
                        object newValue = Convert.ChangeType(setting.ProviderValue, newT);
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
        public async Task<bool> SaveSetting<T>(T setting, int emailServiceProviderId)
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
                            var esetting = new EmailServiceProviderSetting()
                            {
                                EmailServiceProviderSettingId = 0,
                                EmailServiceProviderId = emailServiceProviderId,
                                ProviderKey = prop.Name,
                                ProviderValue = prop.GetValue(setting, null).ToString()
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

        public async Task<int> InsertOrSave(EmailServiceProvider model)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            model.AutoFill();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {

                await db.OpenAsync();
                return await db.ExecuteScalarAsync<int>("if exists( select 1 from EmailServiceProvider where Name=@Name)" +
                                                        " BEGIN update EmailServiceProvider set IsActive=@IsActive where Name=@Name; " +
                                                        "select EmailServiceProviderId from EmailServiceProvider where Name=@Name END  " +
                                                        " ELSE BEGIN  insert into EmailServiceProvider(Name,Image,Description,IsActive,AddedOn,AddedBy) " +
                                                        " values (@Name,@Image,@Description,@IsActive,@AddedOn,@AddedBy); select scope_identity(); END",
                    new { model.Name, model.Description, model.Image, IsActive = true, AddedOn = DateTime.UtcNow, AddedBy = model.AddedBy });

            }
        }
        public async Task<bool> DeleteEmailService(string name)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("Delete es from EmailServiceProvider as e inner join EmailServiceProviderSetting as es on e.EmailServiceProviderId = es.EmailServiceProviderId " +
                                      "where Name = @Name ;" +
                                      " Delete From EmailServiceProvider Where Name=@Name;",
                    new {isActive = false, Name = name});
                return true;
            }
        }


    }
}
