using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.SeedWork;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Infrastructure.Repositories
{
    public class BaseCosmosRepository<TEntity> : IAsyncRepository<TEntity>
        where TEntity : Entity
    {
        protected CosmosClient _cosmosClient;
        protected string _containerName;
        protected Container _container;
        protected string _dataBaseName;
        protected readonly int? _maxConcurrencyLevel;
        protected readonly IConfiguration _configuration;
        public BaseCosmosRepository(CosmosClient cosmosClient, IConfiguration configuration, string containerName, IIdentityService identityService)
        {
            _cosmosClient = cosmosClient;
            _containerName = containerName;
            _configuration = configuration;
            //_dataBaseName = configuration["DataBaseName"];
            var queryConcurrencyLevel = configuration["CosmosQueryMaxConcurrency"];
            if (!string.IsNullOrEmpty(queryConcurrencyLevel))
                _maxConcurrencyLevel = Convert.ToInt32(queryConcurrencyLevel);

            SetCountry(identityService.CountryCode);
        }

        public void SetCountry(string countryCode)
        {
            _dataBaseName = _configuration.GetSection("CosmosDbNames").GetChildren().FirstOrDefault(ch => ch.Key == countryCode)?.Value;
            if (string.IsNullOrEmpty(_dataBaseName))
                _dataBaseName = _configuration.GetSection("CosmosDbNames").GetChildren().FirstOrDefault()?.Value;

            _container = _cosmosClient.GetDatabase(_dataBaseName).GetContainer(_containerName);
        }


        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var createdItem = await _container.CreateItemAsync(entity);
            return createdItem.Resource;
        }
        public virtual async Task<List<TEntity>> AddBulkAsync(List<TEntity> entities)
        {
            List<TEntity> addSuccessfully = new List<TEntity>();
            List<Task> concurrentTasks = new List<Task>(entities.Count);
            foreach (TEntity entity in entities)
            {
                concurrentTasks.Add(_container.CreateItemAsync(entity)
                          .ContinueWith(itemResponse =>
                          {
                              if (itemResponse.IsCompletedSuccessfully)
                                  addSuccessfully.Add(entity);
                          }));
            }
            await Task.WhenAll(concurrentTasks);
            return addSuccessfully;
        }
        public virtual async Task<List<TEntity>> UpdateBulkAsync(List<TEntity> entities)
        {
            List<TEntity> updateSuccessfully = new List<TEntity>();
            List<Task> concurrentTasks = new List<Task>(entities.Count);
            foreach (TEntity entity in entities)
            {
                concurrentTasks.Add(_container.ReplaceItemAsync(entity, (string)entity.GetType().GetProperty("id").GetValue(entity))
                          .ContinueWith(itemResponse =>
                          {
                              if (itemResponse.IsCompletedSuccessfully)
                                  updateSuccessfully.Add(entity);
                          }));
            }
            await Task.WhenAll(concurrentTasks);
            return updateSuccessfully;
        }
        public IQueryable<TEntity> GetAll()
        {
            QueryRequestOptions requestOptions = null;

            if (_maxConcurrencyLevel.HasValue)
                requestOptions = new QueryRequestOptions { MaxConcurrency = _maxConcurrencyLevel };

            return _container.GetItemLinqQueryable<TEntity>(allowSynchronousQueryExecution: true, requestOptions: requestOptions);
        }
        public async Task UpdateAsync(TEntity entity)
        {
            await _container.UpsertItemAsync(entity);
        }

        public Task<TEntity> GetAsync(int id)
        {
            throw new NotSupportedException();
        }

        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await _container.DeleteItemAsync<TEntity>(entity.Code, new PartitionKey(null));
        }

        public Task SaveEntities(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
