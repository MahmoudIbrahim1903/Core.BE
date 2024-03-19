using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    /// <summary>
    /// The Model to send in the Register Method
    /// </summary>
    public class RegisterViewModel
    {
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        //public string UserRole { get; set; }
        public List<string> UserRoles { get; set; }

        public string UserName { get; set; }
        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        /// <summary>
        /// Example: +201223792581
        /// </summary>
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        //public DateTime? DateOfBirth { get; set; }
        public string TenantCode { get; set; }
        public string ApplicationVersion { get; set; }
        public string Language { get; set; }
        //public Gender Gender { get; set; }
        public Domain.Enums.Platform Platform { get; set; }
    }
}