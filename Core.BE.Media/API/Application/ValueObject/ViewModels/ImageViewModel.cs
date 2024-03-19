using System.Linq;
using System.Runtime.Serialization;
using Emeint.Core.BE.Media.Domain.Models;
using Emeint.Core.BE.Media.Utilities;

namespace Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels
{
    [DataContract]
    public class ImageViewModel
    {
        //private const string _imagesRootFolder = "/imgapi/"; // TODO: Get from settings

        [DataMember]
        public string ImageCode { get; set; }
        //[DataMember]
        //public string ClientReferenceNumber { get; }
        ////[DataMember]
        ////public IFormFile Image { get; set; }
        //[DataMember]
        //public int? Width { get; }
        //[DataMember]
        //public int? Height { get; }
        [DataMember]
        public string Url { get; set; }

        public ImageViewModel()
        {

        }
        public ImageViewModel(string imageCodeOrUrl, string imagesRootFolder, bool buildAzureStorageFullUrl, string azureStorageBasePath, string container)
        {
            if (!string.IsNullOrEmpty(imageCodeOrUrl))
            {
                if (ImagePathUtility.IsAbsoluteUrl(imageCodeOrUrl) || (ImagePathUtility.IsRelativeUrl(imageCodeOrUrl)) )
                {
                    ImageCode = imageCodeOrUrl.Split('/').Last();
                    //string y = imageCodeOrUrl.Substring(imageCodeOrUrl.LastIndexOf('/') + 1);
                    Url = imageCodeOrUrl;
                }

                else
                {
                    ImageCode = imageCodeOrUrl;
                    Url = imagesRootFolder + ImageCode;
                    //Image image = GetImageByCode(imageCode);
                    //ImageCode = image.Code;
                    //ClientReferenceNumber = image.ClientReferenceNumber;
                    //Height = image.Height;
                    //Width = image.Width;
                    //Url = $"/api/image/download_image?image_path={imageCodeOrUrl}"; // TODO: review

                    //Url = _imagesFolder + tenantCode + "/" + imageCodeOrUrl; TODO: Add tenant
                }

                if (!string.IsNullOrEmpty(azureStorageBasePath) && !string.IsNullOrEmpty(container) && buildAzureStorageFullUrl)
                { 
                   string path = ImagePathUtility.GetImageQueryPath(Url);
                   Url = $"{azureStorageBasePath}{container}{path}";
                }
            }
        }

        public ImageViewModel(ImageMetaData image, string imagesRootFolder)
        {
            ImageCode = image.Code;
            //ClientReferenceNumber = image.ClientReferenceNumber;
            //Height = image.Height;
            //Width = image.Width;
            Url = imagesRootFolder + image.Code;

            //if (IsUrl(image) == true)
            //{
            //    Url = image;
            //}
            //else
            //{
            //    ImageCode = image;
            //    //TODO: construct URL using ImageId
            //}
        }
    }
}