using System;
using System.IO;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Media.Utilities;
using Microsoft.AspNetCore.Http;
//using SkiaSharp;

namespace Emeint.Core.BE.Media.Domain.Models
{
    public class ImageMetaData
        : Entity
    {
        protected ImageMetaData()
        {
        }

        public ImageMetaData(IFormFile image, string clientReferenceNumber, string tenantCode, string createdBy)
        {
            if (image != null)
            {
                id = Guid.NewGuid().ToString();
                var ext = ImagePathUtility.GetFileExtension(image.FileName);
                Code = $"IMG_{DateTime.UtcNow.ToString($"yyyy-MM-dd-HH_mmss{new Random().Next(1, 100000)}")}{ext}";
                CreatedBy = createdBy;
                CreationDate = DateTime.UtcNow;
                ClientReferenceNumber = clientReferenceNumber;
                Name = image.FileName;
                //ImagePath = $"{Code}{ext}";
                ImagePath = Code;
                TenantCode = tenantCode;
                OriginalBinaryDataLength = (int)image.Length;

                //Set image binary data
                using (var memoryStream = new MemoryStream())
                {
                    image.CopyToAsync(memoryStream).Wait();
                    ImageData = new ImageData(memoryStream.ToArray(), createdBy);
                }

                //Set image width and hight
                //using (var imageFromStream = System.Drawing.Image.FromStream(image.OpenReadStream()))
                //{
                //    Width = imageFromStream.Width;
                //    Height = imageFromStream.Height;
                //}
            }
        }

        public string id { set; get; }
        public string ClientReferenceNumber { get; set; }
        public string ImagePath { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string TenantCode { get; set; }
        public int? OriginalBinaryDataLength { get; set; }
        public int? ImageDataId { set; get; }
        public virtual ImageData ImageData { set; get; }
        //public string ScalingMode { get; set; }
    }
}
