using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Kachuwa.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.Primitives;

namespace Kachuwa.Plugin
{
    public class PluginViewProvider : IFileProvider
    {
        private readonly Dictionary<string, Assembly> _pluginAssemblies;
        private string _baseNamespace;
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars()
            .Where(c => c != '/' && c != '\\').ToArray();
        public PluginViewProvider(Dictionary<string, Assembly> assemblies)
        {
            _pluginAssemblies = assemblies;
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
            foreach (var key in _pluginAssemblies.Keys)
            {
                foreach (string manifestResourceName in _pluginAssemblies.GetValueOrDefault(key).GetManifestResourceNames())
                {
                    if (manifestResourceName.StartsWith(this._baseNamespace, StringComparison.Ordinal))
                        fileInfoList.Add((IFileInfo)new EmbeddedResourceFileInfo(_pluginAssemblies.GetValueOrDefault(key), manifestResourceName, manifestResourceName.Substring(this._baseNamespace.Length),DateTimeOffset.Now));
                }
            }
           
            return (IDirectoryContents)new KachuwaDirectoryContents((IEnumerable<IFileInfo>)fileInfoList);
        }
        private string[] ignoreFiles = new string[] { "_ViewImports", "_ViewStart", "_Layout",
            "/Overrides/Plugins/Index.cshtml",
            "/Overrides/Admin/Plugins/Index.cshtml",
            "/Admin/Plugins/Index.cshtml",
            "/Views/Plugins/Index.cshtml",
            "/Overrides/Plugins/New.cshtml",
            "/Overrides/Admin/Plugins/New.cshtml",
            "/Admin/Plugins/New.cshtml",
            "/Views/Plugins/New.cshtml",
        };
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
            //if (subpath.Contains("_ViewImports") || subpath.Contains("_ViewStart"))
            //{
            //    return new NotFoundFileInfo(subpath);
            //}
            //plugin/pluginname/home/index
            //plugin/pluginname/shared/index=>//plugin/pluginname/index
            if (subpath.ToLower().Contains("plugin") == false)
            {
                return new NotFoundFileInfo(subpath);
            }
            subpath = subpath.Substring(subpath.IndexOf("Plugin", StringComparison.Ordinal) + 6);
            int firstIndex = subpath.IndexOfNth("/", 1);
            int secondIndex = subpath.IndexOfNth("/", 2);
            string pluginName = subpath.Substring(firstIndex + 1, secondIndex - 1);
            Assembly pluginAssembly = null;
            _pluginAssemblies.TryGetValue(pluginName, out pluginAssembly);
            if (pluginAssembly == null)
                return new NotFoundFileInfo(subpath);
            //project name as base namespace
            _baseNamespace = pluginAssembly.GetName().Name;// _assbly.GetTypes()[0].Namespace;
            //pluginname/viewname
            subpath = subpath.Replace(pluginName, subpath.Count(f => f == '/') <= 2 ? "Views" : "Views");
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
            if (pluginAssembly.GetManifestResourceInfo(resourcePath) == null)
            {
                return new NotFoundFileInfo(name);
            }
            var result = new PluginViewInfo(pluginAssembly, resourcePath);
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