using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Kachuwa.State;
using Newtonsoft.Json;

namespace Kachuwa.Storage
{
    public class LocalStorageProvider : IStorageProvider
    {
        private readonly string _filePath;
        private readonly IFileOptions _fileOptions;
        private readonly IKeyGenerator _keyGenerator;
        private readonly IWebHostEnvironment _environment;
        // private readonly ICacheService _cachingService;
       // private static Dictionary<string, FileSession> keys { get; set; } = new Dictionary<string, FileSession>();

        public LocalStorageProvider(IFileOptions fileOptions, IKeyGenerator keyGenerator, IWebHostEnvironment environment, ICacheService cachingService)
        {
            _fileOptions = fileOptions;
            _keyGenerator = keyGenerator;
            _environment = environment;
            // _cachingService = cachingService;
            string rootpath = environment.WebRootPath;
            _filePath = Path.Combine(rootpath, _fileOptions.Path);
            RootPath = _filePath;
        }

        public string GetTempPath()
        {
            return Path.Combine(_environment.WebRootPath, "temp", _keyGenerator.GetKey());
        }
        public string GetTempChunkedPath()
        {
            return Path.Combine(_environment.WebRootPath, "temp", "chuncked");
        }

        public string GetTempRelativePath()
        {
            return Path.Combine("temp", _keyGenerator.GetKey());
        }

        public string RootPath { get; set; }
        public async Task<IFile> GetFile(string filePath)
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
                        ContentType = ExtensionToContentType(Path.GetExtension(filePath).TrimStart('.'), _fileOptions),
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
            var physicalPath = "";
            if (filePath.StartsWith("/"))
            {
                filePath = filePath.TrimStart('/').Replace("/", "\\");
                physicalPath = Path.Combine(RootPath, filePath);
            }
            if (File.Exists(physicalPath))
                File.Delete(physicalPath);
            return Task.FromResult(true);
        }



        public void CopyStream(Stream stream, string destPath)
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

        public async Task<string> SaveFile(string filePath, string contentType, Stream stream)
        {
            filePath = filePath.Replace(@"/", @"\\");
            string folder = await CheckOrCreateDirectory(filePath);
            string fileName = $"{_keyGenerator.GetKey()}.{ ContentTypeToExtension(contentType, _fileOptions)}";// myFile.FileName;


            string physicalfilepath = folder + "\\" + fileName;
            var relativePath = Path.Combine(_fileOptions.Path, filePath, fileName);//uploads
            relativePath = relativePath.Replace("\\", "/").Replace(@"\", "/");
            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }
            using (var f = File.OpenWrite(physicalfilepath))
            {
                var buffer = new byte[8 * 1024];
                int len;
                while ((len = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await f.WriteAsync(buffer, 0, len);
                }
            }

            return relativePath;
        }

        public Task<string> CheckOrCreateDirectory(string path)
        {
            string rootpath = _environment.WebRootPath;
            string folderPath = Path.Combine(rootpath, _fileOptions.Path, path);
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
            var relativePath = Path.Combine(_fileOptions.Path, dirPath, fileName);
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


        private const int CHUNK_LIMIT = 1024 * 1024;

        #region Chunked File upload
        public FileSession CreateSession(long user, string fileName, int chunkSize, long fileSize)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                throw new BadRequestException("File name missing");

            if (chunkSize > CHUNK_LIMIT)
                throw new BadRequestException(String.Format("Maximum chunk size is {0} bytes", CHUNK_LIMIT));

            if (chunkSize < 1)
                throw new BadRequestException("Chunk size must be greater than zero");

            if (fileSize < 1)
                throw new BadRequestException("Total size must be greater than zero");

            FileSession session = new FileSession(user, new FileInformation(fileSize, fileName, chunkSize));

            //if (keys.TryGetValue()[session.Id] == null)
            //  keys.Add(session.Id, session);
            // return _cachingService.Get(session.Id,60*60, () => session);

            if (!Directory.Exists(Path.Combine(GetTempChunkedPath(), session.Id)))
            {
                Directory.CreateDirectory(Path.Combine(GetTempChunkedPath(), session.Id));
                File.WriteAllText(Path.Combine(GetTempChunkedPath(), session.Id, "session.json"),
                    JsonConvert.SerializeObject(session));
            }
            return session;


        }

        public FileSession GetSession(string id)
        {
            if (Directory.Exists(Path.Combine(GetTempChunkedPath(), id)))
            {
               
               var json= File.ReadAllText(Path.Combine(GetTempChunkedPath(), id, "session.json"));
               return JsonConvert.DeserializeObject<FileSession>(json);
            }

            return null;

        }

        public List<FileSession> GetAllSessions()
        {
            var list = new List<FileSession>();
            foreach (var key in Directory.GetFiles(Path.Combine(GetTempChunkedPath()), "*session.json", SearchOption.AllDirectories))
            {
                try
                {
                    var json = File.ReadAllText(key);
                    var file= JsonConvert.DeserializeObject<FileSession>(json);
                    list.Add(file);
                }
                catch (Exception e)
                {

                }

            }

            return list;
        }

        public void PersistBlock(string sessionId, long userId, int chunkNumber, byte[] buffer)
        {
            FileSession session = GetSession(sessionId);

            try
            {
                if (session == null)
                {
                    throw new NotFoundException("Session not found");
                }

                Persist(sessionId, chunkNumber, buffer);



                session.FileInfo.MarkChunkAsPersisted(chunkNumber);
                session.RenewTimeout();
                // _cachingService.Get(session.Id, 60 * 60, () => session);

                //keys[session.Id] = session;
                File.WriteAllText(Path.Combine(GetTempChunkedPath(), sessionId, "session.json"),
                    JsonConvert.SerializeObject(session));
            }
            catch (System.Exception e)
            {
                if (session != null)
                    session.MaskAsFailed();

                throw e;
            }
        }

        public void WriteToStream(Stream stream, FileSession session)
        {
            using (var sw = new BinaryWriter(stream))
            {
                for (int i = 1; i <= session.FileInfo.TotalNumberOfChunks; i++)
                {
                    sw.Write(Read(session.Id, i));
                }
            }

            stream.Flush();

        }

        public Stream GetFileStream(FileSession session)
        {
            return new ChunkedFileStream(this, session);

        }


        public async void Persist(string id, int chunkNumber, byte[] buffer)
        {
            string chunkDestinationPath = Path.Combine(GetTempChunkedPath(), id);

            if (!Directory.Exists(chunkDestinationPath))
            {
                Directory.CreateDirectory(chunkDestinationPath);
            }

            string path = Path.Combine(GetTempChunkedPath(), id, chunkNumber.ToString());
            await File.WriteAllBytesAsync(path, buffer);
        }

        public byte[] Read(string id, int chunkNumber)
        {
            string targetPath = Path.Combine(GetTempChunkedPath(), id, chunkNumber.ToString());
            return File.ReadAllBytes(targetPath);
        }




        #endregion
    }
}