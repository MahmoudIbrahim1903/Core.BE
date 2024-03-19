using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Media.Domain.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Infrastructure.Repositories
{
    public class DocumentCosmosRepository : BaseCosmosRepository<Domain.Models.Document>, IDocumentRepository
    {
        public DocumentCosmosRepository(CosmosClient cosmosClient, IConfiguration configuration, IIdentityService identityService) : base(cosmosClient, configuration, "Documents", identityService)
        {
        }

        public async Task DeleteDocumentByCode(string documentCode, string deletedBy)
        {
            var document = GetAll().Where(d => d.Code == documentCode && !d.IsDeleted).ToList()?.FirstOrDefault();
            if (document != null) { 
            document.DeletedBy = deletedBy;
            document.DeletedDate = DateTime.UtcNow;
            document.IsDeleted = true;
            }
            await UpdateAsync(document);
        }

        public Domain.Models.Document GetDocumentByCode(string documentCode)
        {
            return GetAll().Where(d => d.Code == documentCode && !d.IsDeleted).ToList()?.FirstOrDefault();

        }
    }
}
