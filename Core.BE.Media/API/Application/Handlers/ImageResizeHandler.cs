using Emeint.Core.BE.Media.Domain.Enums;
//using SkiaSharp;

namespace Emeint.Core.BE.Media.API.Application.Handlers
{
    public class ImageResizeHandler
    {
        //private static SKSize GetRequiredSize(SKBitmap fullsizeImage, int requiredWidth, int requiredHeight, int scalingMode)
        //{
        //    switch (scalingMode)
        //    {
        //        case (int)ImageScalingMode.Overflow:

        //            int sourceWidth = fullsizeImage.Width;
        //            int sourceHeight = fullsizeImage.Height;

        //            float nPercentW = ((float)requiredWidth / (float)sourceWidth);
        //            float nPercentH = ((float)requiredHeight / (float)sourceHeight);
        //            float nPercent = (nPercentH > nPercentW) ? nPercentH : nPercentW;

        //            requiredWidth = (int)(sourceWidth * nPercent);
        //            requiredHeight = (int)(sourceHeight * nPercent);

        //            break;
        //        case (int)ImageScalingMode.Stretch:
        //            //No need to add code here as long as size will be create directly with requiredWidth and requiredHeight
        //            break;
        //        case (int)ImageScalingMode.Fit:
        //        default: //scale to fit
        //            if (requiredWidth > fullsizeImage.Width && requiredHeight > fullsizeImage.Height)
        //            {
        //                return new SKSize(fullsizeImage.Width,fullsizeImage.Height);
        //            }

        //            bool onlyResizeIfWider = fullsizeImage.Width > fullsizeImage.Height;
        //            if (onlyResizeIfWider)
        //            {
        //                if (fullsizeImage.Width <= requiredWidth)
        //                {
        //                    requiredWidth = fullsizeImage.Width;
        //                }
        //            }

        //            int newHeight = fullsizeImage.Height * requiredWidth / fullsizeImage.Width;
        //            if (newHeight > requiredHeight)
        //            {
        //                // Resize with height instead
        //                requiredWidth = fullsizeImage.Width * requiredHeight / fullsizeImage.Height;
        //                newHeight = requiredHeight;
        //            }

        //            requiredHeight = newHeight;

        //            break;
        //    }
        //    return new SKSize(requiredWidth, requiredHeight);
        //}

    }
}
