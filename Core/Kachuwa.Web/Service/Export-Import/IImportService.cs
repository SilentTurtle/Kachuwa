using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using NPOI.HSSF.UserModel;

namespace Kachuwa.Web.Service
{
    public interface IImportService
    {
        List<T> Import<T>(IFormFile file);
        List<T> Import<T>(string filePath);
    }
}

    