using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using Kachuwa.Identity.Service;
using Kachuwa.Log;

namespace Kachuwa.Widgets
{
    public class DashboardWidgetService : IDashboardWidgetService
    {
     
        private readonly ILogger _logger;

        public DashboardWidgetService() { 
        }
       
    }
}
