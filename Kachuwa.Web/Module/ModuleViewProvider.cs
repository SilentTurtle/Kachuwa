using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Kachuwa.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Kachuwa.Web.Module
{
    public class ModuleViewProvider : IFileProvider
    {

        private  ConcurrentDictionary<string, Assembly> ModulesAssemblies { get; set; }
        private string _baseNamespace;
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars()
            .Where(c => c != '/' && c != '\\').ToArray();
        public ModuleViewProvider(ConcurrentDictionary<string, Assembly> assemblies)
        {
            ModulesAssemblies = assemblies;
        }
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();
        }
        private string[] ignoreFiles = new string[] { "_ViewImports", "_ViewStart", "_Layout" };
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
            if (subpath.ToLower().Contains("module") == false || subpath.ToLower().StartsWith("/area")|| subpath.ToLower().Contains("admin"))
            {
                return new NotFoundFileInfo(subpath);
            }
            subpath = subpath.Substring(subpath.IndexOf("Module", StringComparison.Ordinal) + 6);
            int firstIndex = subpath.IndexOfNth("/", 1);
            int secondIndex = subpath.IndexOfNth("/", 2);
            string pluginName = subpath.Substring(firstIndex + 1, secondIndex - 1);
            Assembly moduleAssembly = null;
            ModulesAssemblies.TryGetValue(pluginName, out moduleAssembly);
            if (moduleAssembly == null)
                return new NotFoundFileInfo(subpath);
            //project name as base namespace
            _baseNamespace = moduleAssembly.GetName().Name;// _assbly.GetTypes()[0].Namespace;
            //pluginname/viewname
            subpath = subpath.Replace(pluginName, subpath.Count(f => f == '/') <= 2 ? "Views/Shared" : "Views");
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
            var result = new ModuleViewInfo(moduleAssembly, resourcePath);
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