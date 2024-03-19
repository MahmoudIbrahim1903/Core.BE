using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.Admin;
using Emeint.Core.BE.Notifications.API.Application.ValueObjects.DTOs;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Emeint.Core.BE.Notifications.Domain.Manager.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Notifications.Domain.Manager.Concretes
{
    public class UserService : IUserService
    {
        #region Fields
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly string _typeName;
        #endregion


        #region CTOR
        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IIdentityService identityService)
        {
            _userRepository = userRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identityService = identityService;
            _typeName = GetType().Name;
        }

        #endregion

        #region Methods 

        public UserDataDTO GetUserByuserId(string userId)
        {
            var user = _userRepository.GetUserByUserRegisterId(userId);

            if (user != null)
                return new UserDataDTO(user);

            return null;
        }

        public void UpdateUserInfo(string userId, string pushToken, string deviceId, string languageCode, Enums.Platform platform)
        {
            //Loading user to update its info 
            var user = _userRepository.GetUserByUserRegisterId(userId);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(pushToken))
                {
                    user.PushNotificationToken = pushToken;
                }
                if (!string.IsNullOrEmpty(deviceId))
                {
                    user.DeviceId = deviceId;
                }

                user.LanguageCode = languageCode;
                user.DevicePlatform = platform;
                user.ModificationDate = DateTime.UtcNow;
                _userRepository.Update(user);
                _userRepository.SaveEntities();
            }
        }


        public void AddNewUser(string userId, string name, string deviceId, string languageCode, string pushToken, UserDeviceDTO userDevice, Enums.Platform platform, string createdBy)
        {
            _userRepository.Add(new User(userId, null, deviceId, languageCode, pushToken, null, platform, createdBy));
            _userRepository.SaveEntities();
        }

        public void RemoveDuplicateTokens(string applicationUserId, string pushToken)
        {
            //loading user has the same token 
            var usersHaveTheSameTokens = _userRepository.GetUserByToken(pushToken);
            if (usersHaveTheSameTokens != null && usersHaveTheSameTokens.Count > 0)
            {
                foreach (var user in usersHaveTheSameTokens)
                {
                    if (user.ApplicationUserId != applicationUserId)
                    {
                        //empty push token with all users except cuurnt user 
                        //validating that only one token per user <Last logged in user should recieve notification others can pull>
                        user.PushNotificationToken = string.Empty;
                        _userRepository.Update(user);
                        _userRepository.SaveEntities();
                    }
                }
            }
        }

        public void UnRegisterUserFromPushToken(string userId)
        {
            var user = _userRepository.GetUserByUserRegisterId(userId);
            if (user != null)
            {
                user.PushNotificationToken = string.Empty;
                _userRepository.Update(user);
                _userRepository.SaveEntities();
            }
        }

        public List<UserDataDTO> GetAllUsers()
        {
            var allUsers = _userRepository.GetAll();
            var usersDTOs = new List<UserDataDTO>();
            if (allUsers != null && allUsers.Count() > 0)
            {
                foreach (var user in allUsers)
                {
                    usersDTOs.Add(new UserDataDTO(user));
                }
            }

            return usersDTOs;
        }

        public List<AdminUserResponseViewModel> GetUsers(AdminGetUsersRequestViewModel criteria)
        {
            List<AdminUserResponseViewModel> usersVm = new List<AdminUserResponseViewModel>();
            var users = _userRepository.GetUsers(criteria);
            if (users != null && users.Count > 0)
            {
                users.Select(m => new AdminUserResponseViewModel
                {
                    UserId = m.ApplicationUserId,
                    LanguageCode = m.LanguageCode,
                    Platform = m.DevicePlatform,
                    Name = m.Name
                })
                .ToList();
            }

            return usersVm;
        }
        public async Task CreateOrUpdateUser(string userId, string platformHeader, string pushToken, string deviceId, string language_code = "en")
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            _logger.LogInformation($"{_typeName}.{methodName}: Saving user with platform in database, from: " + userId);

            //loading user by id if exists 
            var user = GetUserByuserId(userId);

            var platform = GetPlatform(platformHeader);
            //Checking if user not null update its info 
            if (user != null)
            {
                //delegating to user manager 
                UpdateUserInfo(user.ApplicationUserId, pushToken, deviceId, language_code, platform);
            }
            else
            {
                //user does not exists 
                //inserting new one using user odentifier in the app (emai , phonenumber, or code/id)
                string createdBy = userId ?? "System";

                if (String.IsNullOrEmpty(createdBy))
                    throw new MissingParameterException("user_name");

                AddNewUser(userId, null, deviceId, language_code, pushToken, null, platform, createdBy);
            }

            //checking push token dublication 
            if (!string.IsNullOrEmpty(pushToken))
                RemoveDuplicateTokens(userId, pushToken);

        }

        #endregion

        #region HelperMethods 
        private Enums.Platform GetPlatform(string platformHeader)
        {
            //Maping platforms 
            Enums.Platform platform =
                !string.IsNullOrEmpty(platformHeader) ?
                platformHeader switch
                {
                    "android" => Enums.Platform.Android,
                    "ios" => Enums.Platform.Ios,
                    "web" => Enums.Platform.WebBrowser,
                    _ => throw new InvalidParameterException("Platform", platformHeader),
                }
                : throw new MissingParameterException("platform");
            return platform;
        }
        #endregion
    }
}
