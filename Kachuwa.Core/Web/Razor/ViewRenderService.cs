using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Kachuwa.Web.Razor
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorPageFactoryProvider _pageFactory;
        private readonly IRazorPageActivator _pageActivator;
        private readonly HtmlEncoder _htmlEncoder;
        private readonly IOptions<RazorViewEngineOptions> _optionsAccessor;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ViewRenderService(
              IRazorPageFactoryProvider pageFactory,
            IRazorPageActivator pageActivator,
            HtmlEncoder htmlEncoder,
            IOptions<RazorViewEngineOptions> optionsAccessor,
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,IHostingEnvironment hostingEnvironment)
        {
            _pageFactory = pageFactory;
            _pageActivator = pageActivator;
            _htmlEncoder = htmlEncoder;
            _optionsAccessor = optionsAccessor;
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<string> RenderToStringAsync(string controller,string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var routeData = new RouteData();
            routeData.Values.Add("controller", controller);
            var actionContext = new ActionContext(httpContext, routeData,
                new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );
                await viewResult.View.RenderAsync(viewContext);
                // var ss = (Razor2View) viewResult.View;
                //await ss.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

        public async Task<string> RenderToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var routeData = new RouteData();
            routeData.Values.Add("controller", "");
            var actionContext = new ActionContext(httpContext, routeData,
                new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext,viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );
                await viewResult.View.RenderAsync(viewContext);
                // var ss = (Razor2View) viewResult.View;
                //await ss.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

        private  bool CheckViewRelativePath(string path)
        {
            return path[0] == '~' || path[0] == '/';
        }

        private bool CheckFileExist(string filePath)
        {
            string path = filePath.Replace('/', '\\');
            string normalizedSecondPath = path.TrimStart(new char[] { '\\' });
            string physicalPath= Path.Combine(_hostingEnvironment.ContentRootPath, normalizedSecondPath);
            if (File.Exists(physicalPath))
                return true;
            return false;
        }
        public async Task<string> RenderTemplateAsync(string viewFilePath, object model)
        {
            if (CheckViewRelativePath(viewFilePath) ||
                viewFilePath.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
            {
                if (!CheckFileExist(viewFilePath))
                    throw new Exception("File does not exist.");
                string result = "";
                var factoryResult = _pageFactory.CreateFactory(viewFilePath);
                if (factoryResult.Success)
                {
                    // Only need to lookup _ViewStarts for the main page.
                    var viewStartPages = Array.Empty<ViewLocationCacheItem>(); //no main page
                    if (factoryResult.IsPrecompiled)
                    {
                        //_logger.PrecompiledViewFound(relativePath);
                    }
                }
                var page = factoryResult.RazorPageFactory;
                //no view starts in template
                var viewStarts = new IRazorPage[0];

                var view = new RazorView(_razorViewEngine, _pageActivator, viewStarts, page(), _htmlEncoder);

                var httpContext = new DefaultHttpContext {RequestServices = _serviceProvider};
                var routeData = new RouteData();
                routeData.Values.Add("controller", "");
                var actionContext = new ActionContext(httpContext, routeData,
                    new ActionDescriptor());
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngineResult.Found(viewFilePath, view);

                    if (viewResult.View == null)
                    {
                        throw new ArgumentNullException("View not found!");
                    }

                    var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(),
                        new ModelStateDictionary())
                    {
                        Model = model
                    };

                    var viewContext = new ViewContext(
                        actionContext,
                        viewResult.View,
                        viewDictionary,
                        new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                        sw,
                        new HtmlHelperOptions()
                    );
                    await viewResult.View.RenderAsync(viewContext);
                    result = sw.ToString();
                }
                return result;
            }
            else
            {
                throw new Exception("Invalid View path,Specify relative path for view.");
            }
        }
    }
}