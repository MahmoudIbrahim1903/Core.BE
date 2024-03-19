using System.ComponentModel.DataAnnotations;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}
