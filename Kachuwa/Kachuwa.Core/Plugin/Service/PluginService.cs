using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using Kachuwa.Data.Extension;
using Kachuwa.IO;
using Kachuwa.Log;
using Kachuwa.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Plugin
{
    public class PluginService : IPluginService
    {
        private readonly IKeyGenerator _keyGenerator;
        private readonly IFileOptions _fileOptions;
        private readonly IStorageProvider _storageProvider;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CrudService<Plugin> PluginCrudService { get; set; } = new CrudService<Plugin>();

        public PluginService(IKeyGenerator keyGenerator, IFileOptions fileOptions, IStorageProvider storageProvider,
            ILogger logger, IWebHostEnvironment hostingEnvironment)
        {
            _keyGenerator = keyGenerator;
            _fileOptions = fileOptions;
            _storageProvider = storageProvider;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<bool> UpdateStatus(Plugin plugin)
        {

            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("Update [dbo].[Plugin] SET IsInstalled=@IsInstalled IsActive=@IsActive Where SystemName=@SystemName;"
                                      , new { isActive = plugin.IsActive, plugin.IsInstalled, plugin.SystemName });
                return true;
            }
        }
        public async Task<bool> AddPlugins(IEnumerable<Plugin> plugins)
        {
            var dbPlugins = await PluginCrudService.GetListAsync();
            foreach (var plugin in plugins)
            {
                if (dbPlugins.Any())
                {
                    if (!dbPlugins.Any(e => e.SystemName == plugin.SystemName))
                    {
                        plugin.AutoFill();
                        await PluginCrudService.InsertAsync<int>(plugin);
                    }
                    else
                    {
                        plugin.AutoFill();
                        await PluginCrudService.UpdateAsync(plugin);
                    }
                }
                else
                {
                    plugin.AutoFill();
                    await PluginCrudService.InsertAsync<int>(plugin);
                }
            }

            return true;

        }

        public async Task<PluginInstallStatus> UnzipAndInstall(IFormFile zipFile)
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
                System.IO.Directory.Delete(targetPath, true);
                return new PluginInstallStatus
                {
                    IsInstalled = true
                };
            }
            catch (Exception e)
            {
                return new PluginInstallStatus
                {
                    IsInstalled = false,
                    HasError = true,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}
