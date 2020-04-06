using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Storage;
using Kachuwa.Web;
using Kachuwa.Web.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Kachuwa.Grid.API
{
    [Route("api/v1/kachuwa/grid")]
    public class FileUploadApiController : BaseController
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
        [Route("ajaxfileupload")]
        public async Task<dynamic> AjaxUploadFile()
        {
            try
            {
                var files = Request.Form.Files;
                var path = "Uploads";
                string folder = "Default";
                if (Request.Form.ContainsKey("fdr"))
                {
                    folder = Request.Form["fdr"].ToString();
                    path = path + "\\" + folder;
                }

                bool isUploaded = false;

                string rootpath = _hostingEnvironment.WebRootPath;
                string filepath = Path.Combine(rootpath, path);
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var tempFolderName = Path.GetTempFileName();
                string physicalfilepath = "";
                foreach (var file in files)
                {
                    var filename = ContentDispositionHeaderValue
                        .Parse(file.ContentDisposition)
                        .FileName.Trim();
                    //.Trim('"');
                    physicalfilepath = filepath + "\\" + Guid.NewGuid() + "_" + Path.GetExtension(file.FileName);


                    //bool key=  await _storageProvider.CheckIfKeyExists(file.FileName);
                    //   _storageProvider.SaveFile(key,);
                    //_storageProvider.CheckIfKeyExists()
                    try
                    {
                        CopyStream(file.OpenReadStream(), physicalfilepath);
                        isUploaded = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogType.Error, () => ex.Message, ex);
                    }
                }
                //int w = 0, h = 0;
                //System.Drawing.Image img = System.Drawing.Image.FromFile(physicalfilepath);
                //w = img.Width;
                //h = img.Height;
                //img.Dispose();
                string savedFilePath = physicalfilepath.Replace(rootpath, "").Replace("\\", "/");
                return HttpResponse(200, "", new { isUploaded, savedFilePath });
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message.ToString(), e);
                return ErrorResponse(500, e.Message);
            }

        }

        [HttpPost]
        [Route("ajaxupload")]
        public async Task<dynamic> UploadFile()
        {
            try
            {
                var files = Request.Form.Files;
                var path = "Uploads";
                string folder = "Default";
                if (Request.Form.ContainsKey("fdr"))
                {
                    folder = Request.Form["fdr"].ToString();
                    path = path + "\\" + folder;
                }

                bool isUploaded = false;

                string rootpath = _hostingEnvironment.WebRootPath;
                string filepath = Path.Combine(rootpath, path);
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var tempFolderName = Path.GetTempFileName();
                string physicalfilepath = "";
                foreach (var file in files)
                {
                    var filename = ContentDispositionHeaderValue
                        .Parse(file.ContentDisposition)
                        .FileName.Trim();
                        //.Trim('"');
                    physicalfilepath = filepath + "\\" +Guid.NewGuid()+"_"+ Path.GetExtension(file.FileName);


                    //bool key=  await _storageProvider.CheckIfKeyExists(file.FileName);
                    //   _storageProvider.SaveFile(key,);
                    //_storageProvider.CheckIfKeyExists()
                    try
                    {
                        CopyStream(file.OpenReadStream(), physicalfilepath);
                        isUploaded = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogType.Error, () => ex.Message, ex);
                    }
                }
                int w = 0, h = 0;
                System.Drawing.Image img = System.Drawing.Image.FromFile(physicalfilepath);
                w = img.Width;
                h = img.Height;
                img.Dispose();
                string savedFilePath = physicalfilepath.Replace(rootpath, "").Replace("\\", "/");
                return HttpResponse(200, "", new { isUploaded, savedFilePath, w, h });
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



        public void CopyStream(Stream stream, string destPath)
        {
            using (var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }
        public object HttpResponse(int statusCode, string msg, object data)
        {

            return new
            {
                Code = statusCode,
                Message = msg,
                Data = data
            };
        }

        public object HttpResponse(int statusCode, string msg)
        {
            return new
            {
                Code = statusCode,
                Message = msg,
            };
        }
        public object ValidationResponse(List<string> errors)
        {

            return new
            {
                Code = 600,
                Message = "Validation Error",
                Errors = errors ?? new List<string>()
            };
        }
        public object ErrorResponse(int statusCode, string msg)
        {

            return new
            {
                Code = statusCode,
                Message = msg,
                Errors = new string[] { msg }
            };
        }
        public object ErrorResponse(string msg)
        {
            return new
            {
                Code = (int)HttpStatusCode.BadRequest,
                Message = msg,
                Errors = new string[] { msg }
            };
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