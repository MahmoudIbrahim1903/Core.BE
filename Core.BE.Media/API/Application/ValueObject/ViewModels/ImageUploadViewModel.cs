using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels
{
    [DataContract]
    public class ImageUploadViewModel
    {
        //[DataMember]
        //public string ImageCode { get; }
        [DataMember]
        [Required]
        public string ClientReferenceNumber { get; }

        [DataMember]
        [Required]
        public IFormFile Image { get; set; }

        //[DataMember]
        //public int? Width { get; }
        //[DataMember]
        //public int? Height { get; }
        //[DataMember]
        //public string Url { get; set; }

        //public ImageUploadViewModel(string image)
        //{
        //    //if (IsUrl(image) == true)
        //    //{
        //    //    Url = image;
        //    //}
        //    //else
        //    //{
        //    //    ImageCode = image;
        //    //    //TODO: construct URL using ImageId
        //    //}
        //}

        ////private bool IsUrl(string image)
        ////{
        ////    if (image != null && image.Contains("http"))
        ////    {
        ////        return true;
        ////    }

        ////    return false;
        ////}

    }
}