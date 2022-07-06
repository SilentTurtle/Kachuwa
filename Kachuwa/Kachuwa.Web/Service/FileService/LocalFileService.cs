using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web.Services
{
    public class LocalFileService: IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public LocalFileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public void CopyStream(Stream stream, string destPath)
        {
            using (var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }

        public  string Save(IFormFile file)
        {
            IFormFile myFile = file;
            string rootpath = _environment.WebRootPath;
            string folderPath = Path.Combine(rootpath, "Uploads");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            var tempFolderName = Path.GetTempFileName();
            string physicalfilepath = folderPath + "\\" + myFile.FileName;
            var relativePath = Path.Combine("Uploads", myFile.FileName);
            relativePath =relativePath.Replace("\\", "/").Replace(@"\", "/");
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
            return "";
        }

        public string CheckOrCreateDirectory(string path)
        {
            string rootpath = _environment.WebRootPath;
            string folderPath = Path.Combine(rootpath, path);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string Save(string dirPath, IFormFile file)
        {
            string physicallPath= CheckOrCreateDirectory(dirPath);
            IFormFile myFile = file;
        
            string physicalfilepath = physicallPath+ "\\" + myFile.FileName;
            var relativePath = Path.Combine(dirPath, myFile.FileName);
            relativePath= relativePath.Replace("\\", "/").Replace(@"\", "/");
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
            return "";
        }

        public void Delete(string dirPath, string filePath)
        {
            string physicallPath = CheckOrCreateDirectory(dirPath);
            var path=Path.Combine(physicallPath, filePath);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}