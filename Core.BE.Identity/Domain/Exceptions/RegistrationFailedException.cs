using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Emeint.Core.BE.Identity.Domain.Exceptions
{
    public class RegistrationFailedException : BusinessException
    {
        public RegistrationFailedException(List<IdentityError> errors, int passwordMinLength)
        {
            IdentityErrorDescriber identityErrorDescriber = new IdentityErrorDescriber();

            Code = (int)IdentityErrorCodes.RegistrationFailed;
            Resourcekey = IdentityErrorCodes.RegistrationFailed.ToString();
            MessageParameters.Add(passwordMinLength.ToString());

            MessageEn = string.Empty;
            MessageAr = string.Empty;
            MoreDetails = string.Empty;

            foreach (var error in errors)
            {
                if (MoreDetails != string.Empty)
                {
                    MoreDetails = MoreDetails + ", ";
                    MoreDetails += error.Description;
                }
                else
                    MoreDetails = error.Description;

                /*if (MessageEn != string.Empty)
                {
                    MessageEn = MessageEn + ", ";
                    MessageAr = MessageAr + ", ";
                }
                if (error.Code == identityErrorDescriber.PasswordRequiresDigit().Code)
                {
                    MessageEn += "Password must have at least one digit";
                    MessageAr += "يجب أن تحتوي كلمة المرور علي رقم ";
                }
                else if (error.Code == identityErrorDescriber.PasswordRequiresLower().Code)
                {
                    MessageEn += "Password must have at least one lower case letter";
                    MessageAr += "يجب أن تحتوي كلمة المرور علي حرف صغير ";
                }
                else if (error.Code == identityErrorDescriber.PasswordRequiresUpper().Code)
                {
                    MessageEn += "Password must have at least one upper case letter";
                    MessageAr += "يجب أن تحتوي كلمة المرور علي حرف كبير ";
                }
                else if (error.Code == identityErrorDescriber.PasswordRequiresNonAlphanumeric().Code)
                {
                    MessageEn += "Password must have at least one non alphanumeric character";
                    MessageAr += "يجب أن تحتوي كلمة المرور علي رمز ";
                }
                else if (error.Code == identityErrorDescriber.PasswordTooShort(passwordMinLength).Code)
                {
                    MessageEn += $"Password must be at least {passwordMinLength} characters";
                    MessageAr += $"يجب أن تحتوي كلمة المرور علي {passwordMinLength} حروف ";
                }
                else
                {
                    MessageEn = error.Description;
                    MessageAr = error.Description;
                }*/
            }
        }
    }
}
