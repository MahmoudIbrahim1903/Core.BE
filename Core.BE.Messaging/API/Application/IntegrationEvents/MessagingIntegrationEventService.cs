using Emeint.Core.BE.API.Application.IntegrationEvents;
using Emeint.Core.BE.Notifications.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.BuildingBlocks.EventBus.Abstractions;
using Microsoft.BuildingBlocks.EventBus.Events;
using Microsoft.BuildingBlocks.IntegrationEventLogEF.Services;
using Microsoft.BuildingBlocks.IntegrationEventLogEF.Utilities;

using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.Application.IntegrationEvents
{
    public class MessagingIntegrationEventService : CoreBEIntegrationEventService<MessagingContext>, IMessagingIntegrationEventService
    {
        private readonly MessagingContext _messagingContext;

        public MessagingIntegrationEventService(IEventBus eventBus, MessagingContext context, Func<DbConnection,
            IIntegrationEventLogService> integrationEventLogServiceFactory)
            : base(eventBus, context, integrationEventLogServiceFactory)
        {
            _messagingContext = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
