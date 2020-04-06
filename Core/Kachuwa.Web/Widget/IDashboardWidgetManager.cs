using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kachuwa.Web
{
    public interface IDashboardWidgetManager
    {
        Task<IEnumerable<DashboardWidgetConfig>> GetDashboardWidgetConfigs(string dashboardName);

        Task<IEnumerable<DashboardWidgetConfig>> GetAllWidgets();

        Task<bool> SaveDashboardWidgets(string dashboardName,
            IEnumerable<DashboardWidgetConfig> widgetConfigs);


        Task<bool> ResetDashboardWidgets(string dashboardName);
    }
}