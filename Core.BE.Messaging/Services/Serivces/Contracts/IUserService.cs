using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs;
using Emeint.Core.BE.Notifications.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Manager.Contracts
{
    public interface IUserService
    {
        UserDataDTO GetUserByuserId(string userId);
        void UpdateUserInfo(string userId, string pushToken, string deviceId, string language_code, Enums.Platform platform);
        void AddNewUser(string userId, string name, string deviceId, string languageCode, string pushToken, UserDeviceDTO userDevice, Enums.Platform platform, string createdBy);
        void RemoveDuplicateTokens(string applicationUserId, string pushToken);
        void UnRegisterUserFromPushToken(string userId);
        List<UserDataDTO> GetAllUsers();
        List<AdminUserResponseViewModel> GetUsers(AdminGetUsersRequestViewModel criteria);
        Task CreateOrUpdateUser(string userId, string platform, string pushToken, string deviceId, string languageCode);

    }
}
