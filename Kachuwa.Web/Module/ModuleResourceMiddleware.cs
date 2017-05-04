using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kachuwa.Extensions;
using Kachuwa.Web.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Kachuwa.Web.Module
{
    /// <summary>
    /// Load embeded resources file from module
    /// </summary>
    public class ModuleResourceMiddleware
    {
        #region Members
        /// <summary>
        /// The next middleware in the pipeline.
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// The currently embedded asset types.
        /// </summary>
        private readonly Dictionary<string, string> _contentTypes = new Dictionary<string, string>() {
            { ".ico", "image/x-icon" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".css", "text/css" },
            { ".js", "text/javascript" }
        };
        #endregion

        /// <summary>
        /// Creates a new middleware instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        public ModuleResourceMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current http context</param>
        /// <returns>An async task</returns>
        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (path.StartsWith("/module/"))
            {
                var moduleContainer = context.RequestServices.GetService<ModuleContainer>();

                string[] pathparts = path.Split('/');
                //remove empty array
                pathparts = pathparts.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                string moduleName = pathparts[1];//0=module/1=modulename
                var moduleAssembly = moduleContainer.Modules.SingleOrDefault(e => e.Name.ToLower() == moduleName.ToLower());
                // var moduleAssembly = moduleContainer.Modules.First();
                if (moduleAssembly == null)
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                var provider = new EmbeddedFileProvider(moduleAssembly.Assembly);
                var resourceFilepath = path.Replace("/module/", "");
                //since embedded resource is case sensitive and we need to resources folders file in lowercase
                resourceFilepath = resourceFilepath.Replace(moduleName + "/", "").ToLower();

               // var resources = moduleAssembly.Assembly.GetManifestResourceNames();
               // string resourceOrgFilepath = resources.SingleOrDefault(x => x.Contains(resourceFilepath.Replace('/','.')));
               
                var fileInfo = provider.GetFileInfo(resourceFilepath);

                if (fileInfo.Exists)
                {
                    var headers = context.Response.GetTypedHeaders();
                    var etag = GenerateETag(path, fileInfo.LastModified);

                    var etagHeader = context.Request.Headers["If-None-Match"];
                    if (etagHeader.Count == 0 || etagHeader[0] != etag)
                    {
                        context.Response.ContentType = GetContentType(Path.GetExtension(path));
                        context.Response.ContentLength = fileInfo.Length;
                        context.Response.Headers["ETag"] = etag;
                        headers.LastModified = fileInfo.LastModified.ToUniversalTime();

                        await context.Response.SendFileAsync(fileInfo);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status304NotModified;
                        //fixed system.InvlalidOperationException Write to non-body 304
                       // await context.Response.WriteAsync("");
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
            else
            {
                await next.Invoke(context);
            }
        }



        #region Private methods
        /// <summary>
        /// Gets the content type for the asset.
        /// </summary>
        /// <param name="path">The asset path</param>
        /// <returns>The content type</returns>
        private string GetContentType(string path)
        {
            try
            {
                return _contentTypes[Path.GetExtension(path)];
            }
            catch
            {
                return "text/plain";
            }
          
        }
        #endregion

        /// <summary>
        /// Generates a ETag from the given name and date.
        /// </summary>
        /// <param name="name">The resource name</param>
        /// <param name="date">The modification date</param>
        /// <returns>The etag</returns>
        public static string GenerateETag(string name, DateTime date)
        {
            var str = name + date.ToString("yyyy-MM-dd HH:mm:ss");
            return str.ToMd5();

        }
        private static string GenerateETag(string name, DateTimeOffset lastModified)
        {
            var str = name + lastModified.ToString("yyyy-MM-dd HH:mm:ss");
            return str.ToMd5();
        }
    }
}