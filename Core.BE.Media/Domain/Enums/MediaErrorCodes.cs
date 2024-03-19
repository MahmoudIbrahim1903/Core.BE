namespace Emeint.Core.BE.Media.Domain.Enums
{
    /// <summary>
    /// From 300 to 399
    /// </summary>
    public enum MediaErrorCodes
    {
        DeleteFileFailed = 300,
        ImageAlreadyExist = 301,
        MaxImageSizeExceeded = 302,
        MaxVideoSizeExceeded = 303,
        MaxAudioSizeExceeded = 304,
        UnsupportedAudioFile = 305
    }
}