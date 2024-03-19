using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emeint.Core.BE.Identity.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Model;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Enums;
using System.Linq;
using Emeint.Core.BE.Identity.Domain.Configurations;
using Emeint.Core.BE.Utilities;
using System.Text.RegularExpressions;
using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts;
using Emeint.Core.BE.Domain.Exceptions;
using System.Security.Claims;

namespace Emeint.Core.BE.Identity.API.Infrastructure.CustomGrant
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();
        private readonly IIdentityService _identityService;
        private readonly IIdentityConfigurationManager _configurationManager;
        private readonly ICountryService _countryService;
        private readonly IClientRoleValidator _clientRoleValidator;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAccountService _accountService;

        public ResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager,
            IIdentityConfigurationManager configurationManager, IIdentityService identityService,
            IClientRoleValidator clientRoleValidator,
            ICountryService countryService, RoleManager<ApplicationRole> roleManager, IAccountService accountService)
        {
            _userManager = userManager;
            _configurationManager = configurationManager;
            _identityService = identityService;
            _countryService = countryService;
            _clientRoleValidator = clientRoleValidator;
            _roleManager = roleManager;
            _accountService = accountService;
        }


        /// <summary>Validates the resource owner password credential</summary>
        /// <param name="context">The context.</param>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            // load user context.UserName
            string username = context.UserName;

            // Decode username
            string email = System.Net.WebUtility.UrlDecode(username);
            string phoneNumber = null;

            ApplicationUser user = _userManager.FindByEmailAsync(email).Result;
            LoginBy loginBy = LoginBy.Email;

            try
            {
                if (user == null)
                {
                    var countryCode = _identityService.CountryCode;
                    if (string.IsNullOrEmpty(countryCode))
                        countryCode = _configurationManager.GetDefaultCountry();

                    if (countryCode == "EGY")
                    {
                        var country = _countryService.GetCountryByCode(countryCode);
                        if (country != null)
                        {
                            var egyptianPhoneNumberPattern = "^\\+201[0|1|2|5]{1}[0-9]{8}";
                            var egyptianPhoneNumberRegex = new Regex(egyptianPhoneNumberPattern, RegexOptions.None);
                            Match egyptianPhoneMatch = egyptianPhoneNumberRegex.Match(username);
                            if (egyptianPhoneMatch.Success)
                            {
                                user = GetUserByPhoneNumberAndPassword(username, context.Password);
                                phoneNumber = username;
                                loginBy = LoginBy.PhoneNumber;
                            }
                        }
                    }

                    if (countryCode == "TZA")
                    {
                        var country = _countryService.GetCountryByCode(countryCode);
                        if (country != null)
                        { 
                            var tanzaniaPhoneNumberPattern = "^\\+255[6|7]{1}[0-9]{8}";
                            var tanzaniaPhoneNumberRegex = new Regex(tanzaniaPhoneNumberPattern, RegexOptions.None);
                            Match tanzaniaPhoneMatch = tanzaniaPhoneNumberRegex.Match(username);
                            if (tanzaniaPhoneMatch.Success)
                            {
                                user = GetUserByPhoneNumberAndPassword(username, context.Password);
                                phoneNumber = username;
                                loginBy = LoginBy.PhoneNumber;
                            }
                        }
                    }
                }

                if (user == null)
                    throw new InvalidUsernameOrPasswordException();

                #region ValidateRole
                var userRoles = _userManager.GetRolesAsync(user).Result;
                _clientRoleValidator.ValidateClientRole(context.Request.Client.ClientName, userRoles.ToList());
                #endregion

                #region Validate lock out
                if (user.LockoutEnabled &&
                    user.LockoutEnd.HasValue &&
                    user.LockoutEnd > DateTimeOffset.UtcNow)
                    throw new UserIsLockedOutException();
                #endregion


                ValidatePassword(user, phoneNumber ?? email, user.PasswordHash, context.Password, loginBy);
                ValidatePhoneNumberAndEmailConfirmation(user);

                #region Validate Suspended user
                if (user.SuspensionStatus == UserSuspensionStatus.Suspended)
                {
                    if (user.SuspensionSource == SuspensionSource.Admin)
                    {
                        throw new UserSuspendedException();
                    }
                    else
                    {
                        user.SuspensionStatus = UserSuspensionStatus.Active;
                        user.SuspensionSource = null;
                        user.SuspensionReason = null;
                        user.ActivationDate = DateTime.UtcNow;
                        user.ActivatedBy = user.Id;
                        var userUpdatedResult = _userManager.UpdateAsync(user).Result;
                    }
                }
                #endregion

                if (!_configurationManager.IsConcurrentSessionsAllowed())
                {
                    await _accountService.RemoveAllGrantsAsync(user.Id, context.Request.Client.ClientId);
                }

                context.Result = new GrantValidationResult(user.Id, "Password", null, "local",
                    new Dictionary<string, object>()
                    {
                        { "error_code", ErrorCodes.Success},
                        { "error_msg", ""},
                        {"user_display_name", $"{user.FirstName} {user.LastName}" },
                        {"user_id", user.Id },
                        {"allowed_roles", userRoles.ToList() },
                        {"reset_password_required", user.ResetPasswordRequired },
                        {"phone_number", user.PhoneNumber ?? String.Empty },
                        {"email", user.Email ?? String.Empty }
                    });

                //reset lockout values 
                if (user.LockoutEnabled)
                {
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = null;
                    var userUpdatedResult = _userManager.UpdateAsync(user).Result;
                }
            }
            catch (BusinessException ex)
            {
                var dataDic = new Dictionary<string, object>()
                    {
                        {"error_code", ex.Code},
                        {"error_msg", ex.MessageEn},
                    };

                if (ex.Code == (int)IdentityErrorCodes.PhoneNumberVerificationRequired || ex.Code == (int)IdentityErrorCodes.EmailVerificationRequired)
                {
                    dataDic.Add("user_display_name", $"{user.FirstName} {user.LastName}");
                    dataDic.Add("user_id", user.Id);
                }

                if (user != null)
                {
                    context.Result =
                        new GrantValidationResult(user.Id, "Password", null, "local", dataDic);

                    //increase access faild count
                    if (user.LockoutEnabled)
                    {
                        user.AccessFailedCount += 1;

                        if (user.AccessFailedCount >= _configurationManager.GetMaxFailedAccessAttempts())
                            user.LockoutEnd = DateTimeOffset.UtcNow + new TimeSpan(0, _configurationManager.GetDefaultLockoutMinutes(), 0);
                        var userUpdatedResult = _userManager.UpdateAsync(user).Result;
                    }
                }
                else
                {
                    var messageSw = LocalizationUtility.GetLocalizedResourceText(ex.Resourcekey, ex.MessageParameters, Language.sw);
                    context.Result =
                        new GrantValidationResult(TokenRequestErrors.InvalidGrant, $";error_code:{ex.Code};error_msg:{ex.MessageEn};error_msgAr:{ex.MessageAr};error_msgSw:{messageSw};"); //this line returns status_code 400 ,and add error_code and error_msg to response message
                }
            }
            catch (Exception ex)
            {
                context.Result =
                   new GrantValidationResult(TokenRequestErrors.InvalidGrant, $";error_code:1;error_msg:{ex.Message};"); //this line returns status_code 400 ,and add error_code and error_msg to response message
            }

            //return Task.FromResult(context.Result);
        }

        private ApplicationUser GetUserByPhoneNumberAndPassword(string phoneNumber, string password)
        {
            var usersList = _userManager.Users.Where(u => u.PhoneNumber == phoneNumber).ToList();
            foreach (var user in usersList)
            {
                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    return user;
                }
            }
            return null;
        }

        private void ValidatePassword(ApplicationUser user, string username, string hashedPassword, string plainPassword, LoginBy loginBy)
        {
            PasswordVerificationResult password = _passwordHasher.VerifyHashedPassword(user, hashedPassword, plainPassword);

            string currUserName = user.Email;
            if (loginBy == LoginBy.PhoneNumber)
                currUserName = user.PhoneNumber;

            // Validate password
            if (password != PasswordVerificationResult.Success || !string.Equals(username, currUserName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidUsernameOrPasswordException();
            }
        }

        private void ValidatePhoneNumberAndEmailConfirmation(ApplicationUser user)
        {
            //if user is admin -- do not validate on PhoneNumberConfirmed and EmailConfirmed
            var adminRoles = _roleManager.Roles.Where(r => r.IsAdmin).ToList();
            var userRoles = _userManager.GetRolesAsync(user).Result;

            if (userRoles.Intersect(adminRoles.Select(ar => ar.Name)).Any())
                return;

            // Validate Phone number confirmation
            //if (_configurationManager.GetAccountVerificationPhoneVerification() == PhoneNumberVerification.SignUp)
            //{
            // Not confirmed
            //    if (!user.PhoneNumberConfirmed)
            //    {
            //        // return error code 0 and token
            //        throw new PhoneNumberVerificationRequiredException();
            //    }
            //}
            var role = _roleManager.FindByNameAsync(userRoles.First()).Result;
            if (role.IsPhoneNumberVerificationRequired && !user.PhoneNumberConfirmed)
                throw new PhoneNumberVerificationRequiredException();

            if (_configurationManager.GetAccountVerificationEmailVerification() == EmailVerification.SignUp)
            {
                // Not confirmed
                if (!user.EmailConfirmed)
                {
                    throw new EmailVerificationRequiredException();
                }
            }
        }
    }
}