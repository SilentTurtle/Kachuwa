using System;
using System.IO;
using System.Reflection;
using Kachuwa.Caching;
using Kachuwa.Extensions;
using Microsoft.Extensions.FileProviders;

namespace Kachuwa.Web.Module
{
    public class ModuleViewInfo : IFileInfo
    {
        private readonly string _viewPath;
        private byte[] _viewContent;
        private bool _exists;
        private readonly Assembly _assembly;
        private readonly ICacheService _cacheService;
        //private long? _length;

        public ModuleViewInfo(Assembly assembly, string viewPath, ICacheService cacheService)
        {
            _viewPath = viewPath;
            _assembly = assembly;
            _cacheService = cacheService;
            LastModified = File.GetLastWriteTimeUtc(_assembly.Location);
            GetView(viewPath);
        }
        public bool Exists => _exists;

        public bool IsDirectory => false;

        public DateTimeOffset LastModified { get; }

        public long Length
        {
            get
            {
                using (var stream = new MemoryStream(_viewContent))
                {
                    return stream.Length;
                }
            }
            //get
            //{
            //    if (!_length.HasValue)
            //    {
            //        using (var stream = _assembly.GetManifestResourceStream(_viewPath))
            //        {
            //            _length = stream.Length;
            //        }
            //    }
            //    return _length.Value;
            //}
        }

        public string Name => Path.GetFileName(_viewPath);

        public string PhysicalPath => null;

        public Stream CreateReadStream()
        {
            //  var stream = _assembly.GetManifestResourceStream(_viewPath);
            //return stream;
            if (_viewContent != null)
                return new MemoryStream(_viewContent);
            return null;
        }

        private void GetView(string viewPath)
        {

            try
            {
                var key = (viewPath + "modulesview").ToMd5();
                // bool loadedFromCache = true;
                var stream = _cacheService.Get(key, 10, () =>
                {
                    var non_cached_stream = _assembly.GetManifestResourceStream(viewPath);
                    byte[] buffer = new byte[16 * 1024];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        _exists = true;
                        int read;
                        while ((read = non_cached_stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                        return ms.ToArray();
                    }
                });
                _viewContent = stream;

              
            }
            catch (Exception ex)
            {
                _exists = false;
                // if something went wrong, Exists will be false
            }
        }
    }
}