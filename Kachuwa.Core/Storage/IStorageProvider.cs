using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Storage
{
    public interface IStorageProvider
    {
        string RootPath { get; set; }
        Task<IFile> GetFile(string filePath);
        Task<string> Save(IFormFile file);
        Task SaveFile(string contentType, Stream stream);
        Task<string> Save(string dirPath, IFormFile file);
        Task<string> CheckOrCreateDirectory(string path);
        Task<bool> Delete(string dirPath, string filePath);
        Task<bool> Delete(string filePath);
    }
}