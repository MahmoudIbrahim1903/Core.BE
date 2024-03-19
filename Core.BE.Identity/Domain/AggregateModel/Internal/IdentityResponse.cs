using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Domain.AggregateModel.Internal
{
    public class IdentityResponse
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
    }
}
