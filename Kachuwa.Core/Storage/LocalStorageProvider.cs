using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Storage
{
    public class LocalStorageProvider : IStorageProvider
    {
        private readonly string _filePath;
        private readonly IFileOptions _fileOptions;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IHostingEnvironment _environment;
        public LocalStorageProvider(IFileOptions fileOptions, IKeyGenerator keyGenerator, IHostingEnvironment environment)
        {
            _fileOptions = fileOptions;
            _keyGenerator = keyGenerator;
            _environment = environment;
            string rootpath = _environment.WebRootPath;
            _filePath = Path.Combine(rootpath, _fileOptions.Path);
        }


        //public async Task SaveFile(string key, string contentType, Stream stream, IFileOptions options)
        //{
        //    if (!Directory.Exists(_filePath))
        //        Directory.CreateDirectory(_filePath);

        //    using (var f = File.OpenWrite($"{_filePath.TrimEnd('\\')}\\{key}.{ContentTypeToExtension(contentType, options)}"))
        //    {
        //        var buffer = new byte[8 * 1024];
        //        int len;
        //        while ((len = stream.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            await f.WriteAsync(buffer, 0, len);
        //        }
        //    }
        //}

        //public Task<string> GetRedirectPath(string key)
        //{
        //    var file = Path.GetExtension(Directory.GetFiles(_filePath, $"{key}.*").First());
        //    return Task.FromResult($"{_redirectPath}/{key}{file}");
        //}

        public async Task<IFile> GetFile(string filePath, IFileOptions options)
        {
            if (File.Exists(filePath))
            {

                using (var f = File.OpenRead(filePath))
                {
                    var ms = new MemoryStream();
                    await f.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    return new KachuwaFile()
                    {
                        ContentType = ExtensionToContentType(Path.GetExtension(filePath).TrimStart('.'), options),
                        Stream = ms
                    };
                }
            }
            else
            {
                throw new Exception("file Not found");
            }
        }



        public Task<bool> Delete(string dirPath, string filePath)
        {
            string physicallPath = CheckOrCreateDirectory(dirPath).Result;
            var path = Path.Combine(physicallPath, filePath);
            if (File.Exists(path))
                File.Delete(path);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            return Task.FromResult(true);
        }
        private void CopyStream(Stream stream, string destPath)
        {
            using (var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }

        public Task<string> Save(IFormFile file)
        {
            IFormFile myFile = file;
            string rootpath = _environment.WebRootPath;
            string folderPath = _filePath;//Path.Combine(rootpath, "Uploads");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            var tempFolderName = Path.GetTempFileName();
            string fileName = $"{_keyGenerator.GetKey()}.{ ContentTypeToExtension(myFile.ContentType, _fileOptions)}";// myFile.FileName;
            string physicalfilepath = folderPath + "\\" + fileName;
            var relativePath = Path.Combine(_fileOptions.Path, fileName);//uploads
            relativePath = relativePath.Replace("\\", "/").Replace(@"\", "/");
            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }
            if (myFile != null && myFile.Length != 0)
            {

                try
                {
                    CopyStream(myFile.OpenReadStream(), physicalfilepath);
                    return Task.FromResult(relativePath);


                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //}
            }
            return Task.FromResult("");
        }

        public Task<string> CheckOrCreateDirectory(string path)
        {
            string rootpath = _environment.WebRootPath;
            string folderPath = Path.Combine(rootpath, path);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            return Task.FromResult(folderPath);
        }

        public async Task<string> Save(string dirPath, IFormFile file)
        {
            string physicallPath = await CheckOrCreateDirectory(dirPath);
            IFormFile myFile = file;
            string fileName = $"{_keyGenerator.GetKey()}.{ ContentTypeToExtension(myFile.ContentType, _fileOptions)}";// myFile.FileName;


            string physicalfilepath = physicallPath + "\\" + fileName;
            var relativePath = Path.Combine(dirPath, fileName);
            relativePath = relativePath.Replace("\\", "/").Replace(@"\", "/");
            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }
            if (myFile != null && myFile.Length != 0)
            {

                try
                {
                    CopyStream(myFile.OpenReadStream(), physicalfilepath);
                    return relativePath;


                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //}
            }
            return string.Empty;
        }

        private string ExtensionToContentType(string extension, IFileOptions options)
        {
            if (options.AllowedTypes != null)
            {
                foreach (var allowedImageType in options.AllowedTypes)
                {
                    if (extension.Equals(allowedImageType.FileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        return allowedImageType.ContentType;
                    }
                }
            }

            throw new Exception("Unsupported extension.");
        }

        private string ContentTypeToExtension(string contentType, IFileOptions options)
        {
            if (options.AllowedTypes != null)
            {
                foreach (var allowedImageType in options.AllowedTypes)
                {
                    if (contentType.Equals(allowedImageType.ContentType, StringComparison.OrdinalIgnoreCase))
                    {
                        return allowedImageType.FileExtension;
                    }
                }
            }

            throw new Exception("Unsupported content type.");
        }
    }
}