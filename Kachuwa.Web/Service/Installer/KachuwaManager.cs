using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Configuration;
using Kachuwa.Data.Crud;
using Kachuwa.Installer;
using Kachuwa.Web.Module;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Kachuwa.Web.Service.Installer
{
    public class KachuwaConfigurationManager : IKachuwaConfigurationManager
    {
        private readonly KachuwaConnectionStrings _connectionString;
        private readonly IConfigToJson _configToJson;
        private readonly IScriptRunner _scriptRunner;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly KachuwaAppConfig _appConfig;

        public KachuwaConfigurationManager(IConfigToJson configToJson, 
            IOptionsSnapshot<KachuwaAppConfig> appConfig, 
            IOptionsSnapshot<KachuwaConnectionStrings> connectionString,
            IScriptRunner scriptRunner, 
            IHostingEnvironment hostingEnvironment,
                IApplicationLifetime applicationLifetime)
        {
            _connectionString = connectionString.Value;
            _configToJson = configToJson;
            _scriptRunner = scriptRunner;
            _hostingEnvironment = hostingEnvironment;
            _applicationLifetime = applicationLifetime;
            _appConfig = appConfig.Value;
        }

        private Dialect getDbProvider(string connectionString)
        {
            return Dialect.SQLServer;
        }

        private string getDbFolderPath(Dialect dialect)
        {
            switch (dialect)
            {
                case Dialect.SQLServer:
                    return "mssql";
                case Dialect.PostgreSQL:
                    return "postgres";
                case Dialect.MySQL:
                    return "mysql";
                case Dialect.SQLite:
                    return "sqllite";
            }

            return "";
        }

        private string GetInstallScripts(Dialect dialect)
        {
            string sqlfilepath = Path.Combine(_hostingEnvironment.ContentRootPath,
                $"db\\{_appConfig.Version}\\{getDbFolderPath(dialect)}\\install");
            var ext = new List<string> { "sql" };
            var files = Directory.GetFiles(sqlfilepath);

            var orderedEnumerable = files.OrderBy(e =>
            {
                var asdf = e.Split('.');
                var typ = asdf.GetType();
                return typ == typeof(int) ? e : null;
            });

            var scripts = (from file in orderedEnumerable where File.Exists(file) select File.ReadAllText(file)).ToList();


            return string.Join(" ", scripts);
        }

        public async Task<bool> Install(string connectionString)
        {
            try
            {
                if (await CheckConnection(connectionString))
                {
                    var dialect = getDbProvider(connectionString);
                    var scripts = GetInstallScripts(dialect);
                    var status = await _scriptRunner.Run(dialect, connectionString, scripts);
                    if (status)
                    {
                        _connectionString.DefaultConnection = connectionString;
                        _configToJson.SaveConnectionString(_connectionString);
                        _appConfig.IsInstalled = true;
                        _appConfig.DbProvider = dialect.ToString();
                        _configToJson.SaveKachuwaConfig(_appConfig);
                        _applicationLifetime.StopApplication();
                    }

                    return status;
                }
                else
                {
                    throw new DivideByZeroException();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<bool> Install(InstallerViewModel model)
        {
            try
            {

                var dialect = (Dialect)Enum.Parse(typeof(Dialect), model.DatabaseProvider);
                var scripts = GetInstallScripts(dialect);
                var status = await _scriptRunner.Run(dialect, model.ToString(), scripts);
                if (status)
                {
                    _connectionString.DefaultConnection = model.ToString();
                    _configToJson.SaveConnectionString(_connectionString);
                    _appConfig.IsInstalled = true;
                    _appConfig.DbProvider = dialect.ToString();
                    _configToJson.SaveKachuwaConfig(_appConfig);
                    _applicationLifetime.StopApplication();
                }

                return status;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<bool> CheckConnection(string connectionString)
        {
            try
            {
                var dialect = getDbProvider(connectionString);
                return await _scriptRunner.CheckConnection(dialect, connectionString);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private string GetUnInstallScripts(Dialect dialect)
        {
            string sqlfilepath = Path.Combine(_hostingEnvironment.ContentRootPath,
                $"db\\{_appConfig.Version}\\{getDbFolderPath(dialect)}\\uninstall");

            var files = Directory.GetFiles(sqlfilepath);

            var orderedEnumerable = files.OrderBy(e =>
            {
                var asdf = e.Split('.');
                var typ = asdf.GetType();
                return typ == typeof(int) ? e : null;
            });

            var scripts = (from file in orderedEnumerable where File.Exists(file) select File.ReadAllText(file)).ToList();


            return string.Join(" ", scripts);
        }
        public async Task<bool> Unintall(string connectionString)
        {
            try
            {

                var dialect = getDbProvider(connectionString);
                var scripts = GetUnInstallScripts(dialect);
                var status = await _scriptRunner.Run(dialect, connectionString, scripts);
                if (status)
                {
                    _connectionString.DefaultConnection = "";
                    _configToJson.SaveConnectionString(_connectionString);
                    _appConfig.IsInstalled = false;
                    _appConfig.DbProvider = "";// dialect.ToString();
                    _configToJson.SaveKachuwaConfig(_appConfig);
                    _applicationLifetime.StopApplication();
                }

                return status;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<string> BackUpDb(string connectionString)
        {
            throw new NotImplementedException();
        }

        public Task<string> BackUpSystem()
        {
            throw new NotImplementedException();
        }
    }
}
