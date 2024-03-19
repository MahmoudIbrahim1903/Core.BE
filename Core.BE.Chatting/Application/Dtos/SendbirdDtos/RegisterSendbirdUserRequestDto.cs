using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Application.Dtos.SendbirdDtos
{
    public class RegisterSendbirdUserRequestDto
    {
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public string ProfileUrl { get; set; }
    }
}
