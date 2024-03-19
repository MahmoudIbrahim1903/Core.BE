using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Services.Serivces.Contracts
{
    public interface IAdminMessageService
    {

        List<AdminUserResponseViewModel> GetUsers(AdminGetUsersRequestViewModel criteria);
        PagedList<MessageDTO> GetMessages(PaginationVm pagination, SortingViewModel sorting);
        List<AdminMessageStatusResponseViewModel> GetMessageDetails(string messageCode);
        Task<Response<SystemEventNotificationDto>> Send(SendNotificationRequestDto message);
    }
}
