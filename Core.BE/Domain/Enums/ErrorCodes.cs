namespace Emeint.Core.BE.Domain.Enums
{
    /// <summary>
    /// From 0 to 99
    /// </summary>
    public enum ErrorCodes
    {
        Success = 0,
        InternalServerError = 1,
        DuplicateRequest = 2,
        InvalidGuid = 9,
        InvalidParameter = 10,
        MissingParameter = 11,
        UnauthorizedAction = 12,
        ConnectionToProviderFailed = 13,
        InvalidOperation = 14,
        InvalidEmail = 15,
        InvalidPhoneNumber = 16,
        InvalidUrl = 17,
        ForceVersionUpdate = 18,
        OptionalVersionUpdate = 19,
        HashedValueMismatchSignature = 20,
        RequestToProviderFailedException = 21,
        InvalidUniqueParameter = 22,
        InvalidNationalId = 23,
        InvalidUserInput = 24

        //BillingRequired = 4,
        //AccessDenied = 4,
        //InvalidUsernameOrPassword = 5,
        //InvalidSubscriber = 6,
        //SubscriptionAlreadyExists = 7,
        //SubscriptionAlreadyExistsWithAnotherService = 8,
        //SubscriptionRequired = 9,
        //DeactivatedSubscriber = 10,
        //SubscriptionExpired = 11,
        //ActivationCodeRequired = 12,
        //ActivationRequired = 13,
        //DiscontinuedApplicationVersion = 14,
        //DatabaseConnectionFailed = 15,
        //IdentificationServiceConnectionFailed = 16,
        //InvalidToken = 17,
        //SessionExpired = 18,
        //IntegrationServiceError = 19,
        //IdleSubscriber = 20,
        //SendingMailFailed = 21,
        //SendingSmsFailed = 22,
        //InvalidParameterData = 23,
        //MissingMasterData = 24,
        //InvalidOperation = 25,
        //InvalidTerminalCode = 26,
        //InvalidServiceCode = 27,
        //InactiveService = 28,
        //InvalidServiceId = 29,
        //UnSupportedPlatform = 30,
        //SessionInvalidated = 31,
        //ActivationCodeFailed = 32,
        //RegistrationFailure = 33,
        //ResendVerificationCodeTimeRestriction = 34,
        //NoMatchingData = 68678
    }
}