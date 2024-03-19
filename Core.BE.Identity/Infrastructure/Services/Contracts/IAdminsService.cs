using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Admin;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel;
using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Contracts
{
    public interface IAdminsService
    {
        Task<PagedList<AdminVM>> GetAdmins(string name, PaginationVm pagination, AdminsSortBy sortBy, SortDirection direction, string country);
        Task<bool> AddAdmin(AddAdminVM adminReq);
        Task<bool> UpdateAdmin(UpdateAdminVM adminReq);
        Task<List<string>> GetAdminRoles();
    }
}
