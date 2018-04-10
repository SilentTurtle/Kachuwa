using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Configuration;
using Kachuwa.Data;
using Kachuwa.Web.Model;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MXTires.Microdata.Core;
using MXTires.Microdata.Core.Intangible;
using MXTires.Microdata.Core.Intangible.StructuredValues;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web
{
    public interface ISeoService
    {
        CrudService<SEO> Seo { get; set; }
        Task<bool> CheckUrlExist(string url, string type);
        Task<string> GenerateMetaContents();
        Task<SEO> GetBySeoType(string seoType, int id);
        Task<string> GenerateJsonLdForWebSite();
        Task<string> GenerateJsonLdForPage();
    }
}