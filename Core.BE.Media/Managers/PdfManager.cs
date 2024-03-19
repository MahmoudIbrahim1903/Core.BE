using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Media.Domain.Models;
using Emeint.Core.BE.Media.Utilities;
using OpenHtmlToPdf;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using ImageMetaData = Emeint.Core.BE.Media.Domain.Models.ImageMetaData;

namespace Emeint.Core.BE.Media.Managers
{
    public class PdfManager : IPdfManager
    {
        private readonly IImageRepository _imageRepository;
        public PdfManager(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public PdfManager()
        {

        }
        public byte[] ConvertHtmlToPDF(string html)
        {
            return Pdf.From(html)
                     .OfSize(PaperSize.A4)
                     .WithMargins(0.Centimeters())
                     .Content();
        }
        public async Task<PdfDocument> ConvertImages(List<string> imagesCodes, string pdfName, bool deleteImagesAfterConverting)
        {
            if (imagesCodes != null && imagesCodes.Count > 0)
            {
                List<XImage> pdfImages = new List<XImage>();

                foreach (var imageCode in imagesCodes)
                {
                    ImageMetaData image = _imageRepository.GetImageByCode(imageCode);
                    string imagePath = ImagePathUtility.GetImageFilePath(image.ImagePath);
                    XImage pdfImage;
                    using (var fileStream = new FileStream(imagePath, FileMode.Open))
                    {
                        var memoryStream = new MemoryStream(image.ImageData.BinaryData);
                        memoryStream.CopyToAsync(fileStream).Wait();
                        //create a pdf Image from original image
                        pdfImage = XImage.FromStream(memoryStream);
                        pdfImages.Add(pdfImage);
                    }
                }
                var pdfDocument = ConvertToPdf(pdfImages, pdfName);
                if (deleteImagesAfterConverting && pdfDocument != null)
                {
                    // Delete Images from database according to business requirments
                    var deleteResult = await DeleteImages(imagesCodes);
                }
                return pdfDocument;
            }
            else
            {
                throw new MissingParameterException("Images Codes");

            }
        }
        public PdfDocument ConvertToPdf(List<XImage> images, string pdfName, int width = 600)
        {
            //this is used to configure encoding for .NETCore
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var document = new PdfDocument())
            {
                foreach (var image in images)
                {
                    PdfPage page = document.AddPage();
                    using (image)
                    {
                        // Calculate new height to keep image ratio
                        var height = (int)(((double)width / (double)image.PixelWidth) * image.PixelHeight);

                        // Change PDF Page size to match image
                        page.Width = width;
                        page.Height = height;

                        XGraphics gfx = XGraphics.FromPdfPage(page);
                        gfx.DrawImage(image, 0, 0, width, height);
                    }
                    //save the changes to the pdf document to a folder that must be created before saving.
                    Directory.CreateDirectory("./wwwroot/files/Convert");
                    document.Save($"wwwroot/files/Convert/{pdfName}.pdf");
                }
                return document;
            }
        }

        public async Task<bool> DeleteImages(List<string> imageCodes)
        {
            foreach (var imageCode in imageCodes)
            {
                if (_imageRepository.ImageCodeExists(imageCode))
                    await _imageRepository.DeleteByCode(imageCode);
            }
            return true;
        }
    }
}

