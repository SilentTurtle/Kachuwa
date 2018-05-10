using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Kachuwa.Web.API
{
    [Route("api/v1/file")]
    public class FileUploadApiController : BaseApiController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStorageProvider _storageProvider;
        public ILogger _logger { get; private set; }

        public FileUploadApiController(IHostingEnvironment hostingEnvironment, ILogger logger, IStorageProvider storageProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _storageProvider = storageProvider;
            _logger = logger;
        }

        [HttpPost]
        [Route("ajaxupload")]
        public async Task<dynamic> UploadFile()
        {
            try
            {
                var files = Request.Form.Files;
                List<string> paths = new List<string>();
                foreach (var file in files)
                {
                    string relativePath = await _storageProvider.Save(file);
                    paths.Add(relativePath);
                }

                return HttpResponse(200, "", paths);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message.ToString(), e);
                return ErrorResponse(500, e.Message);
            }

        }

        [HttpPost]
        [Route("remove")]
        public dynamic Remove([FromBody]string file)
        {
            try
            {
                string rootpath = _hostingEnvironment.WebRootPath;
                string filepath = Path.Combine(rootpath, file);
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                return HttpResponse(200, "", true);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message.ToString(), e);
                return ErrorResponse(500, e.Message);
            }

        }

      

        //[HttpPost]
        //[Route("crop")]
        //public async Task<ApiResponse> CropImage(CropModel model
        // )
        //{
        //    if (string.IsNullOrEmpty(model.imagePath)
        //        || !model.cropPointX.HasValue
        //        || !model.cropPointY.HasValue
        //        || !model.imageCropWidth.HasValue
        //        || !model.imageCropHeight.HasValue)
        //    {
        //        return HttpResponse((int)HttpStatusCode.NotImplemented, "Invalid parameters");
        //    }

        //    string existingfilepath = _hostingEnvironment.WebRootPath + model.imagePath.Replace("/", "\\");
        //    byte[] imageBytes = System.IO.File.ReadAllBytes(existingfilepath);
        //    byte[] croppedImage = ImageHelper.CropImage(imageBytes, model.cropPointX.Value, model.cropPointY.Value, model.imageCropWidth.Value, model.imageCropHeight.Value);

        //    string tempFolderName = Path.Combine(_hostingEnvironment.WebRootPath, "banner");

        //    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(model.imagePath);
        //    string fileName = Path.GetFileName(existingfilepath).Replace(fileNameWithoutExtension, fileNameWithoutExtension + "_cropped");
        //    string newfilePath = Path.Combine(tempFolderName, fileName);
        //    try
        //    {
        //        Kachuwa.IO.FileHelper.SaveFile(croppedImage, newfilePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log(LogType.Error, () => ex.Message);
        //        //Log an error     
        //        return HttpResponse(500, ex.Message);
        //    }

        //    string filepath = newfilePath.Replace(_hostingEnvironment.WebRootPath, "").Replace("\\", "/");
        //    return HttpResponse(200, "", filepath);
        //}

    }
}