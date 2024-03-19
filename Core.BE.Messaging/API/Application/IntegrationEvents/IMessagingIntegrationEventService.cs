using Emeint.Core.BE.API.Application.IntegrationEvents;
using Emeint.Core.BE.Notifications.Infrastructure;


namespace Emeint.Core.BE.Notifications.API.Application.IntegrationEvents
{
    interface IMessagingIntegrationEventService : ICoreBEIntegrationEventService<MessagingContext>
    {
    }
}
