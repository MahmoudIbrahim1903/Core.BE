using System;
using System.Collections.Generic;
using System.Linq;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Domain.Enums;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Admin;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.AccountViewModel;
using Emeint.Core.BE.Identity.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using Emeint.Core.BE.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public class AdminsRepository : BaseRepository<ApplicationUser, ApplicationDbContext>, IAdminsRepository
    {
        public AdminsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public PagedList<ApplicationUser> GetAdminsByCriteria(string name = null, PaginationVm pagination = null, AdminsSortBy sortBy = AdminsSortBy.CreationDate, SortDirection direction = SortDirection.Desc, string country = null)
        {
            var roles = _context.Roles.Where(r => r.IsAdmin && r.Name != "SuperAdmin").Select(r => r.Id).AsNoTracking();
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.Roles.Any(r => roles.Contains(r.RoleId)));
            if (!string.IsNullOrEmpty(name))
                query = query.Where(u => u.FirstName.ToLower().Contains(name.ToLower()) || u.LastName.ToLower().Contains(name.ToLower()) || (u.FirstName + " " + u.LastName).ToLower().Contains(name.ToLower()));

            switch (sortBy)
            {
                case AdminsSortBy.Name:
                    if (direction == BE.Domain.Enums.SortDirection.Desc)
                        query = query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName).AsNoTracking();
                    else
                        query = query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).AsNoTracking();
                    break;
                default:
                    if (direction == BE.Domain.Enums.SortDirection.Desc)
                        query = query.OrderByDescending(u => u.RegistrationDate).AsNoTracking();
                    else
                        query = query.OrderBy(u => u.RegistrationDate).AsNoTracking();
                    break;
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Include(u => u.PermittedCountries);
                query = query.Where(u => u.PermittedCountries.Any(pc => pc.CountryCode.ToLower()== country.ToLower()));
            }

            var adminsPagedList = new PagedList<ApplicationUser>(query.Count(), pagination);
            if (pagination != null)
            {
                query = query.Skip(adminsPagedList.SkipCount).Take(adminsPagedList.TakeCount);
            }
            adminsPagedList.List = query.ToList();
            return adminsPagedList;
        }

        public ApplicationUser GetById(string id)
        {
            return _context.Users.FirstOrDefault(u => u.Id.Equals(id));
        }
    }
}
