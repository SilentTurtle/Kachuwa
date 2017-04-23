//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewEngines;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;

//namespace Kachuwa.Core.Web.ViewEngine
//{
//    class Class1
//    {
//    }

//    public class KachuwaViewEngine : IView
//    {
//        private string _path;
//        private IServiceCollection _serviceCollection;

//        public KachuwaViewEngine(string path, IServiceCollection serviceCollection)
//        {
//            _path = path;
//            _serviceCollection = serviceCollection;
//        }
//        public async Task RenderAsync(ViewContext context)
//        {
//            var result = "here view file render";// Path, context.ViewData.Model);
//            context.Writer.Write(result);
//        }

//        public string Path { get; }
//    }
//    public interface IKachuwaViewEngine : IViewEngine
//    {
//    }
//    public interface IPugRendering
//    {
//        Task<string> Render(FileInfo pugFile, object model, ViewDataDictionary viewData, ModelStateDictionary modelState);
//    }
//    public interface IPugzorTempDirectoryProvider
//    {
//        string TempDirectory { get; }
//    }
//    public class KachuwaViewOptionsSetup : IConfigureOptions<MvcViewOptions>
//    {
//        private readonly IKachuwaViewEngine _pugzorViewEngine;

//        public KachuwaViewOptionsSetup(IKachuwaViewEngine pugzorViewEngine)
//        {
//            if (pugzorViewEngine == null)
//            {
//                throw new ArgumentNullException(nameof(pugzorViewEngine));
//            }

//            _pugzorViewEngine = pugzorViewEngine;
//        }

//        public void Configure(MvcViewOptions options)
//        {
//            if (options == null)
//            {
//                throw new ArgumentNullException(nameof(options));
//            }

//            options.ViewEngines.Add(_pugzorViewEngine);
//        }
//    }

//    //recommended way to add concrete view engin
//    // services.AddTransient<IConfigureOptions<MvcViewOptions>, PugzorMvcViewOptionsSetup>();
//    //services.AddSingleton<IPugzorViewEngine, PugzorViewEngine>();
//}
