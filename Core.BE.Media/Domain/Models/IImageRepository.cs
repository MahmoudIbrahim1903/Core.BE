using Emeint.Core.BE.Domain.SeedWork;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Models
{
    public interface IImageRepository :
        IAsyncRepository<ImageMetaData>
    {
        ImageMetaData GetImageByCode(string code);
        //Image GetImageByImagePath(string path);
        Task DeleteByCode(string code);
        void DeleteById(int id);
        bool ImageClientReferenceNumberExists(string clientReferenceNumber);
        bool ImageCodeExists(string code);
    }
}