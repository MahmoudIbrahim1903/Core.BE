using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Emeint.Core.BE.Domain.Enums;
using System.Linq;
using MassTransit;
using MassTransit.Scoping;

namespace Emeint.Core.BE.API.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private IHttpContextAccessor _context;
        private List<string> userRoles;
        private ScopedConsumeContextProvider _messageContext;


        public IdentityService(IHttpContextAccessor httpContext, ScopedConsumeContextProvider messageContext)
        {
            _context = httpContext;
            _messageContext = messageContext;
        }

        public bool IsValidToken
        {
            get { return string.IsNullOrEmpty(UserId) ? false : true; }
        }

        public string UserId
        {
            get { return _context.HttpContext.User.FindFirstValue("sub"); }
        }

        public string ClientId
        {
            get { return _context.HttpContext.User.FindFirstValue("client_id"); }
        }


        public string DisplayName
        {
            get
            {
                return _context.HttpContext.User.FindFirstValue("name") + " " +
                       _context.HttpContext.User.FindFirstValue("last_name");
            }
        }

        public string FirstName
        {
            get
            {
                return _context.HttpContext.User.FindFirstValue("name");
            }
        }

        public string LastName
        {
            get
            {
                return _context.HttpContext.User.FindFirstValue("last_name");
            }
        }

        public string UserName
        {
            //Asp Identity Identifier: user id or email or phoneNumebr ( according to business)
            get { return _context.HttpContext.User.FindFirstValue("user_name"); }
        }

        public string Email
        {
            get { return _context.HttpContext.User.FindFirstValue("email"); }
        }

        private string _countryCode;
        public string CountryCode
        {
            get
            {
                string countryCode = null;

                //if consumer
                countryCode = _messageContext?.GetContext()?.Headers?.FirstOrDefault(h => h.Key == "Country").Value?.ToString();

                //if API call
                if (string.IsNullOrEmpty(countryCode))
                    countryCode = _context?.HttpContext?.Request?.Headers["Country"].ToString();

                //if logged-in user
                if (string.IsNullOrEmpty(countryCode))
                    countryCode = _context.HttpContext?.User?.FindFirstValue("country_code");

                if (string.IsNullOrEmpty(countryCode))
                    countryCode = _countryCode;

                return countryCode;
            }
            set { _countryCode = value; }
        }

        public string TenantCode
        {
            get { return _context.HttpContext.User.FindFirstValue("tenant_code"); }
        }

        public string CityCode
        {
            get { return _context.HttpContext.User.FindFirstValue("city_code"); }
        }

        public string AreaCode
        {
            get { return _context.HttpContext.User.FindFirstValue("area_code"); }
        }

        public Language Language
        {
            get
            {
                StringValues languageStringValues = new StringValues();

                _context.HttpContext?.Request?.Headers?.TryGetValue("Language", out languageStringValues);

                if (languageStringValues.ToString() != string.Empty)
                {
                    if (languageStringValues.ToString().ToLower() == "en")
                    {
                        return Language.en;
                    }
                    else if (languageStringValues.ToString().ToLower() == "ar")
                    {
                        return Language.ar;
                    }
                    else if(languageStringValues.ToString().ToLower() == "sw")
                    {
                        return Language.sw;
                    }
                }

                return Language.en; // Default is English
            }
        }

        public List<string> UserRoles
        {
            get
            {
                userRoles = new List<string>();
                var roles = _context.HttpContext.User.FindAll("role");
                foreach (var role in roles)
                {
                    userRoles.Add(role.Value);
                }
                return userRoles;
            }
        }

        public PhoneNumberVerification PhoneNumberVerificationRequiredAt
        {
            get
            {
                StringValues phoneNumberVerificationStringValues = new StringValues();

                _context.HttpContext.Request.Headers.TryGetValue("phone_verification_required_at", out phoneNumberVerificationStringValues);

                if (phoneNumberVerificationStringValues.ToString() != string.Empty)
                {
                    if (phoneNumberVerificationStringValues.ToString().ToLower() == "SignUp")
                    {
                        return PhoneNumberVerification.SignUp;
                    }
                }

                return PhoneNumberVerification.None;
            }
        }

        public EmailVerification EmailVerificationRequiredAt
        {
            get
            {
                StringValues emailVerificationStringValues = new StringValues();

                _context.HttpContext.Request.Headers.TryGetValue("email_verification_required_at", out emailVerificationStringValues);

                if (emailVerificationStringValues.ToString() != string.Empty)
                {
                    if (emailVerificationStringValues.ToString().ToLower() == "SignUp")
                    {
                        return EmailVerification.SignUp;
                    }

                }

                return EmailVerification.None;
            }
        }

        public string PhoneNumber
        {
            get { return _context.HttpContext.User.FindFirstValue("phone_number"); }

        }

        public string PhoneNumberConfirmed
        {
            get { return _context.HttpContext.User.FindFirstValue("phone_number_verified"); }
        }

        public string EmailConfirmed
        {
            get { return _context.HttpContext.User.FindFirstValue("email_verified"); }
        }

        public UserSuspensionStatus UserStatus
        {
            get
            {
                var userStatusStringValues = _context.HttpContext.User.FindFirstValue("user_status");

                if (userStatusStringValues.ToString() == string.Empty) return UserSuspensionStatus.Active;

                switch (userStatusStringValues.ToString().ToLower())
                {
                    case "active":
                        return UserSuspensionStatus.Active;
                    case "suspended":
                        return UserSuspensionStatus.Suspended;

                }

                return UserSuspensionStatus.Active;
            }
        }
    }
}