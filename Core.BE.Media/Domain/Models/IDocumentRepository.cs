using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Models
{
    public interface IDocumentRepository : IAsyncRepository<Document>
    {
        Document GetDocumentByCode(string documentCode);
        Task DeleteDocumentByCode(string documentCode, string deletedBy);
    }
}
