﻿using System;
using System.Linq;
using Kachuwa.Caching;
using Kachuwa.Extensions;
using Kachuwa.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
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
            throw new NotImplementedException();
        }
        private string[] ignoreFiles = new string[] { "Component", "Plugin","Components","Plugins","_ViewImports", "_ViewStart", "_Layout" };
        public IFileInfo GetFileInfo(string subpath)
        {
            if (ignoreFiles.Any(subpath.Contains))
            {
                return new NotFoundFileInfo(subpath);
            }

            //checking if view request from page controller
            object kpageUrl = null;
            var currentContext = ContextResolver.Context;
            currentContext.Items.TryGetValue("KPageUrl", out kpageUrl);
            if (kpageUrl == null)
                return new NotFoundFileInfo(subpath);
            else
            {
                
                //pass pageurl
                var result = new PageFileInfo(_pageService, _log, _cacheService, kpageUrl.ToString().DecodeUrl(), subpath);
                return result.Exists ? result as IFileInfo : new NotFoundFileInfo(subpath);
            }


        }

        public IChangeToken Watch(string filter)
        {
           // return NullChangeToken.Singleton;
            return new PageChangeToken(_pageService, _log, _cacheService, filter);
        }
    }
    internal class EmptyDisposable : IDisposable
    {
        public static EmptyDisposable Instance { get; } = new EmptyDisposable();
        private EmptyDisposable() { }
        public void Dispose() { }
    }
}