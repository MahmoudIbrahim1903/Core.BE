using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Chatting.Application.Dtos;
using Emeint.Core.BE.Chatting.Domain.Configurations;
using Emeint.Core.BE.Chatting.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Controllers.Admin
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/admin/chatting/users/v{version:apiVersion}")]
    public class UsersAdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfigurationManager _configurationManager;
        public UsersAdminController(IUserService userService, IConfigurationManager configurationManager)
        {
            _userService = userService;
            _configurationManager = configurationManager;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin, ChatFullController")]
        [Route("types")]
        public Response<List<string>> GetUsersTypes()
        {

            var types = _userService.GetUsersTypes(true);
            return new Response<List<string>>
            {
                Data = types
            };
        }
    }
}
