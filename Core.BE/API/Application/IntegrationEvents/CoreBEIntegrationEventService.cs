using Microsoft.BuildingBlocks.EventBus.Abstractions;
using Microsoft.BuildingBlocks.EventBus.Events;
using Microsoft.BuildingBlocks.IntegrationEventLogEF.Services;
using Microsoft.BuildingBlocks.IntegrationEventLogEF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Emeint.Core.BE.API.Application.IntegrationEvents
{
    public class CoreBEIntegrationEventService<TContext> : ICoreBEIntegrationEventService<TContext> where TContext: DbContext
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly TContext _context;
        private readonly IIntegrationEventLogService _eventLogService;

        public CoreBEIntegrationEventService(IEventBus eventBus, TContext context,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_context.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            await SaveEventAndContextChangesAsync(evt);
            _eventBus.Publish(evt);
            await _eventLogService.MarkEventAsPublishedAsync(evt);
        }

        private async Task SaveEventAndContextChangesAsync(IntegrationEvent evt)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_context)
                .ExecuteAsync(async () => {
                    // Achieving atomicity between original ordering database operation and the IntegrationEventLog thanks to a local transaction
                    await _context.SaveChangesAsync();
                    await _eventLogService.SaveEventAsync(evt, _context.Database.CurrentTransaction.GetDbTransaction());
                });
        }
    }
}