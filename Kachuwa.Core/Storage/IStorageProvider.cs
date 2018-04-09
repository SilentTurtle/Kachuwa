using System.IO;
using System.Threading.Tasks;

namespace Kachuwa.Storage
{
    public interface IStorageProvider
    {
        Task<bool> CheckIfKeyExists(string key);

        Task<string> GetRedirectPath(string key);

        Task SaveFile(string key, string contentType, Stream stream, IFileOptions options);

        Task<IFile> GetFile(string key, IFileOptions options);
    }
}