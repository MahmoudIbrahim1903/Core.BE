using System.Runtime.Serialization;

namespace Emeint.Core.BE.Model
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