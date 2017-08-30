using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Log;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kachuwa.Web.Services
{
    public class LocalFileService: IFileService
    {
        private readonly IHostingEnvironment _environment;

        public LocalFileService(IHostingEnvironment environment)
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
    }
}
