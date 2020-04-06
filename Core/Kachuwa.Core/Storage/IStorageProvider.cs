using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Storage
{
    public interface IStorageProvider

    {

        string GetTempPath();
        string GetTempChunkedPath();
        string GetTempRelativePath();
        string RootPath { get; set; }
        Task<IFile> GetFile(string filePath);
        Task<string> Save(IFormFile file);
        Task<string> SaveFile(string filePath, string contentType, Stream stream);
        Task<string> Save(string dirPath, IFormFile file);
        Task<string> CheckOrCreateDirectory(string path);
        Task<bool> Delete(string dirPath, string filePath);
        Task<bool> Delete(string filePath);
        void CopyStream(Stream stream, string destPath);

        #region chunk upload

        FileSession CreateSession(long user, String fileName, int chunkSize, long fileSize);

        FileSession GetSession(String id);

        List<FileSession> GetAllSessions();

        void PersistBlock(String sessionId, long userId, int chunkNumber, byte[] buffer);
        void WriteToStream(Stream stream, FileSession session);

        Stream GetFileStream(FileSession session);

        /// <summary>
        /// override uppon storate file base or memory...
        /// </summary>
        /// <param name="id"></param>
        /// <param name="chunkNumber"></param>
        /// <param name="buffer"></param>
        void Persist(string id, int chunkNumber, byte[] buffer);
        byte[] Read(string id, int chunkNumber);
        #endregion

    }
}