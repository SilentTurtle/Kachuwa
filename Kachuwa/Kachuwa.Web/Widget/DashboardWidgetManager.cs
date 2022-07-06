using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Log;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Kachuwa.Web
{
    public class DashboardWidgetManager : IDashboardWidgetManager
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IWidgetService _widgetService;
        private readonly ILogger _logger;
        private string ConfigStorageLocation = "app_data/dashboard";
        private string dashboardConfigFileExtension = ".json";

        public DashboardWidgetManager(IWebHostEnvironment hostingEnvironment, IWidgetService widgetService,ILogger logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _widgetService = widgetService;
            _logger = logger;
        }

        private string GetPath(string dashboardName)

        {
            dashboardName = string.IsNullOrEmpty(dashboardName) ? "empty" : dashboardName;
            string directory = Path.Combine(_hostingEnvironment.ContentRootPath, ConfigStorageLocation);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            string filepath = Path.Combine(directory, dashboardName + dashboardConfigFileExtension);
            if (!File.Exists(filepath))
                using (File.Create(filepath)) { };
            return filepath;
        }

        private async Task<IEnumerable<DashboardWidgetConfig>> FetchFromConfigFile(string dashboardName)
        {
            var filepath = GetPath(dashboardName);
            var strDashboardConfig = await File.ReadAllTextAsync(filepath);
            var dashboardWidgetConfigs =
                JsonConvert.DeserializeObject<IEnumerable<DashboardWidgetConfig>>(strDashboardConfig);
            return dashboardWidgetConfigs ?? new List<DashboardWidgetConfig>();
        }
        public async Task<IEnumerable<DashboardWidgetConfig>> GetDashboardWidgetConfigs(string dashboardName)
        {
            return await FetchFromConfigFile(dashboardName);
        }

        public async Task<IEnumerable<DashboardWidgetConfig>> GetAllWidgets()
        {

            var widgets = await _widgetService.GetAllWidgets();
            if (widgets.Any())
            {
                return widgets.Select(x => new DashboardWidgetConfig()
                {
                    WidgetSystemName =  x.SystemName,
                    Settings = x.Settings
                }).ToList();
            }
            else
            {
                return new List<DashboardWidgetConfig>();
            }
        }


        public async Task<bool> SaveDashboardWidgets(string dashboardName, IEnumerable<DashboardWidgetConfig> widgetConfigs)
        {
            try
            {

                var filepath = GetPath(dashboardName);
                var configJson = JsonConvert.SerializeObject(widgetConfigs);
                await File.WriteAllTextAsync(filepath, configJson);
                return true;
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return false;
            }
        }

        public async Task<bool> ResetDashboardWidgets(string dashboardName)
        {
            try
            {

                var filepath = GetPath(dashboardName);
            
                await File.WriteAllTextAsync(filepath, "[]");
                return true;
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return false;
            }
        }
    }
}