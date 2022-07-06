using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Storage;
using Kachuwa.Web.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.API.V1
{
    [Route("api/v1/file")]
    [AllowAnonymous]
    public class ChunkFileUploadController : BaseApiController
    {
        private readonly IStorageProvider _storageProvider;
        // private static _storageProvider _storageProvider = new _storageProvider(new LocalFileSystemRepository());

        public ChunkFileUploadController(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        /// <summary>
        /// Create an upload session
        /// </summary>
        /// <remarks>creates a new upload session</remarks>
        /// <param name="userId">User ID</param>
        /// <param name="sessionParams">Session creation params</param>
        [HttpPost]
        [Route("create/{userId}")]
        //[SwaggerResponse(201, Type = typeof(SessionCreationStatusResponse))]
        //[SwaggerResponse(500)]
        public async Task<dynamic>  StartSession([FromRoute] long userId,
            [FromForm] CreateSessionParams sessionParams)
        {

            FileSession session = _storageProvider.CreateSession(userId, sessionParams.FileName,
                sessionParams.ChunkSize.Value,
                sessionParams.TotalSize.Value);
            
            return HttpResponse(200, "", SessionCreationStatusResponse.fromSession(session));
        }

        /// <summary>
        /// Uploads a file chunk
        /// </summary>
        /// <remarks>uploads a file chunk</remarks>
        /// <param name="userId">User ID</param>
        /// <param name="sessionId">Session ID</param>
        /// <param name="chunkNumber">Chunk number (starts from 1)</param>
        /// <param name="inputFile">File chunk content</param>
        [HttpPut]
        [Consumes("multipart/form-data")]
        [Route("upload/user/{userId}/session/{sessionId}/")]
        //[SwaggerResponse(200, Description = "Block upload successfully")]
        //[SwaggerResponse(202, Description = "Server busy during that particular upload. Try again")]
        //[SwaggerResponse(410, Description = "Session timeout")]
        //[SwaggerResponse(500, Description = "Internal server error")]
        public  async Task<dynamic>  UploadFileChunk([FromRoute, Required] long? userId,
            [FromRoute, Required] string sessionId,
            [FromQuery, Required] int? chunkNumber,
            [FromForm] IFormFile file)
        {
            if (!userId.HasValue)
                return ErrorResponse(500, "User missing");
           

            if (String.IsNullOrWhiteSpace(sessionId))
                return ErrorResponse(500, "Session ID is missing"); 

            if (chunkNumber < 1)
                return ErrorResponse(500, "Invalid chunk number");

            // due to a bug, inputFile comes null from Mvc
            // however, I want to test the code and have to pass it to the UploadFileChunk function...
            IFormFile ufile = file ;//?? Request.Form.Files.First());

            _storageProvider.PersistBlock(sessionId, userId.Value, chunkNumber.Value, ToByteArray(ufile.OpenReadStream()));

           // return Json(sessionId);
            return HttpResponse(200, "", sessionId);
        }

        /// <summary>
        /// Gets the status of a single upload
        /// </summary>
        /// <remarks>gets the status of a single upload</remarks>
        /// <param name="sessionId">Session ID</param>
        [HttpGet]
        [Route("upload/{sessionId}")]
        //[SwaggerResponse(404, Description = "Session not found")]
        //[SwaggerResponse(500, Description = "Internal server error")]
        //[SwaggerResponse(200, typeof(UploadStatusResponse))]
        public async Task<dynamic> GetUploadStatus([FromRoute, Required] string sessionId)
        {
            var session = _storageProvider.GetSession(sessionId);
          
            return HttpResponse(200, "", UploadStatusResponse.fromSession(session));
        }

        /// <summary>
        /// Gets the status of all uploads
        /// </summary>
        /// <remarks>gets the status of all uploads</remarks>
        [HttpGet]
        [Route("uploads")]
        //[SwaggerResponse(404, Description = "Session not found")]
        //[SwaggerResponse(200, typeof(List<UploadStatusResponse>))]
        public async Task<dynamic> GetAllUploadStatus()
        {
            var sessions = _storageProvider.GetAllSessions();
           // return UploadStatusResponse.fromSessionList(sessions);
            return HttpResponse(200, "", UploadStatusResponse.fromSessionList(sessions));
        }

        /// <summary>
        /// Downloads a previously uploaded file
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <remarks>downloads a previously uploaded file</remarks>
        [HttpGet]
        [Route("download/{sessionId}")]
        //[SwaggerResponse(200, Description = "OK")]
        //[SwaggerResponse(404, Description = "Session not found")]
        //[SwaggerResponse(500, Description = "Internal server error")]
        public async Task DownloadFile([FromRoute, Required] string sessionId)
        {
            FileSession session = _storageProvider.GetSession(sessionId);

            var response = targetResponse ?? Response;

            response.ContentType = "application/octet-stream";
            response.ContentLength = session.FileInfo.FileSize;
            response.Headers["Content-Disposition"] = "attachment; fileName=" + session.FileInfo.FileName;

            _storageProvider.WriteToStream(targetOutputStream ?? Response.Body, session);
        }

        private byte[] ToByteArray(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        

        Stream targetOutputStream = null;
        // intended for integration tests only
        [ApiExplorerSettings(IgnoreApi = true)]
        private void SetOuputStream(Stream replacementStream)
        {
            this.targetOutputStream = replacementStream;
        }

        HttpResponse targetResponse = null;
        [ApiExplorerSettings(IgnoreApi = true)]
        private void SetTargetResponse(HttpResponse replacementResponse)
        {
            this.targetResponse = replacementResponse;
        }
    }
}