using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Admin;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel;
using Emeint.Core.BE.Identity.Domain.Configurations;
using Emeint.Core.BE.Identity.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Repositories;
using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts;
using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts.InterCommunication;
using Emeint.Core.BE.InterCommunication.Messages;
using Emeint.Core.BE.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Concretes
{
    public class AdminsService : IAdminsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAdminsRepository _applicationUserRepository;
        private readonly ISendEmailNotifierService _sendEmailNotifierService;
        private readonly IIdentityConfigurationManager _configurationManager;
        private readonly IAccountService _accountService;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        #region Generate Password Options
        int _passwordMinLength = 8;
        int _passwordCharGroupLength = 1;
        bool _passwordRequireNumber = true;
        bool _passwordRequireUpperCase = true;
        bool _passwordRequireSpecialChar = false;
        #endregion

        public AdminsService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IAdminsRepository applicationUserRepository, ISendEmailNotifierService sendEmailNotifierService, IAccountService accountService,
            IIdentityConfigurationManager configurationManager, IIdentityService identityService, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationUserRepository = applicationUserRepository;
            _sendEmailNotifierService = sendEmailNotifierService;
            _configurationManager = configurationManager;
            _accountService = accountService;
            _identityService = identityService;
            _configuration = configuration;
        }
        public async Task<PagedList<AdminVM>> GetAdmins(string name, PaginationVm pagination, AdminsSortBy sortBy, SortDirection direction, string country)
        {
            var admins = _applicationUserRepository.GetAdminsByCriteria(name, pagination, sortBy, direction, country);
            List<AdminVM> result = new List<AdminVM>();
            foreach (var admin in admins.List)
            {
                var roleNames = _userManager.GetRolesAsync(admin).Result.ToList();
                result.Add(new AdminVM
                {
                    UserId = admin.Id,
                    IsActive = admin.SuspensionStatus == UserSuspensionStatus.Active,
                    Email = admin.Email,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    PhoneNumber = admin.PhoneNumber,
                    CreationDate = admin.RegistrationDate.ToString("yyyyMMddHHmmss"),
                    Roles = roleNames,
                    LastLoginDate = admin.LastLoginDate?.ToString("yyyyMMddHHmmss"),
                });
            }
            return new PagedList<AdminVM> { List = result, TotalCount = admins.TotalCount };
        }


        public async Task<bool> AddAdmin(AddAdminVM adminReq)
        {
            var registerRequest = new RegisterViewModel
            {
                CountryCode = "EGY",
                UserName = adminReq.Email ?? adminReq.PhoneNumber,
                UserRoles = adminReq.Roles,
                FirstName = adminReq.FirstName,
                LastName = adminReq.LastName,
                Email = adminReq.Email,
                PhoneNumber = adminReq.PhoneNumber,
                Platform = Domain.Enums.Platform.portal,
                Password = GeneratePasswordUtility.Generate(_passwordMinLength, _passwordCharGroupLength, _passwordRequireUpperCase, _passwordRequireSpecialChar, _passwordRequireNumber)
            };
            var newUser = await _accountService.CreateUser(registerRequest);
            if (newUser.IsSucceeded)
            {
                //send email
                var user = _applicationUserRepository.GetById(newUser.Data.UserId);
                AddUpdateAdminSendEmail(user, registerRequest.Password, true);
                _accountService.AddPermittedCountryForUser(newUser.Data.UserId, _identityService.CountryCode, _identityService.DisplayName);
            }
            return newUser.IsSucceeded;

        }

        public async Task<bool> UpdateAdmin(UpdateAdminVM adminReq)
        {
            var admin = _userManager.FindByIdAsync(adminReq.UserId).Result;
            //validate existance email and phone number
            if (adminReq.PhoneNumber != null)
            {
                List<ApplicationUser> usersWithSamePhoneNumber = _userManager.Users.Where(u => u.PhoneNumber == adminReq.PhoneNumber && u.Id != adminReq.UserId).ToList();
                if (usersWithSamePhoneNumber != null && usersWithSamePhoneNumber.Count > 0 && usersWithSamePhoneNumber.Any(u => u.PhoneNumberConfirmed))
                    throw new PhoneNumberAlreadyExistsException();
            }
            if (adminReq.Email != null)
            {
                EmailUtility.ValidateEmail(adminReq.Email);
                ApplicationUser existingApplicationUserByEmail = _userManager.Users.FirstOrDefault(u => u.Email == adminReq.Email && u.Id != adminReq.UserId);
                if (existingApplicationUserByEmail != null)
                    throw new EmailAlreadyExistsException();
            }
            var currentRoles = await _userManager.GetRolesAsync(admin);

            if (adminReq.Roles == null || adminReq.Roles.Count == 0)
                throw new MissingParameterException("UpdateAdminVM.Roles");

            if (!currentRoles.All(r => adminReq.Roles.Contains(r)) || !adminReq.Roles.All(r => currentRoles.Contains(r)))
            {
                await _userManager.RemoveFromRolesAsync(admin, currentRoles);
                await _userManager.AddToRolesAsync(admin, adminReq.Roles);
                _accountService.RemoveAllPersistedGrants(admin.Id);
            }

            admin.FirstName = adminReq.FirstName;
            admin.LastName = adminReq.LastName;
            admin.UserName = adminReq.Email ?? adminReq.PhoneNumber;

            if (admin.Email != adminReq.Email)
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(admin);
                if (removePasswordResult.Succeeded)
                {
                    string newPassword = GeneratePasswordUtility.Generate(_passwordMinLength, _passwordCharGroupLength, _passwordRequireUpperCase, _passwordRequireSpecialChar, _passwordRequireNumber);
                    var addPasswordResult = await _userManager.AddPasswordAsync(admin, newPassword);
                    if (addPasswordResult.Succeeded)
                    {
                        admin.Email = adminReq.Email;
                        AddUpdateAdminSendEmail(admin, newPassword, false);
                    }
                }
            }
            admin.PhoneNumber = adminReq.PhoneNumber;

            //suspend user suspension status is changed
            if (!adminReq.IsActive && admin.SuspensionStatus == UserSuspensionStatus.Active)
                await _accountService.SuspendAccount(admin, _identityService.DisplayName, SuspensionSource.Admin, "");

            //Activate user suspension status is changed
            if (adminReq.IsActive && admin.SuspensionStatus == UserSuspensionStatus.Suspended)
                await _accountService.ActivateAccount(admin, _identityService.DisplayName);

            var result = await _userManager.UpdateAsync(admin);
            return result.Succeeded;
        }

        public async Task<List<string>> GetAdminRoles()
        {
            var roles = _roleManager.Roles.Where(r => r.IsAdmin && r.Name != "SuperAdmin").Select(r => r.Name).OrderBy(n => n).ToList();
            return roles;
        }



        #region Helpers
        private async Task AddUpdateAdminSendEmail(ApplicationUser admin, string password, bool isNew)
        {
            EmailQMessage emailQMessage;
            EmailVerification emailVerification = _configurationManager.GetAccountVerificationEmailVerification();
            if (emailVerification == EmailVerification.SignUp)
            {
                //send email with verify link 
                var emailVerificationCode = _accountService.GetEmailVerificationCode(admin).Result;
                var callbackurl = $"{_configurationManager.GetServerBaseUrl()}api/identity/account/v1/verify_email?user_id={admin.Id}&code={emailVerificationCode}";
                emailQMessage = isNew ?
                    GetCreateAdminWithVerificationEmailQMessage(admin, password, callbackurl) :
                    GetUpdateAdminWithVerificationEmailQMessage(admin, password, callbackurl);
            }
            else
            {
                emailQMessage = isNew ?
                    GetCreateAdminWithoutVerificationEmailQMessage(admin, password) :
                    GetUpdateAdminWithoutVerificationEmailQMessage(admin, password);
            }
            _sendEmailNotifierService.NotifyAsync(emailQMessage);
        }
        private EmailQMessage GetCreateAdminWithVerificationEmailQMessage(ApplicationUser user, string password, string callbackUrl)
        {
            EmailQMessage emailQMessage;

            emailQMessage = new EmailQMessage()
            {
                From = _configurationManager.GetEmailFrom(),
                To = new string[] { user.Email },
                Subject = _configurationManager.GetCreateAdminEmailSubject(),
                Type = "AddNewAdminWithVerificationEmailTemplate",
                ExtraParams = null,
                Body = $"Dear {user.FirstName}, <br> <br>" +
                       $"Congratulations! An admin account has been created for you.<br>" +
                       $"Please verify your account by using the below link" +
                       $"<a href='{callbackUrl}'> Verify Account </a>" +
                       $"Login with your email and password: {password} <br> <br>" +
                       $"Regards"
            };
            return emailQMessage;
        }
        private EmailQMessage GetCreateAdminWithoutVerificationEmailQMessage(ApplicationUser user, string password)
        {
            EmailQMessage emailQMessage;

            var st = _configuration["PortalUrl"];

            emailQMessage = new EmailQMessage()
            {
                From = _configurationManager.GetEmailFrom(),
                To = new string[] { user.Email },
                Subject = _configurationManager.GetCreateAdminEmailSubject(),
                Type = "AddNewAdminWithoutVerificationEmailTemplate",
                ExtraParams = null,
                Body = $"Dear {user.FirstName}, <br> <br>" +
                       $"Congratulations! An admin account has been created for you.<br>" +
                       $"Please login with your email and password: {password} <br>" +
                       $"Portal link: <a href='{_configuration["PortalUrl"]}'> {_configuration["PortalUrl"]} </a> <br> <br>" +
                       $"Regards"
            };
            return emailQMessage;
        }
        private EmailQMessage GetUpdateAdminWithVerificationEmailQMessage(ApplicationUser user, string password, string callbackUrl)
        {
            EmailQMessage emailQMessage;

            emailQMessage = new EmailQMessage()
            {
                From = _configurationManager.GetEmailFrom(),
                To = new string[] { user.Email },
                Subject = _configurationManager.GetUpdateAdminEmailSubject(),
                Type = "UpdateAdminWithVerificationEmailTemplate",
                ExtraParams = null,
                Body = $"Dear {user.FirstName}, <br> <br>" +
                       $"Your account email has been updated.<br>" +
                       $"Please verify your account by using the below link" +
                       $"<a href='{callbackUrl}'> Verify Account </a>" +
                       $"Login with your email and password: {password} <br> <br>" +
                       $"Regards"
            };
            return emailQMessage;
        }
        private EmailQMessage GetUpdateAdminWithoutVerificationEmailQMessage(ApplicationUser user, string password)
        {
            EmailQMessage emailQMessage;

            emailQMessage = new EmailQMessage()
            {
                From = _configurationManager.GetEmailFrom(),
                To = new string[] { user.Email },
                Subject = _configurationManager.GetUpdateAdminEmailSubject(),
                Type = "UpdateAdminWithoutVerificationEmailTemplate",
                ExtraParams = null,
                Body = $"Dear {user.FirstName}, <br> <br>" +
                       $"Your account email has been updated.<br>" +
                       $"Please login with your email and password: {password} <br> <br>" +
                       $"Regards"
            };
            return emailQMessage;
        }
        #endregion
    }
}
