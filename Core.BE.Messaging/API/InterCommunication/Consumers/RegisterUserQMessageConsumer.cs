using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using Emeint.Core.BE.Notifications.Domain.Manager.Contracts;
using MassTransit;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.API.InterCommunication.Consumers
{
    public class RegisterUserQMessageConsumer : IInterCommMessageConsumer<UserMessagingChangeQMessage>
    {
        private readonly IUserService _userService;

        public RegisterUserQMessageConsumer(IUserService userService)
        {
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<UserMessagingChangeQMessage> context)
        {
            var message = context.Message;
            await _userService.CreateOrUpdateUser(message.ApplicationUserId, message.Platform, null, null, message.LanguageCode);
        }
    }
}
