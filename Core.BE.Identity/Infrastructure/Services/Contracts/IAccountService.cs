using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Admin;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel;
using Emeint.Core.BE.Identity.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Model;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Contracts
{
    public interface IAccountService
    {
        Task<Response<IdentityLoginResponse>> Login(LoginViewModel loginViewModel, string timezone, decimal utcOffset, string country, string platform, string language, string version);
        Task Logout(IdentityServerLogoutRequestDto logout);
        Task<Response<IdentityRefreshTokenResponse>> RefreshToken(RefreshTokenViewModel refreshTokenViewModel);
        Task<string> GetEmailVerificationCode(ApplicationUser user);
        Task<string> VerifyEmail(string userId, string code);
        Task UnlockAccount(string userId);
        Task ResetPasswordAsync(string currentPassword, string newPassword, string userId);
        void ValidateOtpForResetPassword(string phoneNumber, string otp);
        Task ResetPasswordWithOtpAsync(string phoneNumber, string otp, string newPassword);
        Task<string> GenerateAndResetPassword(ApplicationUser user, bool enforceResetPassword);
        Task<string> GenerateResetPasswordOtp(ApplicationUser user);
        Task RemoveAllGrantsAsync(string userId, string clientId);
        Task UpdateAccountInfo(string userName, string timezone, decimal utcOffset, string countryCode, string platform, string language, string version, DateTime? lastLoginDate);
        Task ActivateAccount(ApplicationUser user, string activatedBy);
        Task SuspendAccount(ApplicationUser user, string suspendedBy, SuspensionSource suspensionSource, string suspensionReason);

        Task VerifyPhoneNumber(string userId, string code);
        Task<string> GeneratePhoneNumberVerificationCode(string userId);

        Task<Response<RegisterResponse>> CreateUser(RegisterViewModel user);
        void RemoveAllPersistedGrants(string userId);
        List<string> GetPermittedCountriesCodes();
        void AddPermittedCountryForUser(string userId, string countryCode, string createdBy);
        //Task AdminResetPasswordWithOtpAsync(string email, string otp, string newPassword);


    }
}
