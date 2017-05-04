using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using Kachuwa.Web.Razor;

namespace Kachuwa.Web
{
    public class PageService : IPageService
    {
        public CrudService<Page> CrudService { get; set; } = new CrudService<Page>();
        public async Task<bool> CheckPageExist(string url)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (SqlConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result = await db.QueryAsync<int>("Select 1 from dbo.Page Where URL=@URL", new { URL = url });
                return result != null && (result.SingleOrDefault() == 1 ? true : false);
            }
        }
    }
}
