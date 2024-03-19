namespace Emeint.Core.BE.Identity.Domain.Enums
{
    /// <summary>
    /// From 100 to 199
    /// </summary>
    public enum IdentityErrorCodes
    {
        InvalidPassword = 100,
        PhoneNumberVerificationRequired = 101,
        EmailVerificationRequired = 102,
        PhoneNumberAndEmailVerificationRequired = 103,
        PhoneNumberAlreadyExists = 104,
        RegistrationFailed = 105,
        EmailVerificationFailed = 106,
        PhoneNumberVerificationFailed = 107,
        InvalidUsername = 108,
        EmailAlreadyExists = 109,
        UserAlreadyExists = 112,
        ResetPasswordFailed = 116,
        ChangePasswordFailed = 117,
        RemovePasswordFailed = 118,
        AddPasswordFailed = 119,
        PhoneNumberAlreadyVerified = 120,
        EmailAlreadyVerified = 121,
        InvalidUsernameOrPassword = 122,
        DateTimeFormatException = 125,
        UpdatedPhoneNumberNotAllowed = 126,
        UpdatedEmailNotAllowed = 127,
        UserNotFound = 129,
        ApplicationVersionWithNumberNotFound = 137,
        UserCanNotDeleted = 139,
        UnSupportedRole = 141,
        FutureDateNotAllowed = 145,
        InvalidRole = 146,
        ApplicationVersionWithCodeNotFound = 148,
        InvalidApplicationVersion = 149,
        InvalidPhoneNumber = 150,
        UserIsLockedOutException = 151,
        UserSuspended = 152,
        InvalidResetPasswordOtp = 153,
        CountryNotFound = 154,
        NationalIdAlreadyExists = 155,
        UpdateUserFailedException = 156
    }
}