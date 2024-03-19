using System.Runtime.Serialization;


namespace Emeint.Core.BE.Notifications.API.Application.ValueObjects.Common
{
    [DataContract]
    public class PaginationResponseViewModel
    {
        [DataMember]
        public int Length { get; set; }
        [DataMember]
        public bool HasMore { get; set; }
    }
}
