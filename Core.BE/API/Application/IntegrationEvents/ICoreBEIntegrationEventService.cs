using Microsoft.BuildingBlocks.EventBus.Events;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Emeint.Core.BE.API.Application.IntegrationEvents
{
    public interface ICoreBEIntegrationEventService<TContext> where TContext : DbContext
    {
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}