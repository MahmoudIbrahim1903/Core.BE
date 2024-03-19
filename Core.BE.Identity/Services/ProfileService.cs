using Emeint.Core.BE.Identity.Domain.Configurations;
using Emeint.Core.BE.Identity.Domain.Model;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityConfigurationManager _configurationManager;
        public ProfileService(UserManager<ApplicationUser> userManager, IIdentityConfigurationManager configurationManager)
        {
            _userManager = userManager;
            _configurationManager = configurationManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;

            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
                throw new ArgumentException("Invalid subject identifier");

            var claims = GetClaimsFromUser(user);
            context.IssuedClaims = claims.ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = true;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                            context.IsActive = false;
                    }
                }
            }
        }

        public virtual IEnumerable<Claim> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName)
            };

            if (!string.IsNullOrWhiteSpace(user.FirstName))
                claims.Add(new Claim("name", user.FirstName));

            if (!string.IsNullOrWhiteSpace(user.LastName))
                claims.Add(new Claim("last_name", user.LastName));

            //Asp Identity Identifier
            claims.Add(new Claim("user_name", user.UserName));

            /*claims.Add(new Claim("terms_acceptance_date", user.TermsAcceptanceDate?.ToString() ?? string.Empty));*/

            //if (_userManager.SupportsUserEmail)
            //{
            //    claims.AddRange(new[]
            //    {
            //        new Claim(JwtClaimTypes.Email, user?.Email),
            //        new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
            //    });
            //}

            if (!string.IsNullOrEmpty(user?.Email))
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.Email, user?.Email),
                });
            }

            if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                    new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }
            // Inject user roles in the JWT
            var roles = _userManager.GetRolesAsync(user);
            if (roles != null && roles.Result.Count > 0)
            {
                foreach (var role in roles.Result)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, role));
                }
            }

            // Inject user DB claims in the JWT
            var dbClaims =  _userManager.GetClaimsAsync(user).Result;
            
            if (dbClaims != null && dbClaims.Any())
                foreach (var claim in dbClaims)
                    claims.Add(new Claim(claim.Type, claim.Value));

            // Inject country in the JWT
            if (!string.IsNullOrWhiteSpace(user.CountryCode))
            {
                claims.Add(new Claim("country_code", user.CountryCode));
            }


            // Inject UserStatus in the JWT
            /*  claims.Add(new Claim("user_status", user.SuspensionStatus.ToString()));    */
            return claims;
        }
    }
}