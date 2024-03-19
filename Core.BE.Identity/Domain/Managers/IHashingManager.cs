using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Managers
{
    public interface IHashingManager
    {
        string HashChangeUserSuspensionStatusRequest(string accountNumber, UserSuspensionStatus userSuspensionStatus);
        string HashCreateUserRequest(string accountNumber, string userRole);
        string HashDeleteUserRequest(string accountNumber);
        //string HashSendEmailRequest(SendMailViewModel mail);
        //string HashSendMessageRequest(AdminEventMessageViewModel serverVM);
        //string HashActivateOwnerVehiclesRequest(string ownerId);
        //string HashUpdateUserInfo(UserInfoViewModel userInfoViewModel);
        string HashAddUserAccountRoleRequest(string accountNumber, string newRole);
        //string HashSendSmsRequest(SmsViewModel smsViewModel);
        string HashUpdateUserName(string accountNumber, string userName);
        string HashGetUserPaymentMethod(string userId);
    }
}
