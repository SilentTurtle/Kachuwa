using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Storage
{
    public interface IStorageProvider
    {
        //Task<bool> CheckIfKeyExists(string key);

       // Task<string> GetRedirectPath(string key);

       // Task SaveFile(string key, string contentType, Stream stream, IFileOptions options);
        Task<IFile> GetFile(string filePath, IFileOptions options);
        Task<string> Save(IFormFile file);
        Task<string> Save(string dirPath, IFormFile file);
        Task<string> CheckOrCreateDirectory(string path);
        Task<bool> Delete(string dirPath, string filePath);
        Task<bool> Delete(string filePath);
    }
}