using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web.Services
{
    public interface IFileService
    {
        string Save(IFormFile file);
        string Save(string dirPath,IFormFile file);
        string CheckOrCreateDirectory(string path);
    }
}