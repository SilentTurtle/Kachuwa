using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Kachuwa.Caching;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Kachuwa.Log;
using SkiaSharp;

namespace Kachuwa.Web.Middleware
{
    public class ImageResizerMiddleware
    {
        struct ResizeParams
        {
            public bool hasParams ;
            public int w;
            public int h;
            public bool autorotate;
            public int quality; // 0 - 100
            public string format; // png, jpg, jpeg
            public string mode; // pad, max, crop, stretch

            public static string[] modes = new string[] { "pad", "max", "crop", "stretch" };

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"w: {w}, ");
                sb.Append($"h: {h}, ");
                sb.Append($"autorotate: {autorotate}, ");
                sb.Append($"quality: {quality}, ");
                sb.Append($"format: {format}, ");
                sb.Append($"mode: {mode}");

                return sb.ToString();
            }
        }

        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;
        private readonly ICacheService _cacheService;

        private static readonly string[] suffixes = new string[] {
            ".png",
            ".jpg",
            ".jpeg"
        };

        public ImageResizerMiddleware(RequestDelegate next, 
            IWebHostEnvironment env, ILogger logger,
            ICacheService cacheService)
        {
            _next = next;
            _env = env;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path;

            // hand to next middleware if we are not dealing with an image
            if ( !IsImagePath(path))//context.Request.Query.Count == 0 ||
            {
                await _next.Invoke(context);
                return;
            }
            // get the image location on disk
            var imagePath = Path.Combine(
                _env.WebRootPath,
                path.Value.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));

            // check file lastwrite
            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(imagePath);
            if (lastWriteTimeUtc.Year == 1601) // file doesn't exist, pass to next middleware
            {
                await _next.Invoke(context);
                return;
            }
            // hand to next middleware if we are dealing with an image but it doesn't have any usable resize querystring params
            var resizeParams = GetResizeParams(path, context.Request.Query);
            if (!resizeParams.hasParams || (resizeParams.w == 0 && resizeParams.h == 0))
            {
                var oriImage = GetOriginalImage(imagePath, lastWriteTimeUtc);

                // write to stream
                context.Response.ContentType = Path.GetExtension(path).ToLower() == ".png" ? "image/png" : "image/jpeg";
                context.Response.ContentLength = oriImage.Size;
                await context.Response.Body.WriteAsync(oriImage.ToArray(), 0, (int)oriImage.Size);

                // cleanup
                oriImage.Dispose();
                return;
            }

            // if we got this far, resize it
            //_logger.Log(LogType.Trace,()=> $"Resizing {path.Value} with params {resizeParams}");
            var imageData = GetImageData(imagePath, resizeParams, lastWriteTimeUtc);
            // write to stream
            context.Response.ContentType = Path.GetExtension(imagePath) == ".png" ? "image/png" : "image/jpeg";
            context.Response.ContentLength = imageData.Size;
            await context.Response.Body.WriteAsync(imageData.ToArray(), 0, (int)imageData.Size);

            // cleanup
            imageData.Dispose();

        }

        private SKData GetOriginalImage(string imagePath,DateTime lastWriteTimeUtc)
        {
            // check cache and return if cached

            var cacheKey = (imagePath.GetHashCode() + lastWriteTimeUtc.ToBinary()).ToString();


            SKData imageData;
            byte[] imageBytes;
            bool isCached = true;
            imageBytes = _cacheService.Get<byte[]>(cacheKey, () =>
            {
                isCached = false;
                SKCodecOrigin origin; // this represents the EXIF orientation
                var bitmap = LoadBitmap(File.OpenRead(imagePath), out origin); // always load as 32bit (to overcome issues with indexed color)
                // encode
                var resizedImage = SKImage.FromBitmap(bitmap);
                var encodeFormat = Path.GetExtension(imagePath).ToLower()== ".png" ? SKEncodedImageFormat.Png : SKEncodedImageFormat.Jpeg;
                imageData = resizedImage.Encode(encodeFormat, 100);
                // cleanup
                resizedImage.Dispose();
                bitmap.Dispose();
                // cache the result
                return imageData.ToArray();
            });
            return SKData.CreateCopy(imageBytes);
           
        }

        private SKData GetImageData(string imagePath, ResizeParams resizeParams, DateTime lastWriteTimeUtc)
        {
            // check cache and return if cached

            var cacheKey = (imagePath.GetHashCode() + lastWriteTimeUtc.ToBinary() + resizeParams.ToString().GetHashCode()).ToString();
            

            SKData imageData;
            byte[] imageBytes;
            bool isCached = true;
            imageBytes = _cacheService.Get<byte[]>(cacheKey, () =>
            {
                isCached = false;
                SKCodecOrigin origin; // this represents the EXIF orientation
                var bitmap = LoadBitmap(File.OpenRead(imagePath), out origin); // always load as 32bit (to overcome issues with indexed color)

                // if autorotate = true, and origin isn't correct for the rotation, rotate it
                if (resizeParams.autorotate && origin != SKCodecOrigin.TopLeft)
                    bitmap = RotateAndFlip(bitmap, origin);

                // if either w or h is 0, set it based on ratio of original image
                if (resizeParams.h == 0)
                    resizeParams.h = (int)Math.Round(bitmap.Height * (float)resizeParams.w / bitmap.Width);
                else if (resizeParams.w == 0)
                    resizeParams.w = (int)Math.Round(bitmap.Width * (float)resizeParams.h / bitmap.Height);

                // if we need to crop, crop the original before resizing
                if (resizeParams.mode == "crop")
                    bitmap = Crop(bitmap, resizeParams);

                // store padded height and width
                var paddedHeight = resizeParams.h;
                var paddedWidth = resizeParams.w;

                // if we need to pad, or max, set the height or width according to ratio
                if (resizeParams.mode == "pad" || resizeParams.mode == "max")
                {
                    var bitmapRatio = (float)bitmap.Width / bitmap.Height;
                    var resizeRatio = (float)resizeParams.w / resizeParams.h;

                    if (bitmapRatio > resizeRatio) // original is more "landscape"
                        resizeParams.h = (int)Math.Round(bitmap.Height * ((float)resizeParams.w / bitmap.Width));
                    else
                        resizeParams.w = (int)Math.Round(bitmap.Width * ((float)resizeParams.h / bitmap.Height));
                }

                // resize
                var resizedImageInfo = new SKImageInfo(resizeParams.w, resizeParams.h, SKImageInfo.PlatformColorType, bitmap.AlphaType);
                var resizedBitmap = bitmap.Resize(resizedImageInfo, SKBitmapResizeMethod.Lanczos3);

                // optionally pad
                if (resizeParams.mode == "pad")
                    resizedBitmap = Pad(resizedBitmap, paddedWidth, paddedHeight, resizeParams.format != "png");

                // encode
                var resizedImage = SKImage.FromBitmap(resizedBitmap);
                var encodeFormat = resizeParams.format == "png" ? SKEncodedImageFormat.Png : SKEncodedImageFormat.Jpeg;
                imageData = resizedImage.Encode(encodeFormat, resizeParams.quality);
                // cleanup
                resizedImage.Dispose();
                bitmap.Dispose();
                resizedBitmap.Dispose();
                // cache the result
                return imageData.ToArray();
            });
            //if (isCached)
            //{
              //  _logger.LogInformation("Serving from cache");
                return SKData.CreateCopy(imageBytes);
          // }

           // return imageData;
        }

        private SKBitmap RotateAndFlip(SKBitmap original, SKCodecOrigin origin)
        {
            // these are the origins that represent a 90 degree turn in some fashion
            var differentOrientations = new SKCodecOrigin[]
            {
                SKCodecOrigin.LeftBottom,
                SKCodecOrigin.LeftTop,
                SKCodecOrigin.RightBottom,
                SKCodecOrigin.RightTop
            };

            // check if we need to turn the image
            bool isDifferentOrientation = differentOrientations.Any(o => o == origin);

            // define new width/height
            var width = isDifferentOrientation ? original.Height : original.Width;
            var height = isDifferentOrientation ? original.Width : original.Height;

            var bitmap = new SKBitmap(width, height, original.AlphaType == SKAlphaType.Opaque);

            // todo: the stuff in this switch statement should be rewritten to use pointers
            switch (origin)
            {
                case SKCodecOrigin.LeftBottom:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(y, original.Width - 1 - x, original.GetPixel(x, y));
                    break;

                case SKCodecOrigin.RightTop:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(original.Height - 1 - y, x, original.GetPixel(x, y));
                    break;

                case SKCodecOrigin.RightBottom:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(original.Height - 1 - y, original.Width - 1 - x, original.GetPixel(x, y));

                    break;

                case SKCodecOrigin.LeftTop:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(y, x, original.GetPixel(x, y));
                    break;

                case SKCodecOrigin.BottomLeft:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(x, original.Height - 1 - y, original.GetPixel(x, y));
                    break;

                case SKCodecOrigin.BottomRight:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(original.Width - 1 - x, original.Height - 1 - y, original.GetPixel(x, y));
                    break;

                case SKCodecOrigin.TopRight:

                    for (var x = 0; x < original.Width; x++)
                        for (var y = 0; y < original.Height; y++)
                            bitmap.SetPixel(original.Width - 1 - x, y, original.GetPixel(x, y));
                    break;

            }

            original.Dispose();

            return bitmap;


        }

        private SKBitmap LoadBitmap(Stream stream, out SKCodecOrigin origin)
        {
            using (var s = new SKManagedStream(stream))
            {
                using (var codec = SKCodec.Create(s))
                {
                    origin = codec.Origin;
                    var info = codec.Info;
                    var bitmap = new SKBitmap(info.Width, info.Height, SKImageInfo.PlatformColorType, info.IsOpaque ? SKAlphaType.Opaque : SKAlphaType.Premul);

                    IntPtr length;
                    var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(out length));
                    if (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput)
                    {
                        return bitmap;
                    }
                    else
                    {
                        throw new ArgumentException("Unable to load bitmap from provided data");
                    }
                }
            }
        }

        private SKBitmap Crop(SKBitmap original, ResizeParams resizeParams)
        {
            var cropSides = 0;
            var cropTopBottom = 0;

            // calculate amount of pixels to remove from sides and top/bottom
            if ((float)resizeParams.w / original.Width < resizeParams.h / original.Height) // crop sides
                cropSides = original.Width - (int)Math.Round((float)original.Height / resizeParams.h * resizeParams.w);
            else
                cropTopBottom = original.Height - (int)Math.Round((float)original.Width / resizeParams.w * resizeParams.h);

            // setup crop rect
            var cropRect = new SKRectI
            {
                Left = cropSides / 2,
                Top = cropTopBottom / 2,
                Right = original.Width - cropSides + cropSides / 2,
                Bottom = original.Height - cropTopBottom + cropTopBottom / 2
            };

            // crop
            SKBitmap bitmap = new SKBitmap(cropRect.Width, cropRect.Height);
            original.ExtractSubset(bitmap, cropRect);
            original.Dispose();

            return bitmap;
        }

        private SKBitmap Pad(SKBitmap original, int paddedWidth, int paddedHeight, bool isOpaque)
        {
            // setup new bitmap and optionally clear
            var bitmap = new SKBitmap(paddedWidth, paddedHeight, isOpaque);
            var canvas = new SKCanvas(bitmap);
            if (isOpaque)
                canvas.Clear(new SKColor(255, 255, 255)); // we could make this color a resizeParam
            else
                canvas.Clear(SKColor.Empty);

            // find co-ords to draw original at
            var left = original.Width < paddedWidth ? (paddedWidth - original.Width) / 2 : 0;
            var top = original.Height < paddedHeight ? (paddedHeight - original.Height) / 2 : 0;

            var drawRect = new SKRectI
            {
                Left = left,
                Top = top,
                Right = original.Width + left,
                Bottom = original.Height + top
            };

            // draw original onto padded version
            canvas.DrawBitmap(original, drawRect);
            canvas.Flush();

            canvas.Dispose();
            original.Dispose();

            return bitmap;
        }

        private bool IsImagePath(PathString path)
        {
            if (path == null || !path.HasValue)
                return false;

            return suffixes.Any(x=> path.ToString().EndsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        private ResizeParams GetResizeParams(PathString path, IQueryCollection query)
        {
            ResizeParams resizeParams = new ResizeParams();

            // before we extract, do a quick check for resize params
            resizeParams.hasParams =
                resizeParams.GetType().GetTypeInfo()
                    .GetFields().Where(f => f.Name != "hasParams")
                    .Any(f => query.ContainsKey(f.Name));

            // if no params present, bug out
            if (!resizeParams.hasParams)
                return resizeParams;

            // extract resize params

            if (query.ContainsKey("format"))
                resizeParams.format = query["format"];
            else
                resizeParams.format = path.Value.Substring(path.Value.LastIndexOf('.') + 1);

            if (query.ContainsKey("autorotate"))
                bool.TryParse(query["autorotate"], out resizeParams.autorotate);

            int quality = 100;
            if (query.ContainsKey("quality"))
                int.TryParse(query["quality"], out quality);
            resizeParams.quality = quality;

            int w = 0;
            if (query.ContainsKey("w"))
                int.TryParse(query["w"], out w);
            resizeParams.w = w;

            int h = 0;
            if (query.ContainsKey("h"))
                int.TryParse(query["h"], out h);
            resizeParams.h = h;

            resizeParams.mode = "max";
            // only apply mode if it's a valid mode and both w and h are specified
            if (h != 0 && w != 0 && query.ContainsKey("mode") && ResizeParams.modes.Any(m => query["mode"] == m))
                resizeParams.mode = query["mode"];

            return resizeParams;
        }
    }
}