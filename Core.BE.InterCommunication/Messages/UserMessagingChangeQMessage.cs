namespace Emeint.Core.BE.InterCommunication.Messages
{

    public class UserMessagingChangeQMessage
    {
        public string ApplicationUserId { get; set; }
        public string LanguageCode { get; set; }
        public string Platform { get; set; }
    }

}