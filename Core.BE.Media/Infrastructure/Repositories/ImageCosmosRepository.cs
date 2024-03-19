using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Media.Domain.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Infrastructure.Repositories
{
    public class ImageCosmosRepository : BaseCosmosRepository<ImageMetaData>, IImageRepository
    {

        public ImageCosmosRepository(CosmosClient cosmosClient, IConfiguration configuration, IIdentityService identityService) : base(cosmosClient, configuration, "ImagesMetaData", identityService)
        {
        }

        public ImageMetaData GetImageByCode(string code)
        {
            var entity = GetAll().Include(imd => imd.ImageData).FirstOrDefault(i => i.Code == code);

            return entity;
        }
        //public Image GetImageByImagePath(string path)
        //{
        //    var entity = _context.Set<Image>().FirstOrDefault(i => i.ImagePath == path);

        //    return entity;
        //}

        public async Task DeleteByCode(string code)
        {
            var imageMetaData = GetAll().FirstOrDefault(i => i.Code == code);

            if (imageMetaData != null)
                await DeleteAsync(imageMetaData);
        }

        public bool ImageClientReferenceNumberExists(string clientReferenceNumber)
        {
            return GetAll().Where(m => m.ClientReferenceNumber == clientReferenceNumber).ToList().Any();
        }

        public bool ImageCodeExists(string code)
        {
            return GetAll().FirstOrDefault(img => img.Code == code) != null;
        }
        public void DeleteById(int id)
        {
            throw new NotSupportedException();
        }
    }
}