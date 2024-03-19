using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.Model
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole(string name):base(name)
        {

        }
        public bool IsAdmin { get; set; }
        public bool IsPhoneNumberVerificationRequired { get; set; }
        public virtual ICollection<IdentityUserRole<string>> Users { get; } = new List<IdentityUserRole<string>>();
    }
}
