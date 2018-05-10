using System;
using System.Collections.Generic;
using System.Linq;
using Kachuwa.Caching;
using Kachuwa.Extensions;
using Kachuwa.Log;
using Kachuwa.Plugin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.Primitives;

namespace Kachuwa.Web
{
    public class PageFileProvider : IFileProvider
    {
        // private string _connection;
        private readonly IPageService _pageService;
        private readonly ILogger _log;
        private readonly ICacheService _cacheService;
        private readonly IHttpContextAccessor _contextAccessor;

        public PageFileProvider(IPageService pageService, ILogger log, ICacheService cacheService, IHttpContextAccessor contextAccessor)
        {
            //_connection = connection;
            _pageService = pageService;
            _log = log;
            _cacheService = cacheService;
            _contextAccessor = contextAccessor;
        }
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (string.IsNullOrEmpty(subpath))
                return (IDirectoryContents)NotFoundDirectoryContents.Singleton;

            if (_ignoreFiles.Any(subpath.Contains))
            {
                return (IDirectoryContents)NotFoundDirectoryContents.Singleton;
            }
            if (subpath.Length != 0 && !string.Equals(subpath, "/", StringComparison.Ordinal))
                return (IDirectoryContents)NotFoundDirectoryContents.Singleton;
            List<IFileInfo> fileInfoList = new List<IFileInfo>();
          
                //foreach (string manifestResourceName in _pluginAssemblies.GetValueOrDefault(key).GetManifestResourceNames())
                //{
                //    if (manifestResourceName.StartsWith(this._baseNamespace, StringComparison.Ordinal))
                //        fileInfoList.Add((IFileInfo)new EmbeddedResourceFileInfo(_pluginAssemblies.GetValueOrDefault(key), manifestResourceName, manifestResourceName.Substring(this._baseNamespace.Length), DateTimeOffset.Now));
                //}
            

            return (IDirectoryContents)new KachuwaDirectoryContents((IEnumerable<IFileInfo>)fileInfoList);
        }
        private readonly string[] _ignoreFiles = new string[] { "_PageNotFound","Component", "Plugin","Components","Plugins","_ViewImports", "_ViewStart", "_Layout" };
        public IFileInfo GetFileInfo(string subpath)
        {
            if (_ignoreFiles.Any(subpath.Contains))
            {
                return new NotFoundFileInfo(subpath);
            }
            object kpageUrl = null;
            var currentContext = ContextResolver.Context;
            currentContext.Items.TryGetValue("KPageUrl", out kpageUrl);
            if (kpageUrl == null)
                return new NotFoundFileInfo(subpath);
            else
            {                
                //pass pageurl
                var result = new PageFileInfo(_pageService, _log, _cacheService, kpageUrl.ToString().ToUrl(), subpath);
                return result.Exists ? result as IFileInfo : new NotFoundFileInfo(subpath);
            }


        }

        public IChangeToken Watch(string filter)
        {
           
            return new PageChangeToken(_pageService, _log, _cacheService, filter);
        }
    }
}