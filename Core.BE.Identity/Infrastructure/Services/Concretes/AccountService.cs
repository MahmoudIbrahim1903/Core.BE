using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel;
using Emeint.Core.BE.Identity.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Repositories;
using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts;
using Emeint.Core.BE.Utilities;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Concretes
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IWebRequestUtility _webRequestUtility;
        private readonly ILogger<AccountService> _logger;
        private readonly IPersistedGrantService _persistedGrantService;
        private readonly IPersistedGrantRepository _persistedGrantRepository;
        private readonly IPermittedCountryService _permittedCountryService;

        #region Generate Password Options
        int _passwordMinLength = 8;
        int _passwordCharGroupLength = 1;
        bool _passwordRequireNumber = true;
        bool _passwordRequireUpperCase = true;
        bool _passwordRequireSpecialChar = false;
        #endregion

        public AccountService(IConfiguration configuration, IWebRequestUtility webRequestUtility,
            ILogger<AccountService> logger, UserManager<ApplicationUser> userManager,
            IIdentityService identityService, IPersistedGrantService persistedGrantService, IPersistedGrantRepository persistedGrantRepository, IPermittedCountryService permittedCountryService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _identityService = identityService;
            _webRequestUtility = webRequestUtility;
            _logger = logger;
            _persistedGrantService = persistedGrantService;
            _persistedGrantRepository = persistedGrantRepository;
            _permittedCountryService = permittedCountryService;
            _passwordMinLength = configuration.GetValue<int>("PasswordMinLength");
            _passwordCharGroupLength = configuration.GetValue<int>("PasswordCharGroupLength");
            _passwordRequireNumber = configuration.GetValue<bool>("PasswordRequireNumber");
            _passwordRequireUpperCase = configuration.GetValue<bool>("PasswordRequireUpperCase");
            _passwordRequireSpecialChar = configuration.GetValue<bool>("PasswordRequireSpecialChar");
        }

        public void RemoveAllPersistedGrants(string userId)
        {
            _persistedGrantRepository.RemoveAllPersistedGrants(userId);
        }

        public async Task<string> GenerateAndResetPassword(ApplicationUser user, bool enforceResetPassword)
        {

            var newPassword = GeneratePasswordUtility.Generate(_passwordMinLength,
               _passwordCharGroupLength, _passwordRequireUpperCase, _passwordRequireSpecialChar,
                _passwordRequireNumber);

            var removingPasswordResult = await _userManager.RemovePasswordAsync(user);
            if (removingPasswordResult.Errors.Any())
                throw new InternalServerErrorException(removingPasswordResult.Errors.FirstOrDefault().Description);

            var addingPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (addingPasswordResult.Errors.Any())
                throw new InternalServerErrorException(addingPasswordResult.Errors.FirstOrDefault().Description);

            if (enforceResetPassword)
                await EnforceResetPassword(user);

            return newPassword;
        }
        public async Task<string> GenerateResetPasswordOtp(ApplicationUser user)
        {
            var otpLength = _configuration["ResetPasswordOtpLength"];
            if (string.IsNullOrEmpty(otpLength))
                otpLength = "4";

            var otp = StringUtility.GenerateRandomNumbersString(Convert.ToInt32(otpLength));

            user.ResetPasswordOTP = otp;
            user.ResetPasswordOTPCreationTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return otp;
        }

        private async Task EnforceResetPassword(ApplicationUser user)
        {
            user.ResetPasswordRequired = true;
            var updateUserResult = await _userManager.UpdateAsync(user);

            if (updateUserResult.Errors.Any())
                throw new InternalServerErrorException(updateUserResult.Errors.FirstOrDefault().Description);
        }

        public virtual async Task<Response<IdentityLoginResponse>> Login(LoginViewModel loginViewModel, string timezone, decimal utcOffset, string country, string platform, string language, string version)
        {
            IdentityLoginResponse loginResponse = null;
            LoginResponse connectResponse = new LoginResponse();

            try
            {
                HttpPostRequest request = new HttpPostRequest($"{_configuration["IdentityUrl"]}/connect/token", loginViewModel,
                "application/x-www-form-urlencoded", new List<KeyValuePair<string, string>>
                { new KeyValuePair<string, string>("Country", _identityService.CountryCode)},
                JsonNamingStrategy.SnakeCase);

                connectResponse = await _webRequestUtility.Post<LoginResponse>(request, JsonNamingStrategy.SnakeCase);

                var claims = new Dictionary<string, string>();

                if (connectResponse.error_code == (int)ErrorCodes.Success)
                {
                    if (connectResponse.reset_password_required)
                        loginResponse = new IdentityLoginResponse(null, null,
                         0, null, null, connectResponse.user_id, connectResponse.user_display_name
                         , connectResponse.allowed_roles, claims, true, connectResponse.email, connectResponse.phone_number);
                    else
                        loginResponse = new IdentityLoginResponse(connectResponse.access_token, connectResponse.token_type,
                          connectResponse.expires_in, connectResponse.refresh_token, _configuration["AccessTokenType"],
                          connectResponse.user_id, connectResponse.user_display_name,
                          connectResponse.allowed_roles, claims, false, connectResponse.email, connectResponse.phone_number);

                    _persistedGrantRepository.RemoveExpiredPersistedGrantsAsync(connectResponse.user_id);
                }
                else
                    loginResponse = new IdentityLoginResponse(null, null,
                     0, null, null, connectResponse.user_id, connectResponse.user_display_name
                     , null, claims, false, connectResponse.email, connectResponse.phone_number);

            }
            catch (Exception ex)
            {
                //extract error_code and error_msg from exception message
                if (ex is BusinessException)
                {
                    var exception = ex as BusinessException;
                    var exceptionDetails = exception.MessageEn.Split(";").Where(m => m.Contains("error_code") || m.Contains("error_msg"));
                    if (exceptionDetails.Any())
                    {
                        connectResponse.error_code = Convert.ToInt32(exceptionDetails.FirstOrDefault(e => e.Contains("error_code")).Split(":")[1]);
                        //connectResponse.error_msg = language.Equals("en") ? exceptionDetails.FirstOrDefault(e => e.Contains("error_msg")).Split(":")[1] : exceptionDetails.FirstOrDefault(e => e.Contains("error_msgAr")).Split(":")[1];
                        string errorMsg = string.Empty;
                        if (language.Equals("ar"))
                            errorMsg = exceptionDetails.FirstOrDefault(e => e.Contains("error_msgAr")).Split(":")[1];
                        else if (language.Equals("sw"))
                            errorMsg = exceptionDetails.FirstOrDefault(e => e.Contains("error_msgSw")).Split(":")[1];
                        else
                            errorMsg = exceptionDetails.FirstOrDefault(e => e.Contains("error_msg")).Split(":")[1];
                        connectResponse.error_msg = errorMsg;

                    }
                    else
                    {
                        connectResponse.error_code = exception.Code;
                        connectResponse.error_msg = exception.MessageEn;
                    }
                }
                else
                {
                    connectResponse.error_code = (int)ErrorCodes.InternalServerError;
                    connectResponse.error_msg = ex.Message;
                }

            }

            if (connectResponse == null)
                throw new InternalServerErrorException("Error while calling connect/token endpoint");

            if (connectResponse.error_code == (int)ErrorCodes.Success)
                await UpdateAccountInfo(connectResponse.user_id, timezone, utcOffset, country, platform, language, version, DateTime.UtcNow);

            return new Response<IdentityLoginResponse>
            {
                Data = loginResponse,
                ErrorCode = connectResponse.error_code,
                ErrorMsg = connectResponse.error_msg
            };

        }


        public async Task Logout(IdentityServerLogoutRequestDto logout)
        {
            //prepare secret
            string plainSecret = $"{logout.client_id}:{logout.client_secret}";
            string encodedSecret = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainSecret));

            //revoke token
            HttpPostRequest revokeTokenRequest = new HttpPostRequest($"{_configuration["IdentityUrl"]}/connect/revocation", new { token = logout.token, token_type_hint = "access_token" },
                "application/x-www-form-urlencoded", new List<KeyValuePair<string, string>>(),
                JsonNamingStrategy.SnakeCase);

            revokeTokenRequest.Headers.Add(new KeyValuePair<string, string>("Authorization", $"Basic {encodedSecret}"));

            await _webRequestUtility.Post<string>(revokeTokenRequest, JsonNamingStrategy.SnakeCase);

            //revoke refresh token
            if (!string.IsNullOrEmpty(logout.refresh_token))
            {
                HttpPostRequest revokeRefreshTokenRequest = new HttpPostRequest($"{_configuration["IdentityUrl"]}/connect/revocation", new { token = logout.refresh_token, token_type_hint = "refresh_token" },
                   "application/x-www-form-urlencoded", new List<KeyValuePair<string, string>>(),
                   JsonNamingStrategy.SnakeCase);
                revokeRefreshTokenRequest.Headers.Add(new KeyValuePair<string, string>("Authorization", $"Basic {encodedSecret}"));

                await _webRequestUtility.Post<string>(revokeRefreshTokenRequest, JsonNamingStrategy.SnakeCase);
            }
        }

        public async Task<string> GetEmailVerificationCode(ApplicationUser user)
        {
            var emailCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(emailCode);
            var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);
            return UrlEncoder.Default.Encode(codeEncoded);
        }

        public async Task<string> VerifyEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException();
            if (user.EmailConfirmed)
                throw new EmailAlreadyVerifiedException();

            var codeDecodedBytes = WebEncoders.Base64UrlDecode(code);
            var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);
            var result = await _userManager.ConfirmEmailAsync(user, codeDecoded);
            if (result.Succeeded)
            {
                // Remove duplicate emails which are not verified
                List<ApplicationUser> usersWithSameEmail = _userManager.Users.Where(u => u.Email == user.Email && !u.EmailConfirmed).ToList();
                foreach (var userWithSameEmail in usersWithSameEmail)
                {
                    await _userManager.DeleteAsync(userWithSameEmail);
                }
                return $"{user.FirstName} {user.LastName}";
            }
            else
                throw new EmailVerificationFailedException();
        }

        public virtual async Task<Response<IdentityRefreshTokenResponse>> RefreshToken(RefreshTokenViewModel refreshTokenViewModel)
        {
            IdentityRefreshTokenResponse refreshTokenResponse = null;
            RefreshTokenResponse connectResponse = new RefreshTokenResponse();

            HttpPostRequest request = new HttpPostRequest($"{_configuration["IdentityUrl"]}/connect/token", refreshTokenViewModel,
                "application/x-www-form-urlencoded", new List<KeyValuePair<string, string>> { },
                JsonNamingStrategy.SnakeCase);

            connectResponse = await _webRequestUtility.Post<RefreshTokenResponse>(request, JsonNamingStrategy.SnakeCase);

            refreshTokenResponse = new IdentityRefreshTokenResponse(
                connectResponse.access_token, connectResponse.token_type,
             connectResponse.expires_in, connectResponse.refresh_token, _configuration["AccessTokenType"]);


            return new Response<IdentityRefreshTokenResponse>
            {
                Data = refreshTokenResponse
            };
        }
        public async Task UnlockAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException();
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            user.UnlockedBy = _identityService.DisplayName;
            user.UnlockedDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
        public async Task ResetPasswordAsync(string currentPassword, string newPassword, string userId)
        {
            int passwordMinLength = Convert.ToInt32(_configuration["PasswordMinLength"]);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException();
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (changePasswordResult.Errors.Any())
                throw new ChangePasswordFailedException(changePasswordResult.Errors.ToList(), passwordMinLength);

            if (user.ResetPasswordRequired)
            {
                user.ResetPasswordRequired = false;
                await _userManager.UpdateAsync(user);
            }

            await RemoveAllGrantsAsync(_identityService.UserId, _identityService.ClientId);
        }

        public virtual async Task RemoveAllGrantsAsync(string userId, string clientId)
        {
            var accessTokenType = _configuration["AccessTokenType"];

            if (accessTokenType == "Reference")
                await _persistedGrantService.RemoveAllGrantsAsync(userId, clientId);
        }

        public async Task UpdateAccountInfo(string userId, string timezone, decimal utcOffset, string countryCode, string platform, string language, string version, DateTime? lastLoginDate)
        {
            if (utcOffset < -720 || utcOffset > 840)
                throw new InvalidParameterException("UtcOffset", utcOffset.ToString());

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException();

            if ((!string.IsNullOrEmpty(timezone)) && (user.Timezone != timezone || user.UtcOffset != utcOffset))
            {
                _logger.LogInformation($"timezone: {timezone}, utcoffset: {utcOffset}, old timezone: {user.UtcOffset}, oldutc offset {utcOffset}");

                user.UtcOffset = utcOffset;
                user.Timezone = timezone;
            }

            if (!string.IsNullOrEmpty(countryCode))
                user.CountryCode = countryCode;

            user.Platform = (Domain.Enums.Platform)Enum.Parse(typeof(Domain.Enums.Platform), platform);
            user.Language = language;
            user.ApplicationVersion = version;
            user.LastLoginDate = lastLoginDate;
            await _userManager.UpdateAsync(user);
        }
        public async Task ActivateAccount(ApplicationUser user, string activatedBy)
        {
            if (user.SuspensionStatus == UserSuspensionStatus.Suspended)
            {
                user.SuspensionStatus = UserSuspensionStatus.Active;
                user.SuspensionSource = null;
                user.ActivationDate = DateTime.UtcNow;
                user.ActivatedBy = activatedBy;
                await _userManager.UpdateAsync(user);
            }
        }
        public async Task SuspendAccount(ApplicationUser user, string suspendedBy, SuspensionSource suspensionSource, string suspensionReason)
        {
            if (user.SuspensionStatus == UserSuspensionStatus.Active)
            {
                user.SuspensionStatus = UserSuspensionStatus.Suspended;
                user.SuspensionReason = suspensionReason;
                user.SuspensionSource = suspensionSource;
                user.SuspendedBy = suspendedBy;
                user.SuspensionDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<string> GeneratePhoneNumberVerificationCode(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException();

            if (user.PhoneNumberConfirmed)
                throw new PhoneNumberAlreadyVerifiedException();

            //Generate Phonenumber verification code
            var phoneCode = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

            return phoneCode;

        }

        public async virtual Task VerifyPhoneNumber(string userId, string code)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException();

            if (user.PhoneNumberConfirmed)
                throw new PhoneNumberAlreadyVerifiedException();

            var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, code);
            if (result.Succeeded)
            {
                // Remove duplicate phone numbers which are not verified
                List<ApplicationUser> usersWithSamePhoneNumber = _userManager.Users.Where(u => u.PhoneNumber == user.PhoneNumber && !u.PhoneNumberConfirmed).ToList();
                foreach (var userWithSamePhoneNumber in usersWithSamePhoneNumber)
                {
                    await _userManager.DeleteAsync(userWithSamePhoneNumber);
                }
            }
            else
            {
                throw new PhoneNumberVerificationFailedException($"{result.Errors.FirstOrDefault().Code} : {result.Errors.FirstOrDefault().Description}");
            }
        }
        public async Task<Response<RegisterResponse>> CreateUser(RegisterViewModel user)
        {

            if (user == null)
                throw new MissingParameterException("User");
            if (user.PhoneNumber != null)
            {
                List<ApplicationUser> usersWithSamePhoneNumber = _userManager.Users.Where(u => u.PhoneNumber == user.PhoneNumber).ToList();
                if (usersWithSamePhoneNumber != null && usersWithSamePhoneNumber.Count > 0 && usersWithSamePhoneNumber.Any(u => u.PhoneNumberConfirmed))
                    throw new PhoneNumberAlreadyExistsException();
            }

            if (user.Email != null)
            {
                EmailUtility.ValidateEmail(user.Email);
                ApplicationUser existingApplicationUserByEmail = _userManager.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingApplicationUserByEmail != null)
                    throw new EmailAlreadyExistsException();
            }


            string externalId = user.ExternalId?.Trim();
            ApplicationUser newApplicationUser = new ApplicationUser
            {
                UserName = user.UserName?.Trim(),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName?.Trim(),
                LastName = user.LastName?.Trim(),
                TenantCode = user.TenantCode,
                RegistrationDate = DateTime.UtcNow,
                Platform = user.Platform
            };

            // To Do make it according to configurations
            if (!string.IsNullOrEmpty(externalId))
                newApplicationUser.Id = externalId;

            var createUserResult = await _userManager.CreateAsync(newApplicationUser, user.Password);

            // Create user failed
            if (!createUserResult.Succeeded || createUserResult.Errors.Any())
                throw new Exception(createUserResult.Errors.FirstOrDefault().Description);

            try
            {
                await _userManager.AddToRolesAsync(newApplicationUser, user.UserRoles);
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(newApplicationUser);
                throw new InternalServerErrorException(ex.Message);
            }

            return new Response<RegisterResponse>()
            {
                Data = new RegisterResponse() { UserId = newApplicationUser.Id },
            };

        }

        public async Task ResetPasswordWithOtpAsync(string phoneNumber, string otp, string newPassword)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber && u.ResetPasswordOTP == otp);
            if (user == null)
                throw new InvalidResetPasswordOtpException();

            await _userManager.RemovePasswordAsync(user);
            var resetPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);

            if (resetPasswordResult.Errors != null && resetPasswordResult.Errors.Any())
                throw new BE.Domain.Exceptions.InvalidOperationException(resetPasswordResult.Errors.FirstOrDefault().Description, resetPasswordResult.Errors.FirstOrDefault().Description);

            user.ResetPasswordOTP = null;
            await _userManager.UpdateAsync(user);
        }

        public void ValidateOtpForResetPassword(string phoneNumber, string otp)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber && u.ResetPasswordOTP == otp);
            if (user == null)
                throw new InvalidResetPasswordOtpException();
        }

        public List<string> GetPermittedCountriesCodes()
        {
            var permittedCountries = _permittedCountryService.GetPermittedCountries(_identityService?.UserId)?.Select(e => e.CountryCode)?.ToList();
            return permittedCountries;
        }

        public void AddPermittedCountryForUser(string userId, string countryCode, string createdBy)
        {
            _permittedCountryService.AddPermittedCountryForUser(userId, countryCode, createdBy);

        }
        //public async Task AdminResetPasswordWithOtpAsync(string email, string otp, string newPassword)
        //{
        //    var user = _userManager.Users.FirstOrDefault(u => u.Email == email && u.ResetPasswordOTP == otp);
        //    if (user == null)
        //        throw new InvalidResetPasswordOtpException();

        //    await _userManager.RemovePasswordAsync(user);
        //    var resetPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);

        //    if (resetPasswordResult.Errors != null && resetPasswordResult.Errors.Any())
        //        throw new BE.Domain.Exceptions.InvalidOperationException(resetPasswordResult.Errors.FirstOrDefault().Description, resetPasswordResult.Errors.FirstOrDefault().Description);

        //    user.ResetPasswordOTP = null;
        //    await _userManager.UpdateAsync(user);
        //}

    }

}
