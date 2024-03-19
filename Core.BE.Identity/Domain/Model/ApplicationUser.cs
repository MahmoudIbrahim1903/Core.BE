using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FirstNameAr { get; set; }
        public string LastNameAr { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string CountryCode { get; set; }
        public string CityCode { get; set; }
        public string AreaCode { get; set; }
        public string AddressDetails { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }

        public string ApplicationVersion { get; set; }
        public string Language { get; set; }
        public Domain.Enums.Platform Platform { get; set; }
        public string ExternalProviderToken { get; set; }
        public string TenantCode { get; set; }
        public string UnlockedBy { set; get; }
        public DateTime? UnlockedDate { set; get; }
        public bool ResetPasswordRequired { get; set; }
        public UserSuspensionStatus SuspensionStatus { get; set; }
        public SuspensionSource? SuspensionSource { get; set; }
        public string SuspendedBy { get; set; }
        public string SuspensionReason { set; get; }
        public DateTime? SuspensionDate { get; set; }
        public string ActivatedBy { get; set; }
        public DateTime? ActivationDate { get; set; }
        public string ResetPasswordOTP { set; get; }
        public DateTime? ResetPasswordOTPCreationTime { set; get; }
        public DateTime? AdminResetPasswordDate { set; get; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; } = new List<IdentityUserLogin<string>>();
        public string Timezone { get; set; }
        public decimal? UtcOffset { get; set; }
        public virtual ICollection<PermittedCountry> PermittedCountries { get; set; }



        /*  //public UserSuspensionStatus UserSuspensionStatus { get; set; }
          [Required]
          public UserSuspensionStatus SuspensionStatus { get; set; }
          public string SuspensionReason { get; set; }
          public string SuspendedByAdminName { get; set; }
          public DateTime? SuspensionDate { get; set; }

          public DateTime? LastSentSmsDate { set; get; }


          [Required]
          public AuthenticatorProvider AuthenticatorProvider { get; set; }
          public DateTime? LastSentEmailDate { set; get; }
          public DateTime? LastModifiedDate { get; set; }
          public string LastModifiedBy { get; set; }

          public string ProfileImage { get; set; }

          public string FacebookLink { get; set; }
          public string GoogleLink { get; set; }

          public DateTime? TermsAcceptanceDate { set; get; }

          public decimal? RatingAverage { get; set; }
          public bool ChangePasswordIsRequired { get; set; }

          public PaymentMethods PaymentMethod { set; get; }
          public string LastUsedCreditCardCode { set; get; }

          public void UpdateLastModified(string modifiedBy)
          {
              LastModifiedBy = modifiedBy;
              LastModifiedDate = DateTime.UtcNow;
          }

          public void Suspend(string suspensionReason, string suspensionByAdminName)
          {
              SuspensionStatus = UserSuspensionStatus.Suspended;
              SuspensionReason = suspensionReason;
              SuspensionDate = DateTime.UtcNow;
              SuspendedByAdminName = suspensionByAdminName;
          }                                   */
    }
}
