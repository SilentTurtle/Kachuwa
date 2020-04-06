using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kachuwa.Web.Service
{
    public interface IExportService
    {
        Task<HttpResponseMessage> Export<T>(List<T> dataSources, string fileName, string sheetName);
    }
}