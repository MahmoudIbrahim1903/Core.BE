using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Media.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Infrastructure.Repositories
{
    public class DocumentRepository : BaseRepository<Document, MediaContext>, IDocumentRepository
    {
        public DocumentRepository(MediaContext context) : base(context)
        {
        }

        public async Task<Document> AddAsync(Document entity)
        {
            var document = base.Add(entity);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task DeleteDocumentByCode(string documentCode, string deletedBy)
        {
            var document = await _context.Documents.FirstOrDefaultAsync(d => d.Code == documentCode);
            document.DeletedBy = deletedBy;
            document.DeletedDate = DateTime.UtcNow;
            document.IsDeleted = true;
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
        }

        public Document GetDocumentByCode(string documentCode)
        {
            return _context.Documents.FirstOrDefault(d => d.Code == documentCode && !d.IsDeleted);
        }

        async Task IAsyncRepository<Document>.DeleteAsync(Document entity)
        {
            base.Delete(entity);
            await _context.SaveChangesAsync();
        }

        async Task IAsyncRepository<Document>.UpdateAsync(Document entity)
        {
            base.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
