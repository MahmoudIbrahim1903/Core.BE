using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using SkiaSharp;
using Emeint.Core.BE.Media.Utilities;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Configurations;
using System.Drawing.Imaging;
using Emeint.Core.BE.Media.Managers;
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.Media.API.Infrastructure.Middlewares
{
    public class ImageDownloaderMiddleware
    {
        public struct ResizeParams
        {
            public bool hasParams;
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
        private readonly ILogger<ImageDownloaderMiddleware> _logger;
        private readonly IHostingEnvironment _env;
        private readonly IMemoryCache _memoryCache;
        private IMediaStorageManager _mediaStorageManager;
        private readonly IConfiguration _configuration;

        private static readonly string[] extension = new string[] {
            ".png",
            ".jpg",
            ".jpeg"
        };

        public ImageDownloaderMiddleware(RequestDelegate next, IHostingEnvironment env,
            ILogger<ImageDownloaderMiddleware> logger,
            IMemoryCache memoryCache,
            IConfiguration configuration
            )
        {
            _next = next;
            _env = env;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, IMediaStorageManager mediaStorageManager)
        {
            _mediaStorageManager = mediaStorageManager;


            // Get path
            var path = context.Request.Path;

            // hand to next middleware if we are not dealing with an image
            if (!path.Value.StartsWith(_configuration["ImagesRootFolder"]))
            {
                await _next.Invoke(context);
                return;
            }

            //Correcting the Image path to match the actual path
            path = ImagePathUtility.GetImageQueryPath(context.Request.Path);
            bool enableImageResizing = false;

            if (!string.IsNullOrEmpty(_configuration["EnableImageResizing"]))
                enableImageResizing = Convert.ToBoolean(_configuration["EnableImageResizing"]);

            if (context.Request.Query.Count == 0 || !enableImageResizing)
            {
                var imgExists = await _mediaStorageManager.ImageExistsAsync(path);
                if (imgExists)
                {
                    await _mediaStorageManager.WriteToResponse(_next, context, path);
                    return;
                }
                else
                {
                    var imgCode = context.Request.Path.Value.Split("/").Last();
                    throw new InvalidParameterException("image_code", imgCode);
                }
            }

            var resizeParams = GetResizeParams(path, context.Request.Query);
            if (!resizeParams.hasParams || (resizeParams.w == 0 && resizeParams.h == 0))
            {
                var imgExists = await _mediaStorageManager.ImageExistsAsync(path);
                if (imgExists)
                {
                    await _mediaStorageManager.WriteToResponse(_next, context, path);
                    return;
                }
                else
                {
                    var imgCode = context.Request.Path.Value.Split("/").Last();
                    throw new InvalidParameterException("image_code", imgCode);
                }
            }

            var resizedImagePath = ImagePathUtility.GetImageNameWithNewWidthHeight(path, resizeParams.w, resizeParams.h);
            var exists = await _mediaStorageManager.ImageExistsAsync(resizedImagePath);
            if (!exists)
                await ResizeImage(path, resizedImagePath, resizeParams.w, resizeParams.h);
            await _mediaStorageManager.WriteToResponse(_next, context, resizedImagePath);

            return;
        }

        //private async Task WriteToResponseAsync(HttpContext context, PathString path, MemoryStream imageStream)
        //{
        //    imageStream.Seek(0, SeekOrigin.Begin);
        //    context.Response.ContentType = Path.GetExtension(path) == "png" ? "image/png" : "image/jpeg";
        //    context.Response.ContentLength = imageStream.Length;
        //    await context.Response.Body.WriteAsync(imageStream.ToArray(), 0, (int)imageStream.Length);
        //}

        #region Removed skia sharp
        //private SKData GetImageData(string imagePath, ResizeParams resizeParams, DateTime lastWriteTimeUtc)
        //{
        //    SKData imageData;
        //    byte[] imageBytes;
        //    long cacheKey;
        //    SKCodecOrigin origin; // this represents the EXIF orientation

        //    unchecked
        //    {
        //        cacheKey = imagePath.GetHashCode() + lastWriteTimeUtc.ToBinary() + resizeParams.ToString().GetHashCode();
        //    }

        //    // check cache and return if cached in memory
        //    if (_configurationManager.CacheImagesInMemory())
        //    {

        //        bool isCached = _memoryCache.TryGetValue<byte[]>(cacheKey, out imageBytes);
        //        if (isCached)
        //        {
        //            _logger.LogInformation("Serving from cache");
        //            return SKData.CreateCopy(imageBytes);
        //        }
        //    }
        //    else
        //    {
        //        //check cache and return if cached on disk
        //        var newImagePath = ImagePathUtility.GetImageNameWithNewWidthHeight(imagePath, resizeParams.w, resizeParams.h);
        //        if (File.Exists(newImagePath))
        //        {
        //            var cachedImage = LoadBitmap(File.OpenRead(newImagePath), out origin);
        //            var image = SKImage.FromBitmap(cachedImage);
        //            imageData = image.Encode();
        //            return imageData;
        //        }
        //    }



        //    var bitmap = LoadBitmap(File.OpenRead(imagePath), out origin); // always load as 32bit (to overcome issues with indexed color)

        //    // if autorotate = true, and origin isn't correct for the rotation, rotate it
        //    if (resizeParams.autorotate && origin != SKCodecOrigin.TopLeft)
        //        bitmap = RotateAndFlip(bitmap, origin);

        //    // if either w or h is 0, set it based on ratio of original image
        //    if (resizeParams.h == 0)
        //        resizeParams.h = (int)Math.Round(bitmap.Height * (float)resizeParams.w / bitmap.Width);
        //    else if (resizeParams.w == 0)
        //        resizeParams.w = (int)Math.Round(bitmap.Width * (float)resizeParams.h / bitmap.Height);

        //    // if we need to crop, crop the original before resizing
        //    if (resizeParams.mode == "crop")
        //        bitmap = Crop(bitmap, resizeParams);

        //    // store padded height and width
        //    var paddedHeight = resizeParams.h;
        //    var paddedWidth = resizeParams.w;

        //    // if we need to pad, or max, set the height or width according to ratio
        //    if (resizeParams.mode == "pad" || resizeParams.mode == "max")
        //    {
        //        var bitmapRatio = (float)bitmap.Width / bitmap.Height;
        //        var resizeRatio = (float)resizeParams.w / resizeParams.h;

        //        if (bitmapRatio > resizeRatio) // original is more "landscape"
        //            resizeParams.h = (int)Math.Round(bitmap.Height * ((float)resizeParams.w / bitmap.Width));
        //        else
        //            resizeParams.w = (int)Math.Round(bitmap.Width * ((float)resizeParams.h / bitmap.Height));
        //    }

        //    // resize
        //    var resizedImageInfo = new SKImageInfo(resizeParams.w, resizeParams.h, SKImageInfo.PlatformColorType, bitmap.AlphaType);
        //    var resizedBitmap = bitmap.Resize(resizedImageInfo, SKBitmapResizeMethod.Lanczos3);

        //    // optionally pad
        //    if (resizeParams.mode == "pad")
        //        resizedBitmap = Pad(resizedBitmap, paddedWidth, paddedHeight, resizeParams.format != "png");

        //    // encode
        //    var resizedImage = SKImage.FromBitmap(resizedBitmap);
        //    var encodeFormat = resizeParams.format == "png" ? SKEncodedImageFormat.Png : SKEncodedImageFormat.Jpeg;
        //    imageData = resizedImage.Encode(encodeFormat, resizeParams.quality);

        //    // cache the result
        //    if (_configurationManager.CacheImagesInMemory())
        //    {
        //        _memoryCache.Set<byte[]>(cacheKey, imageData.ToArray());

        //    }
        //    else
        //    {
        //        ImagePathUtility.SaveImageOnDisk(imagePath, imageData, resizeParams.w, resizeParams.h);
        //    }

        //    // cleanup
        //    resizedImage.Dispose();
        //    bitmap.Dispose();
        //    resizedBitmap.Dispose();
        //    return imageData;
        //}

        //private SKBitmap RotateAndFlip(SKBitmap original, SKCodecOrigin origin)
        //{
        //    // these are the origins that represent a 90 degree turn in some fashion
        //    var differentOrientations = new SKCodecOrigin[]
        //    {
        //        SKCodecOrigin.LeftBottom,
        //        SKCodecOrigin.LeftTop,
        //        SKCodecOrigin.RightBottom,
        //        SKCodecOrigin.RightTop
        //    };

        //    // check if we need to turn the image
        //    bool isDifferentOrientation = differentOrientations.Any(o => o == origin);

        //    // define new width/height
        //    var width = isDifferentOrientation ? original.Height : original.Width;
        //    var height = isDifferentOrientation ? original.Width : original.Height;

        //    var bitmap = new SKBitmap(width, height, original.AlphaType == SKAlphaType.Opaque);

        //    // todo: the stuff in this switch statement should be rewritten to use pointers
        //    switch (origin)
        //    {
        //        case SKCodecOrigin.LeftBottom:

        //            for (var x = 0; x < original.Width; x++)
        //                for (var y = 0; y < original.Height; y++)
        //                    bitmap.SetPixel(y, original.Width - 1 - x, original.GetPixel(x, y));
        //            break;

        //        case SKCodecOrigin.RightTop:

        //            for (var x = 0; x < original.Width; x++)
        //                for (var y = 0; y < original.Height; y++)
        //                    bitmap.SetPixel(original.Height - 1 - y, x, original.GetPixel(x, y));
        //            break;

        //        case SKCodecOrigin.RightBottom:

        //            for (var x = 0; x < original.Width; x++)
        //                for (var y = 0; y < original.Height; y++)
        //                    bitmap.SetPixel(original.Height - 1 - y, original.Width - 1 - x, original.GetPixel(x, y));

        //            break;

        //        case SKCodecOrigin.LeftTop:

        //            for (var x = 0; x < original.Width; x++)
        //                for (var y = 0; y < original.Height; y++)
        //                    bitmap.SetPixel(y, x, original.GetPixel(x, y));
        //            break;

        //        case SKCodecOrigin.BottomLeft:

        //            for (var x = 0; x < original.Width; x++)
        //                for (var y = 0; y < original.Height; y++)
        //                    bitmap.SetPixel(x, original.Height - 1 - y, original.GetPixel(x, y));
        //            break;

        //        case SKCodecOrigin.BottomRight:

        //            for (var x = 0; x < original.Width; x++)
        //                for (var y = 0; y < original.Height; y++)
        //                    bitmap.SetPixel(original.Width - 1 - x, original.Height - 1 - y, original.GetPixel(x, y));
        //            break;

        //        case SKCodecOrigin.TopRight:

        //            for (var x = 0; x < original.Width; x++)
        //                for (var y = 0; y < original.Height; y++)
        //                    bitmap.SetPixel(original.Width - 1 - x, y, original.GetPixel(x, y));
        //            break;

        //    }

        //    original.Dispose();

        //    return bitmap;


        //}

        //private SKBitmap LoadBitmap(Stream stream, out SKCodecOrigin origin)
        //{
        //    using (var s = new SKManagedStream(stream))
        //    {
        //        using (var codec = SKCodec.Create(s))
        //        {
        //            origin = codec.Origin;
        //            var info = codec.Info;
        //            var bitmap = new SKBitmap(info.Width, info.Height, SKImageInfo.PlatformColorType, info.IsOpaque ? SKAlphaType.Opaque : SKAlphaType.Premul);

        //            IntPtr length;
        //            var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(out length));
        //            if (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput)
        //            {
        //                return bitmap;
        //            }
        //            else
        //            {
        //                throw new ArgumentException("Unable to load bitmap from provided data");
        //            }
        //        }
        //    }
        //}

        //private SKBitmap Crop(SKBitmap original, ResizeParams resizeParams)
        //{
        //    var cropSides = 0;
        //    var cropTopBottom = 0;

        //    // calculate amount of pixels to remove from sides and top/bottom
        //    if ((float)resizeParams.w / original.Width < resizeParams.h / original.Height) // crop sides
        //        cropSides = original.Width - (int)Math.Round((float)original.Height / resizeParams.h * resizeParams.w);
        //    else
        //        cropTopBottom = original.Height - (int)Math.Round((float)original.Width / resizeParams.w * resizeParams.h);

        //    // setup crop rect
        //    var cropRect = new SKRectI
        //    {
        //        Left = cropSides / 2,
        //        Top = cropTopBottom / 2,
        //        Right = original.Width - cropSides + cropSides / 2,
        //        Bottom = original.Height - cropTopBottom + cropTopBottom / 2
        //    };

        //    // crop
        //    SKBitmap bitmap = new SKBitmap(cropRect.Width, cropRect.Height);
        //    original.ExtractSubset(bitmap, cropRect);
        //    original.Dispose();

        //    return bitmap;
        //}

        //private SKBitmap Pad(SKBitmap original, int paddedWidth, int paddedHeight, bool isOpaque)
        //{
        //    // setup new bitmap and optionally clear
        //    var bitmap = new SKBitmap(paddedWidth, paddedHeight, isOpaque);
        //    var canvas = new SKCanvas(bitmap);
        //    if (isOpaque)
        //        canvas.Clear(new SKColor(255, 255, 255)); // we could make this color a resizeParam
        //    else
        //        canvas.Clear(SKColor.Empty);

        //    // find co-ords to draw original at
        //    var left = original.Width < paddedWidth ? (paddedWidth - original.Width) / 2 : 0;
        //    var top = original.Height < paddedHeight ? (paddedHeight - original.Height) / 2 : 0;

        //    var drawRect = new SKRectI
        //    {
        //        Left = left,
        //        Top = top,
        //        Right = original.Width + left,
        //        Bottom = original.Height + top
        //    };

        //    // draw original onto padded version
        //    canvas.DrawBitmap(original, drawRect);
        //    canvas.Flush();

        //    canvas.Dispose();
        //    original.Dispose();

        //    return bitmap;
        //}
        #endregion

        private bool IsImagePath(PathString path)
        {
            if (path == null || !path.HasValue)
                return false;

            return extension.Any(x => x.EndsWith(ImagePathUtility.GetFileExtension(path), StringComparison.OrdinalIgnoreCase));
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
        private async Task ResizeImage(string originalImagePath, string resizedImagePath, int newWidth, int newHight)
        {
            //Import Original Image
            System.Drawing.Image originalImage = await _mediaStorageManager.ReadImageAsync(originalImagePath);

            if (originalImage == null)
            {
                var imgCode = originalImagePath.Split("/").Last();
                throw new InvalidParameterException("image_code", imgCode);
            }

            decimal widthToHightRatio = (decimal)originalImage.Width / (decimal)originalImage.Height;
            if (widthToHightRatio == 0)
                widthToHightRatio = 1;

            if (newWidth == 0)
                newWidth = (int)(widthToHightRatio * newHight);
            else
                newHight = (int)(newWidth / widthToHightRatio);

            var imageFormat = GetImageFormat(Path.GetExtension(originalImagePath).Replace(".", string.Empty));

            using (var stream = new MemoryStream())
            {
                originalImage
                .GetThumbnailImage(newWidth, newHight, () => false, IntPtr.Zero)
                .Save(stream, imageFormat);
                stream.Position = 0;

                //write new image too local desk
                await _mediaStorageManager.UploadImageAsync(stream.ToArray(), resizedImagePath);
            }
        }

        private ImageFormat GetImageFormat(string extension)
        {

            if (extension.Trim().ToLower() == "bmp")
                return ImageFormat.Bmp;
            else if (extension.Trim().ToLower() == "emf")
                return ImageFormat.Emf;
            else if (extension.Trim().ToLower() == "exif")
                return ImageFormat.Exif;
            else if (extension.Trim().ToLower() == "gif")
                return ImageFormat.Gif;
            else if (extension.Trim().ToLower() == "icon")
                return ImageFormat.Icon;
            else if (extension.Trim().ToLower() == "jpeg")
                return ImageFormat.Jpeg;
            else if (extension.Trim().ToLower() == "png")
                return ImageFormat.Png;
            else if (extension.Trim().ToLower() == "tiff")
                return ImageFormat.Tiff;
            else if (extension.Trim().ToLower() == "wmf")
                return ImageFormat.Wmf;
            else
                return ImageFormat.Jpeg;
        }
    }
}