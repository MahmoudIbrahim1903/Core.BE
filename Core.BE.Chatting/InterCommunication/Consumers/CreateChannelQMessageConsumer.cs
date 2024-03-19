using Emeint.Core.BE.Chatting.Application.Dtos.SendbirdDtos;
using Emeint.Core.BE.Chatting.Domain.ChattingProviders;
using Emeint.Core.BE.InterCommunication.Contracts;
using Emeint.Core.BE.InterCommunication.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.InterCommunication.Consumers
{
    public class CreateChannelQMessageConsumer : IInterCommMessageConsumer<CreateChannelQMessage>
    {
        private readonly IChatProvider _chatProvider;
        private readonly ILogger<CreateChannelQMessageConsumer> _logger;
        public CreateChannelQMessageConsumer(IChatProvider chatProvider, ILogger<CreateChannelQMessageConsumer> logger)
        {
            _chatProvider = chatProvider;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<CreateChannelQMessage> context)
        {
            var message = context.Message;

            _logger.LogInformation($"Start creating channel between {string.Join(",", message.ChannelMembers)} -  retry count : {context.GetRetryAttempt()}");

            if (message.ChannelMembers != null && message.ChannelMembers.Count() > 0)
            {
                foreach (var member in message.ChannelMembers)
                {
                    await _chatProvider.RegisterUser(member.MemberId, member.MemberName, member.MemberType, member.MemberImageUrl);
                }

                await _chatProvider.CreateChannel(message.ChannelMembers.Select(m => new ChannelMemberDto
                {
                    Id = m.MemberId,
                    Name = m.MemberName
                }).ToList());
            }
        }
    }
}