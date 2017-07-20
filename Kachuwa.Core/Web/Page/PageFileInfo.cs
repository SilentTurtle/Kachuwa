using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Kachuwa.Caching;
using Kachuwa.Extensions;
using Kachuwa.Log;
using Microsoft.Extensions.FileProviders;

namespace Kachuwa.Web
{
    public class PageFileInfo : IFileInfo
    {
        private string _viewPath;
        private byte[] _viewContent;
        private DateTimeOffset _lastModified;
        private bool _exists;
        private readonly IPageService _pageService;
        private readonly ILogger _logger;
        private readonly ICacheService _cacheService;

        public PageFileInfo(IPageService pageService, ILogger logger, ICacheService cacheService, string pageUrl, string viewPath)
        {
            _pageService = pageService;
            _logger = logger;
            _cacheService = cacheService;
            _viewPath = viewPath;
            GetView(pageUrl);
        }
        public bool Exists => _exists;

        public bool IsDirectory => false;

        public DateTimeOffset LastModified => _lastModified;

        public long Length
        {
            get
            {
                using (var stream = new MemoryStream(_viewContent))
                {
                    return stream.Length;
                }
            }
        }

        public string Name => Path.GetFileName(_viewPath);

        public string PhysicalPath => null;

        public Stream CreateReadStream()
        {
            return new MemoryStream(_viewContent);
        }


        private void GetView(string pageUrl)
        {
            try
            {
                var key = (pageUrl + "findingview").ToMd5();
                var view2 = _cacheService.Get(key, 10, () =>
                {
                    var view = _pageService.CrudService.Get("Where IsActive=1 and IsPublished=1 and Url=@Url",
                        new { Url = pageUrl });

                    return (Page)view;
                });

                if (view2 != null)
                {
                    _exists = true;
                    view2.LastRequested = DateTime.UtcNow;
                    _pageService.CrudService.Update(view2);
                    //("Where Location=@Loaction", new { Location = viewPath });
                    
                    string headerNamespaces = _pageService.GetPageNamespaces(view2.UseMasterLayout);
                    _viewContent = Encoding.UTF8.GetBytes(headerNamespaces +view2.Content);
                   
                    _lastModified = Convert.ToDateTime(view2.LastRequested);
                }

            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);


            }

        }
    }
}