using Emeint.Core.BE.Chatting.Application.Dtos.SendbirdDtos;
using Emeint.Core.BE.Chatting.Domain.Configurations;
using Emeint.Core.BE.Chatting.Infractructure.IRepositories;
using Emeint.Core.BE.Chatting.Infractructure.Model;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Domain.ChattingProviders
{
    public class SendbirdProvider : IChatProvider
    {
        private readonly IChannelRepository _channelRepository;
        private readonly IWebRequestUtility _webRequestUtility;
        private readonly IConfigurationManager _configurationManager;
        private readonly IUserRepository _userRepository;

        public SendbirdProvider(IChannelRepository channelRepository, IWebRequestUtility webRequestUtility, IConfigurationManager configurationManager, IUserRepository userRepository)
        {
            _channelRepository = channelRepository;
            _webRequestUtility = webRequestUtility;
            _configurationManager = configurationManager;
            _userRepository = userRepository;
        }

        public async Task CreateChannel(List<ChannelMemberDto> channelMembers)
        {
            //TODO: use automapper
            List<ChannelMember> membersList = channelMembers.Select(m =>
            new ChannelMember
            {
                UserId = m.Id,
                CreatedBy = "System",
                CreationDate = DateTime.UtcNow
            }).ToList();

            bool isExistChannelId = _channelRepository.GetChannel(membersList) != null;
            if (!isExistChannelId)
            {
                string sendbirdApiToken = _configurationManager.GetSendbirdToken();
                string sendbirdAppId = _configurationManager.GetSendbirdAppId();
                Random rnd = new Random();
                CreateSendbirdChannelRequestDto sendbirdCreateChannelVM = new CreateSendbirdChannelRequestDto()
                {
                    ChannelUrl = $"{string.Join('-', channelMembers.Select(m => m.Id.Substring(0, 5)).ToList())}-{rnd.Next().ToString()}",
                    UserIds = channelMembers.Select(m => m.Id).ToList()
                };

                var header = new List<KeyValuePair<string, string>>();
                header.Add(new KeyValuePair<string, string>("Api-Token", sendbirdApiToken));

                string uri = $"https://api-{sendbirdAppId}.sendbird.com/v3/group_channels";
                var httpRequest = new Emeint.Core.BE.Domain.SeedWork.HttpPostRequest(uri, sendbirdCreateChannelVM, "application/json", header, JsonNamingStrategy.SnakeCase);
                var response = await _webRequestUtility.Post<object>(httpRequest, JsonNamingStrategy.SnakeCase);

                _channelRepository.CreateChannel(membersList, sendbirdCreateChannelVM.ChannelUrl);
            }
        }

        public Task DeleteChannel(string channelId)
        {
            // TO DO
            throw new NotImplementedException();
        }

        public Task DeleteUser(string userId)
        {
            // TO DO
            throw new NotImplementedException();
        }

        public async Task RegisterUser(string userId, string name, string type, string imageUrl)
        {
            string sendbirdApiToken = _configurationManager.GetSendbirdToken();
            string sendbirdAppId = _configurationManager.GetSendbirdAppId();

            var header = new List<KeyValuePair<string, string>>();
            header.Add(new KeyValuePair<string, string>("Api-Token", sendbirdApiToken));

            var isRegistered = _userRepository.IsRegistered(userId);

            if (!isRegistered)
            {
                string getUserUri = $"https://api-{sendbirdAppId}.sendbird.com/v3/users";
                var getUserHttpRequest = new Emeint.Core.BE.Domain.SeedWork.HttpGetRequest(getUserUri, new { user_ids = userId }, header);
                var getUserResponse = await _webRequestUtility.Get<GetSendbirdUsersResponseDto>(getUserHttpRequest, JsonNamingStrategy.PascalCase);

                if (getUserResponse.Users.Count == 0)
                {

                    string rgisterUri = $"https://api-{sendbirdAppId}.sendbird.com/v3/users";
                    var rgisterHttpRequest = new Emeint.Core.BE.Domain.SeedWork.HttpPostRequest(rgisterUri, new RegisterSendbirdUserRequestDto() { UserId = userId, Nickname = name, ProfileUrl = null }, "application/json", header, JsonNamingStrategy.SnakeCase);
                    var registerResponse = await _webRequestUtility.Post<object>(rgisterHttpRequest, JsonNamingStrategy.SnakeCase);
                }

                _userRepository.RegisterUser(userId, name, type, imageUrl);
            }
            else
            {
                _userRepository.UpdateRegisteredUser(userId, name, type, imageUrl);
            }
        }
    }
}