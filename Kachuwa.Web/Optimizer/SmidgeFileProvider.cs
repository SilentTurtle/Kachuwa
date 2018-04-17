using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kachuwa.Plugin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Kachuwa.Web.Optimizer
{
    public class SmidgeFileProvider : IFileProvider
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars()
            .Where(c => c != '/' && c != '\\').ToArray();
        public SmidgeFileProvider(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IDirectoryContents GetDirectoryContents(string subpath)
        {

            if (string.IsNullOrEmpty(subpath))
                return (IDirectoryContents)NotFoundDirectoryContents.Singleton;

            if (subpath.Length != 0 && !string.Equals(subpath, "/", StringComparison.Ordinal))
                return (IDirectoryContents)NotFoundDirectoryContents.Singleton;
            List<IFileInfo> fileInfoList = new List<IFileInfo>();

            return (IDirectoryContents)new KachuwaDirectoryContents((IEnumerable<IFileInfo>)fileInfoList);
        }
        public IFileInfo GetFileInfo(string filePath)
        {

            if (string.IsNullOrEmpty(filePath))
            {
                return new NotFoundFileInfo(filePath);
            }

            string wwwrootPath = _hostingEnvironment.WebRootPath;
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            if (filePath.StartsWith("~/"))
            {
                filePath = filePath.TrimStart('~');//remove ~
                filePath = filePath.TrimStart('/');//remove /
            }
            if (filePath.StartsWith("/"))
            {
                filePath = filePath.TrimStart('/');//remove /
            }

            filePath = filePath.Replace(@"/", @"\\");
            string physicalpath1 = Path.Combine(wwwrootPath, filePath);
            string physicalpath2 = Path.Combine(contentRootPath, filePath);
            if (!File.Exists(physicalpath1))
            {
                if (File.Exists(physicalpath2))
                {

                    var result = new SmidgeFileInfo(physicalpath2);
                    return result.Exists ? (IFileInfo)result : new NotFoundFileInfo(filePath);
                }
                else
                {
                    return new NotFoundFileInfo(filePath);
                }
            }
            else
            {
                var result = new SmidgeFileInfo(physicalpath1);
                return result.Exists ? (IFileInfo)result : new NotFoundFileInfo(filePath);
            }


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