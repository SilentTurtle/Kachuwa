using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using Kachuwa.Data.Extension;
using Kachuwa.IO;
using Kachuwa.Log;
using Kachuwa.Storage;
using Kachuwa.Web.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IKeyGenerator _keyGenerator;
        private readonly IFileOptions _fileOptions;
        private readonly IStorageProvider _storageProvider;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        public CrudService<PaymentGateway> GatewayCrudHellper { get; set; } = new CrudService<PaymentGateway>();

        public PaymentService(IKeyGenerator keyGenerator, IFileOptions fileOptions, IStorageProvider storageProvider, ILogger logger, IHostingEnvironment hostingEnvironment)
        {
            _keyGenerator = keyGenerator;
            _fileOptions = fileOptions;
            _storageProvider = storageProvider;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<int> InsertOrSave(PaymentGateway model)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            model.AutoFill();
            using (var db = (SqlConnection)dbFactory.GetConnection())
            {

                await db.OpenAsync();
                return await db.ExecuteScalarAsync<int>("if exists( select 1 from dbo.paymentgateway where SystemName=@SystemName)" +
                    " BEGIN update dbo.paymentgateway set IsActive=@IsActive where SystemName=@SystemName; select paymentgatewayid from dbo.paymentgateway where SystemName=@SystemName END  " +
                    " ELSE BEGIN  insert into dbo.paymentgateway(PaymentGatewayName,Image,Description,SystemName,[Type],IsActive,AddedOn,AddedBy) " +
                    "values (@PaymentGatewayName,@Image,@Description,@SystemName,@Type,@IsActive,@AddedOn,@AddedBy); select scope_identity(); END",
                    new { model.PaymentGatewayName, model.Description, model.Image,model.Type, model.SystemName, IsActive = true, AddedOn = DateTime.UtcNow, AddedBy = model.AddedBy });

            }
        }

        public CrudService<PaymentGatewaySetting> GatewaySettingCrudHellper { get; set; } = new CrudService<PaymentGatewaySetting>();

        public async Task<IEnumerable<PaymentGatewaySetting>> GetSettings(string sysName)
        {
            return await GatewaySettingCrudHellper.GetListAsync(@"Where SystemName=@SysName", new { SysName = sysName });
        }

        public T GetSettings<T>(string sysName) where T : class
        {

            try
            {

                // var dbfactory = DbFactoryProvider.GetFactory();

                IEnumerable<PaymentGatewaySetting> settings = GetSettings(sysName).Result;


                var settingObj = Activator.CreateInstance<T>();
                var settingObjType = settingObj.GetType();
                PropertyInfo[] pi = settingObjType.GetProperties();
                foreach (var setting in settings)
                {
                    var prop = pi.SingleOrDefault(z => z.Name == setting.PaymentGatewayKey);
                    if (prop != null)
                    {
                        Type tPropertyType = settingObjType.GetProperty(prop.Name).PropertyType;
                        // Fix nullables...
                        Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
                        object newValue = Convert.ChangeType(setting.PaymentGatewayValue, newT);
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

        public async Task UpdateOrder(int orderId, string transactionId, string userId, decimal paidAmount)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (SqlConnection)dbFactory.GetConnection())
            {
                var param = new DynamicParameters();
                param.Add("@OrderId", orderId);
                param.Add("@TransactionId", transactionId);
                param.Add("@CustomerId", userId);
                param.Add("@PaidAmount", paidAmount);

                // param.Add("@CartItemId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                //p.Add("@c", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await db.OpenAsync();
                var result =
                    await
                        db.ExecuteAsync("usp_Order_Update", param,
                            commandType: CommandType.StoredProcedure);

            }
        }

        public async Task<bool> CheckActive(string gatewayName)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (SqlConnection)dbFactory.GetConnection())
            {
                var param = new DynamicParameters();

                param.Add("@GatewayName", gatewayName);
                param.Add("@IsActive", dbType: DbType.Boolean, direction: ParameterDirection.Output);
                //p.Add("@c", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await db.OpenAsync();
                var result =
                    await
                        db.ExecuteAsync("usp_PaymentGateway_Check", param,
                            commandType: CommandType.StoredProcedure);
                return param.Get<bool>("@IsActive");

            }
        }

        public async Task UpdatePaymentStatus(int orderId, int statusId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (SqlConnection)dbFactory.GetConnection())
            {
                var param = new DynamicParameters();

                param.Add("@OrderId", orderId);
                param.Add("@StatusId", statusId);

                await db.OpenAsync();
                var result =
                    await
                        db.ExecuteAsync("usp_Order_UpdatePaymentStatus", param,
                            commandType: CommandType.StoredProcedure);

            }
        }

        public async Task UpdateGatewaySetting(List<PaymentGatewaySetting> settings)
        {
            foreach (var setting in settings)
            {
                setting.AutoFill();
                await GatewaySettingCrudHellper.UpdateAsync(setting);
            }
        }

        public async Task<IEnumerable<PaymentGateway>> GetAllPaymentGateways()
        {
            return await GatewayCrudHellper.GetListAsync();
        }
       

        public async Task<bool> SaveSetting<T>(T setting, int paymentGatewayId, string systemName)
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
                            var pgsetting = new PaymentGatewaySetting()
                            {
                                PaymentGatewaySettingId = 0,
                                PaymentGatewayId = paymentGatewayId,
                                PaymentGatewayKey = prop.Name,
                                SystemName = systemName,
                                PaymentGatewayValue = prop.GetValue(setting, null).ToString()
                            };

                            await GatewaySettingCrudHellper.InsertAsync(pgsetting);
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

        public async Task<bool> DeletePaymentGateway(string systemName)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("Update PaymentGateway SET IsActive=@isActive Where SystemName=@systemName;" +
                     "Delete From PaymentGatewaySetting Where SystemName=@systemName", new { isActive = false, systemName });
                return true;
            }
        }

        public async Task<PgStatus> UnzipAndInstall(IFormFile zipFile)
        {
            try
            {
                var temfolder = _storageProvider.GetTempRelativePath();

                string path = await _storageProvider.Save(temfolder, zipFile);
                var targetPath = Path.Combine(_hostingEnvironment.WebRootPath, _fileOptions.Path, temfolder);
                var source = Path.Combine(_hostingEnvironment.ContentRootPath, "Plugins");
                if (path.StartsWith("/"))
                {
                    path = path.TrimStart('/');
                }
                path = path.Replace("/", "\\");
                var physicalPath = Path.Combine(_hostingEnvironment.WebRootPath, path);
                ZipFile.ExtractToDirectory(physicalPath, targetPath);
                var pluginDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "Plugins");
                FileHelper.CopyDirectoryOnly(targetPath, pluginDirectory);
                System.IO.Directory.Delete(targetPath,true);
                return new PgStatus
                {
                    IsInstalled = true
                };
            }
            catch (Exception e)
            {
                return new PgStatus
                {
                    IsInstalled = false,
                    HasError = true,
                    ErrorMessage = e.Message
                };
            }

        }
    }
}