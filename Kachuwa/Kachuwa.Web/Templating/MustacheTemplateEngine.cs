using System;
using System.IO;
using Kachuwa.Log;
using Microsoft.AspNetCore.Hosting;
using Mustache;

namespace Kachuwa.Web
{
    public class MustacheTemplateEngine : ITemplateEngine
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        readonly HtmlFormatCompiler _htmlCompiler = new HtmlFormatCompiler();
        readonly FormatCompiler _compiler = new FormatCompiler();
        public MustacheTemplateEngine(ILogger logger,IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public string Render(string template, object model)
        {
            try
            {
                Generator generator = _compiler.Compile(@template);
                string result = generator.Render(model);
                return result;
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                throw;
            }
          
        }

        public string Render(string template, object model, bool isHtml)
        {
            try
            {
                if (isHtml)
                {
                    Generator generator = _htmlCompiler.Compile(@template);
                    string result = generator.Render(model);
                    return result;
                }
                else
                {
                    Generator generator = _compiler.Compile(@template);
                    string result = generator.Render(model);
                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                throw;
            }
        }

        public string RenderFromFile(string filePath, object model, bool isHtml)
        {
            try
            {
                filePath= Path.Combine(_hostingEnvironment.ContentRootPath, filePath);
                if (File.Exists(filePath))
                {
                    string template = File.ReadAllText(filePath);
                    if (isHtml)
                    {
                        Generator generator = _htmlCompiler.Compile(@template);
                        string result = generator.Render(model);
                        return result;
                    }
                    else
                    {
                        Generator generator = _compiler.Compile(@template);
                        string result = generator.Render(model);
                        return result;
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                throw;
            }
        }
    }
    
}