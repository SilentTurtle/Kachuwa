using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Kachuwa.Web.Optimizer
{
    public class SmidgeFileInfo : IFileInfo
    {
        private readonly string _physicalPath;
        private byte[] _fileContent;
        private bool _exists;

        public SmidgeFileInfo(string physicalPath)
        {
            _physicalPath = physicalPath;
            GetFile(physicalPath);
        }
        public bool Exists => _exists;

        public bool IsDirectory => false;

        public DateTimeOffset LastModified { get; }

        public long Length
        {
            get
            {
                using (var stream = new MemoryStream(_fileContent))
                {
                    return stream.Length;
                }
            }
        }

        public string Name => Path.GetFileName(_physicalPath);

        public string PhysicalPath => null;

        public Stream CreateReadStream()
        {
            if (_fileContent != null)
                return new MemoryStream(_fileContent);
            return null;
        }

        private void GetFile(string filePath)
        {

            try
            {
                _fileContent = File.ReadAllBytes(filePath);
                _exists = true;
            }
            catch (Exception)
            {
                _exists = false;
            }
        }
    }
}