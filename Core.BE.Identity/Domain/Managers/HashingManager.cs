using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Enums;

namespace Emeint.Core.BE.Identity.Domain.Managers
{
    public class HashingManager : IHashingManager
    {
        private string secureKey = "kjsdfiusef;7yewrhas-cds:sqd";

        public string HashActivateOwnerVehiclesRequest(string ownerId)
        {
            if (string.IsNullOrEmpty(ownerId))
                return String.Empty;

            string messageToHash =
                 ownerId.ToString().Trim();

            return HashMessage(messageToHash);
        }

        public string HashCreateUserRequest(string accountNumber, string userRole)
        {
            if (string.IsNullOrEmpty(accountNumber) || string.IsNullOrEmpty(userRole))
                return String.Empty;

            string messageToHash =
                 accountNumber.ToString().Trim() +
               userRole.ToString().Trim();

            return HashMessage(messageToHash);
        }
        public string HashChangeUserSuspensionStatusRequest(string accountNumber, UserSuspensionStatus userSuspensionStatus)
        {
            if (string.IsNullOrEmpty(accountNumber))
                return String.Empty;

            string messageToHash =
                 accountNumber.ToString().Trim() + userSuspensionStatus.ToString().Trim();

            return HashMessage(messageToHash);
        }
        public string HashDeleteUserRequest(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
                return String.Empty;

            string messageToHash =
                 accountNumber.ToString().Trim();

            return HashMessage(messageToHash);
        }
        public string HashAddUserAccountRoleRequest(string accountNumber, string newRole)
        {
            if (string.IsNullOrEmpty(accountNumber) || string.IsNullOrEmpty(newRole))
                return String.Empty;

            string messageToHash =
                 accountNumber.ToString().Trim() +
                 newRole.Trim();

            return HashMessage(messageToHash);
        }

        //public string HashSendEmailRequest(SendMailViewModel mail)
        //{
        //    if (mail == null)
        //        return String.Empty;

        //    var messageToHash = mail.To[0].ToLower().Trim() + mail.From.ToLower().Trim();
        //    return HashMessage(messageToHash);
        //}

        //public string HashSendMessageRequest(AdminEventMessageViewModel serverVM)
        //{
        //    if (serverVM == null)
        //        return String.Empty;

        //    var messageToHash = serverVM.Alert.ToString().ToLower().Trim() +
        //        serverVM.User_Ids.Count().ToString().ToLower().Trim() +
        //        serverVM.User_Ids[0].ToString().ToLower().Trim();
        //    return HashMessage(messageToHash);
        //}


        //public string HashUpdateUserInfo(UserInfoViewModel userInfoViewModel)
        //{
        //    if (userInfoViewModel == null)
        //        return String.Empty;

        //    string messageToHash =
        //         userInfoViewModel.UserId.ToString().Trim() +
        //         userInfoViewModel.UserPhoneNumber?.ToString() ?? string.Empty;

        //    return HashMessage(messageToHash);
        //}

        //public string HashSendSmsRequest(SmsViewModel smsViewModel)
        //{
        //    if (smsViewModel == null)
        //        return String.Empty;

        //    string messageToHash =
        //         smsViewModel.PhoneNumbers.FirstOrDefault().ToString().Trim() +
        //         smsViewModel.SmsProvider.ToString().Trim();

        //    return HashMessage(messageToHash);
        //}

        private string HashMessage(string messageToHash)
        {
            return Emeint.Core.BE.Domain.Managers.HashingManager.HashMessage
                (messageToHash, new SHA256Managed(), secureKey);
        }


        public string HashUpdateUserName(string accountNumber, string userName)
        {
            if (string.IsNullOrEmpty(accountNumber) || string.IsNullOrEmpty(userName))
                return string.Empty;
            string msgToHash = accountNumber + userName;
            return HashMessage(msgToHash);
        }


        public string HashGetUserPaymentMethod(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return String.Empty;

            string messageToHash = userId.Trim();

            return HashMessage(messageToHash);
        }
    }
}
