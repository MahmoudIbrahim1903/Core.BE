using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Server;
using Emeint.Core.BE.Notifications.Domain.Manager.Contracts;
using Emeint.Core.BE.Notifications.Services.Serivces.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Services.Serivces.Concretes
{
    public class AdminMessageService : IAdminMessageService
    {
        private readonly IMessageAggregateService _messageAggregateService;
        public AdminMessageService(IMessageAggregateService messageAggregateService)
        {
            _messageAggregateService = messageAggregateService;
        }
        public List<AdminUserResponseViewModel> GetUsers(AdminGetUsersRequestViewModel criteria)
        {
            return _messageAggregateService.GetUsers(criteria);
        }
        public PagedList<MessageDTO> GetMessages(PaginationVm pagination, SortingViewModel sorting)
        {
            return _messageAggregateService.GetAdminMessages(pagination, sorting);
        }
        public List<AdminMessageStatusResponseViewModel> GetMessageDetails(string messageCode)
        {
            return _messageAggregateService.GetMessageDetails(messageCode);
        }
        public async Task<Response<SystemEventNotificationDto>> Send(SendNotificationRequestDto message)
        {
            return await _messageAggregateService.Send(message);
        }
    }
}
