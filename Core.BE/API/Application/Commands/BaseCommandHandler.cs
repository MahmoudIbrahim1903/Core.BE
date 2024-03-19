//namespace Emeint.Core.BE.API.Application.Commands
//{
//    using Domain.AggregatesModel.OrderAggregate;
//    using MediatR;
//    using Emeint.Core.BE.API.Infrastructure.Services;
//    using Emeint.Core.BE.Infrastructure.Idempotency;
//    using System;
//    using System.Threading.Tasks;

//    // Regular CommandHandler
//    public class BaseCommandHandler
//        : IAsyncRequestHandler<BaseCommand, bool>
//    {
//        //private readonly IOrderRepository _orderRepository;
//        //private readonly IIdentityService _identityService;
//        private readonly IMediator _mediator;

//        // Using DI to inject infrastructure persistence Repositories
//        public BaseCommandHandler(IMediator mediator)
//        {
//            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
//        }

//        public async Task<bool> Handle(BaseCommand message)
//        {
//            // Add/Update the Buyer AggregateRoot
//            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
//            // methods and constructor so validations, invariants and business logic 
//            // make sure that consistency is preserved across the whole aggregate

//            return await new Task<bool>();
//        }
//    }


//    // Use for Idempotency in Command process
//    //public class CreateOrderIdentifiedCommandHandler : IdentifiedCommandHandler<CreateOrderCommand, bool>
//    //{
//    //    public CreateOrderIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
//    //    {
//    //    }

//    //    protected override bool CreateResultForDuplicateRequest()
//    //    {
//    //        return true;                // Ignore duplicate requests for creating order.
//    //    }
//    //}
//}
