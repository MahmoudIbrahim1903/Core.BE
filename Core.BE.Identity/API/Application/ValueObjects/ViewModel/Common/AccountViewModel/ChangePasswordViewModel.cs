using System.ComponentModel.DataAnnotations;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}