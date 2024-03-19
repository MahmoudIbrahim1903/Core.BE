using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Application.Dtos.SendbirdDtos
{
    public class GetSendbirdUsersResponseDto
    {
        public List<object> Users { get; set; }
        public string Next { get; set; }
    }
}
