namespace Emeint.Core.BE.Notifications.Domain.Enums
{
    /// <summary>
    /// From 200 to 299
    /// </summary>
    public enum MessagingErrorCodes
    {
        ErrorCreateUser = 200,
        InvalidUserId = 201,
        ErrorUpdateMessaageStatus = 202,
        MessageTypeNotFound = 203,
        MessageCategoryNotFound = 204,
        UpdatePushNotificationRegistrationFailed = 205
    }
}
