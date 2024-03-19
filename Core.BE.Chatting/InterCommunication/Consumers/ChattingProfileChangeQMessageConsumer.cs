using Emeint.Core.BE.Chatting.Infractructure.IRepositories;
using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.InterCommunication.Consumers
{
    public class ChattingProfileChangeQMessageConsumer : IInterCommMessageConsumer<ChattingUserProfileChangeQMessage>
    {
        private readonly IUserRepository _userRepository;
        public ChattingProfileChangeQMessageConsumer(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<ChattingUserProfileChangeQMessage> context)
        {
            var message = context.Message;
            _userRepository.UpdateRegisteredUser(message.UserId, message.UserName, null, message.ImageUrl);
        }
    }
}
