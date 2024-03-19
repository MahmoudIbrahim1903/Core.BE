using Emeint.Core.BE.Domain.SeedWork;


namespace Emeint.Core.BE.Identity.Domain.Model
{
    public class Profile : Entity
    {
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}