using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Admin;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel;
using Emeint.Core.BE.Identity.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Emeint.Core.BE.Identity.API.Controllers.Admin
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/identity/admin/admins/v{version:apiVersion}")]
    public class AdminsController : Controller
    {
        private readonly IAdminsService _adminService;

        public AdminsController(IAdminsService adminService)
        {
            _adminService = adminService ?? throw new System.ArgumentNullException(nameof(adminService));

        }
        /// <summary>
        /// Get all Admin users
        /// </summary>
        /// <param name="search_criteria"></param>
        /// <param name="language">ar/en</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin, AdministratorsReader, AdministratorsFullController")]
        public async Task<Response<PagedList<AdminVM>>> GetAll(
            [FromHeader(Name = "Language")] string language, [FromHeader(Name = "Country")] string country,
            [FromQuery] string name = null,
            int page_number = 0, int page_size = 0,
            AdminsSortBy sort_by = AdminsSortBy.CreationDate,
            SortDirection direction = SortDirection.Desc)
        {
            var pagination = new PaginationVm { PageNumber = page_number, PageSize = page_size };
            var result = await _adminService.GetAdmins(name, pagination, sort_by, direction, country);
            return new Response<PagedList<AdminVM>>() { Data = result };
        }

        /// <summary>
        /// Add new admin
        /// </summary>
        /// <param name="admin_user"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, AdministratorsFullController")]
        public async Task<Response<bool>> AddAdmin([FromBody] AddAdminVM admin_user, [FromHeader(Name = "Language")] string language)
        {
            if (admin_user == null)
                throw new MissingParameterException("AddAdminUserRequest");
            if (language == null)
                throw new MissingParameterException("Language");

            var result = await _adminService.AddAdmin(admin_user);
            return new Response<bool> { Data = result };
        }

        /// <summary>
        /// update existing admin
        /// </summary>
        /// <param name="admin_user"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "SuperAdmin, AdministratorsFullController")]
        public async Task<Response<bool>> UpdateAdmin([FromBody] UpdateAdminVM admin_user, [FromHeader(Name = "Language")] string language)
        {
            if (admin_user == null)
                throw new MissingParameterException("UpdateAdminUserRequest");
            if (language == null)
                throw new MissingParameterException("Language");

            var result = await _adminService.UpdateAdmin(admin_user);

            return new Response<bool> { Data = result };
        }

        /// <summary>
        /// get all admin roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("roles")]
        [Authorize(Roles = "SuperAdmin, AdministratorsReader, AdministratorsFullController")]
        public async Task<Response<List<string>>> Roles()
        {
            return new Response<List<string>> { Data = await _adminService.GetAdminRoles() };
        }




    }
}