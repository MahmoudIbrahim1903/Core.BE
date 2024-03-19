using System.IO;
using Emeint.Core.BE.Media.API.Application.ValueObject.Dtos;
using Emeint.Core.BE.Media.API.Application.ValueObject.ViewModels;
using Emeint.Core.BE.Media.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Managers
{
    public interface IMediaStorageManager
    {
        Task<string> UploadImageAsync(byte[] binaryData, string imagePath, ImageMetaData imageMetaData = null);
        Task<VideoViewModel> UploadVideoAsync(IFormFile formFile);
        Task<bool> ImageExistsAsync(string azureFilePath);
        Task<System.Drawing.Image> ReadImageAsync(string imagePath);
        Task WriteToResponse(RequestDelegate requestDelegate, HttpContext context, string path);
        Task DeleteImageAsync(string imageCode);
        Task<VideoDto> GetVideoAsync(string videoCode);
        Task<string> GetVideoUrlAsync(string videoPath, string containerName, string accountName, string accountKey);
        Task DeleteVideoAsync(string videoCode);
        Task<DocumentViewModel> UploadDocumentAsync(IFormFile document);
        Task<string> UploadExcelDocumentAsync(FileInfo file, string contentType);
        Task<DocumentViewModel> GetDocumentAsync(string documentCode);
        Task DeleteDocumentAsync(string documentCode, string deletedBy);
        string GetVideoNameByCode(string videoCode);
        string GetDocumentNameByCode(string documentCode);
        Task<AudioViewModel> UploadAudioAsync(IFormFile formFile);
        Task<AudioViewModel> GetAudioAsync(string audioCode);
    }
}
