using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Admin;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel;
using Emeint.Core.BE.Identity.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Model;
using System.Collections.Generic;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public interface IAdminsRepository : IRepository<ApplicationUser>
    {
        PagedList<ApplicationUser> GetAdminsByCriteria(string name, PaginationVm pagination, AdminsSortBy sortBy, SortDirection direction, string country);
        ApplicationUser GetById(string id);
    }
}
