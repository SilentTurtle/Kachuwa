using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Kachuwa.Caching;
using Kachuwa.Extensions;
using Kachuwa.Plugin;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Kachuwa.Web.Module
{
    public class ModuleViewProvider : IFileProvider
    {
        private readonly IModuleManager _moduleManager;
        private readonly ICacheService _cacheService;
        private string _baseNamespace;
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars()
            .Where(c => c != '/' && c != '\\').ToArray();
        public ModuleViewProvider(IModuleManager moduleManager ,ICacheService cacheService)
        {
            _moduleManager = moduleManager;
            _cacheService = cacheService;
        }
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
           
            if (string.IsNullOrEmpty(subpath))
                return (IDirectoryContents)NotFoundDirectoryContents.Singleton;

            if (ignoreFiles.Any(subpath.Contains))
            {
                return (IDirectoryContents)NotFoundDirectoryContents.Singleton;
            }
            if (subpath.Length != 0 && !string.Equals(subpath, "/", StringComparison.Ordinal))
                return (IDirectoryContents)NotFoundDirectoryContents.Singleton;
            List<IFileInfo> fileInfoList = new List<IFileInfo>();
            
            return (IDirectoryContents)new KachuwaDirectoryContents((IEnumerable<IFileInfo>)fileInfoList);
        }
       // private string[] ignoreFiles = new string[] { "_ViewImports", "_ViewStart", "_Layout" };
        private string[] ignoreFiles = new string[] { "_PageNotFound", "Component", "Plugin", "Components", "Plugins", "_ViewImports", "_ViewStart", "_Layout" };
        public IFileInfo GetFileInfo(string subpath)
        {
            if (string.IsNullOrEmpty(subpath))
            {
                return new NotFoundFileInfo(subpath);
            }
            if (ignoreFiles.Any(subpath.Contains))
            {
                return new NotFoundFileInfo(subpath);
            }

            //module/pluginname/home/index
            //module/pluginname/shared/index=>//module/pluginname/index
            if (subpath.ToLower().Contains("module") == false || subpath.ToLower().StartsWith("/area") || subpath.ToLower().Contains("admin"))
            {
                return new NotFoundFileInfo(subpath);
            }
            subpath = subpath.Substring(subpath.IndexOf("Module", StringComparison.Ordinal) + 6);
            int firstIndex = subpath.IndexOfNth("/", 1);
            int secondIndex = subpath.IndexOfNth("/", 2);
            string moduleName = subpath.Substring(firstIndex + 1, secondIndex - 1);

            var module = _moduleManager.FindAsync(moduleName).GetAwaiter().GetResult();

            if (module == null)
                return new NotFoundFileInfo(subpath);
            if (!module.IsInstalled)
            {
                return new NotFoundFileInfo(subpath);
            }
            Assembly moduleAssembly = module.Assembly;
            //project name as base namespace
            _baseNamespace = moduleAssembly.GetName().Name;// _assbly.GetTypes()[0].Namespace;
            //pluginname/viewname
            subpath = subpath.Replace(moduleName, subpath.Count(f => f == '/') <= 2 ? "Views/Shared" : "Views");
            var builder = new StringBuilder(_baseNamespace.Length + subpath.Length);
            builder.Append(_baseNamespace);

            //// Relative paths starting with a leading slash okay
            //if (subpath.StartsWith("/", StringComparison.Ordinal))
            //{
            //    builder.Append(subpath, 1, subpath.Length - 1);
            //}
            //else
            //{
            builder.Append(subpath);
            // }

            for (var i = _baseNamespace.Length; i < builder.Length; i++)
            {
                if (builder[i] == '/' || builder[i] == '\\')
                {
                    builder[i] = '.';
                }
            }

            var resourcePath = builder.ToString();
            if (HasInvalidPathChars(resourcePath))
            {
                return new NotFoundFileInfo(resourcePath);
            }

            var name = Path.GetFileName(subpath);
            if (moduleAssembly.GetManifestResourceInfo(resourcePath) == null)
            {
                return new NotFoundFileInfo(name);
            }
            var result = new ModuleViewInfo(moduleAssembly, resourcePath, _cacheService);
            return result.Exists ? result as IFileInfo : new NotFoundFileInfo(resourcePath);
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
        private static bool HasInvalidPathChars(string path)
        {
            return path.IndexOfAny(InvalidFileNameChars) != -1;
        }
    }
}