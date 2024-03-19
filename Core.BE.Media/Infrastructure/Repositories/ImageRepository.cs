using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Media.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Emeint.Core.BE.Media.Infrastructure.Repositories
{
    public class ImageRepository
        : BaseRepository<ImageMetaData, MediaContext>, IImageRepository
    {
        public ImageRepository(MediaContext context) : base(context)
        {
            //   _context ?? throw new ArgumentNullException(nameof(context));
        }

        public ImageMetaData GetImageByCode(string code)
        {
            var entity = _context.Set<ImageMetaData>().Include(imd => imd.ImageData).FirstOrDefault(i => i.Code == code);

            return entity;
        }
        //public Image GetImageByImagePath(string path)
        //{
        //    var entity = _context.Set<Image>().FirstOrDefault(i => i.ImagePath == path);

        //    return entity;
        //}

        public async Task DeleteByCode(string code)
        {
            var imageMetaData = await _context.ImagesMetaData.FirstOrDefaultAsync(i => i.Code == code);

            if (imageMetaData != null)
                _context.Remove(imageMetaData);
            if (imageMetaData?.ImageDataId != null)
            {
                var imageData = await _context.ImagesData.FirstOrDefaultAsync(i => i.Id == imageMetaData.ImageDataId);
                _context.Remove(imageData);
            }
        }

        public bool ImageClientReferenceNumberExists(string clientReferenceNumber)
        {
            return _context.Set<ImageMetaData>().FirstOrDefault(m => m.ClientReferenceNumber == clientReferenceNumber) != null;
        }

        public async Task<ImageMetaData> AddAsync(ImageMetaData entity)
        {
            base.Add(entity);
            await SaveEntitiesAsync();
            return entity;
        }
        public bool ImageCodeExists(string code)
        {
            return _context.Set<ImageMetaData>().FirstOrDefault(img => img.Code == code) != null;
        }
        public void DeleteById(int id)
        {
            var dbSet = _context.Set<ImageMetaData>();
            var entity = dbSet.FirstOrDefault(i => i.Id == id);
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        async Task IAsyncRepository<ImageMetaData>.UpdateAsync(ImageMetaData entity)
        {
            base.Update(entity);
            await SaveEntitiesAsync();
        }

        async Task IAsyncRepository<ImageMetaData>.DeleteAsync(ImageMetaData entity)
        {
            base.Delete(entity);
            await SaveEntitiesAsync();
        }
    }
}