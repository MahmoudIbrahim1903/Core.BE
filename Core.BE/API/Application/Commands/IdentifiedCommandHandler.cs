using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Infrastructure.Idempotency;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.API.Application.Commands
{
    /// <summary>
    /// Provides a base implementation for handling duplicate request and ensuring idempotent updates, in the cases where
    /// a requestid sent by client is used to detect duplicate requests.
    /// </summary>
    /// <typeparam name="T">Type of the command handler that performs the operation if request is not duplicated</typeparam>
    /// <typeparam name="R">Return value of the inner command handler</typeparam>
    public class IdentifiedCommandHandler<T, R> : IRequestHandler<IdentifiedCommand<T, R>, R>
        where T : IRequest<R>
    {
        private readonly IMediator _mediator;
        private readonly ICommandManager _commandManager;

        public IdentifiedCommandHandler(IMediator mediator, ICommandManager commandManager)
        {
            _mediator = mediator;
            _commandManager = commandManager;
        }

        /// <summary>
        /// Creates the result value to return if a previous request was found
        /// </summary>
        /// <returns></returns>
        protected virtual R CreateResultForDuplicateRequest()
        {
            //return default(R);
            throw new DuplicateRequestException();
        }

        /// <summary>
        /// This method handles the command. It just ensures that no other request exists with the same ID, and if this is the case
        /// just enqueues the original inner command.
        /// </summary>
        /// <param name="message">IdentifiedCommand which contains both original command & request ID</param>
        /// <returns>Return value of inner command or default value if request same ID was found</returns>
        public async Task<R> Handle(IdentifiedCommand<T, R> message, CancellationToken cancellationToken)
        {
            var alreadyExists = await _commandManager.ExistAsync(message.Id);
            if (alreadyExists)
            {
                return CreateResultForDuplicateRequest();
            }
            else
            {
                string command = Newtonsoft.Json.JsonConvert.SerializeObject(message.Command);
                await _commandManager.CreateRequestForCommandAsync<T>(message.Id, command);

                // Send the embeded business command to mediator so it runs its related CommandHandler 
                var result = await _mediator.Send(message.Command);
                
                return result;
            }
        }
    }
}