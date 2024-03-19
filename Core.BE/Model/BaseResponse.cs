using System.Runtime.Serialization;
using Emeint.Core.BE.Domain.Enums;

namespace Emeint.Core.BE.Model
{
    [DataContract]
    public class BaseResponse
    {
        public BaseResponse()
          : this(0, string.Empty)
        {
        }

        public BaseResponse(int er_code, string er_msg)
            : this(er_code, er_msg, 0)
        {
        }

        public BaseResponse(int er_code, string er_msg, int er_details)
        {
            ErrorCode = er_code;
            ErrorMsg = er_msg;
            ErrorDetails = ErrorDetails;
            Expiration = new Expiration();
            Persistence = new Persistence();
        }

        public bool IsSucceeded
        {
            get
            {
                return this.ErrorCode == (int)ErrorCodes.Success;
            }
        }

        [DataMember]
        public int ErrorCode { get; set; }
        
        [DataMember]
        public string ErrorMsg { get; set; }

        [DataMember]
        public string ErrorDetails { get; set; }

        [DataMember]
        public Expiration Expiration { get; set; }

        [DataMember]
        public Persistence Persistence { get; set; }

        [DataMember]
        public int TotalSeconds { get; set; }


        public override string ToString()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return json;
        }
    }

    [DataContract]
    public class Expiration
    {
        public Expiration()
        {

        }

        public Expiration(int duration, int method, int mode, bool is_session_expiry = false)
        {
            this.IsAllowed = true;
            this.Duration = duration;
            this.Method = method;
            this.Mode = mode;
            this.IsSessionExpiry = is_session_expiry;
        }

        [DataMember]
        public bool IsAllowed { get; set; }

        [DataMember]
        public int Duration { get; set; } // In seconds

        [DataMember]
        public int Method { get; set; }

        [DataMember]
        public int Mode { get; set; }

        [DataMember]
        public bool IsSessionExpiry { get; set; }

        public enum ExpiryDuration
        {
            //time in seconds
            NoExpiry = 0,
            QuarterHour = 15 * 60,
            HalfHour = 30 * 60,
            OneHour = 60 * 60,
            OneDay = 24 * 60 * 60
        }
    }

    public class Persistence
    {
        public Persistence()
            : this((int)ScopeLevel.App, false)
        {
        }

        public Persistence(int scope, bool isEncrypted)
        {
            this.Scope = scope;
            this.IsEncrypted = isEncrypted;
        }

        [DataMember]
        public int Scope { get; set; }

        [DataMember]
        public bool IsEncrypted { get; set; }

        public enum ScopeLevel
        {
            App = 0,
            User = 1
        }
    }
}