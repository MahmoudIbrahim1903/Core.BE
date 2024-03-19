using System.Runtime.Serialization;

namespace Emeint.Core.BE.API.Application.ValueObjects.ViewModels
{
    [DataContract]
    public class Response<T> : BaseResponse
    {
        public Response()
        {
        }

        [DataMember]
        public new T Data { get; set; }        
    }
}