
using System.Collections.Generic;
using Emeint.Core.BE.Domain.Enums;

using Emeint.Core.BE.Domain.Enums;


namespace Emeint.Core.BE.API.Infrastructure.Services
{
    public interface IIdentityService
    {
        //string GetUserIdentity();
        string UserId { get; }
        //Asp Identity Identifier 
        string UserName { get; }
        string DisplayName { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
        /// <summary>
        /// set is used to set default country code if other sources are not available. Should be used in background services.
        /// </summary>
        string CountryCode { get; set; }
        string TenantCode { get; }
        string CityCode { get; }
        string AreaCode { get; }
        Language Language { get; }
        List<string> UserRoles { get; }
        PhoneNumberVerification PhoneNumberVerificationRequiredAt { get; }
        EmailVerification EmailVerificationRequiredAt { get; }
        string PhoneNumber { get; }
        string PhoneNumberConfirmed { get; }
        string EmailConfirmed { get; }
        UserSuspensionStatus UserStatus { get; }
        string ClientId { get; }
        public bool IsValidToken { get; }

    }
}