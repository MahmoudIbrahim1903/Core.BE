using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Managers
{
    public interface IPdfManager
    {
        Task<PdfDocument> ConvertImages(List<string> imagesCodes, string pdfName, bool deleteImagesAfterConverting = false);
        byte[] ConvertHtmlToPDF(string html);
    }
}
