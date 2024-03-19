using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion;
using Emeint.Core.BE.Identity.Domain.Enums;
using Emeint.Core.BE.Identity.Domain.Model;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using Emeint.Core.BE.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public class ApplicationVersionRepository : BaseRepository<ApplicationVersion, ApplicationDbContext>, IApplicationVersionRepository
    {
        public ApplicationVersionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public CheckApplicationVersionResponseViewModel CheckApplicationVersion(string version, Domain.Enums.Platform platform)
        {
            var latestApplicationVersion = GetLatestApplicationVersion(platform);
            var latestVersion = new Version(latestApplicationVersion.Version);

            var userApplicationVersion = _context.Set<ApplicationVersion>().Where(u => (u.Version == version && u.Platform == platform && !u.IsDeleted)).FirstOrDefault();
            CheckApplicationVersionResponseViewModel response = null;
            if (userApplicationVersion != null)
            {
                var userVersion = new Version(userApplicationVersion.Version);
                response = new CheckApplicationVersionResponseViewModel()
                {
                    ForceUpdate = userApplicationVersion.IsDiscontinued,
                    IsLatest = (userVersion == latestVersion) ? true : false,
                    StoreUrl = latestApplicationVersion.StoreUrl
                };
            }
            return response;
        }

        public ApplicationVersion GetLatestApplicationVersion(Domain.Enums.Platform platform)
        {
            var latestVersion = _context.Set<ApplicationVersion>().Where(v => v.IsDiscontinued == false && v.IsBeta == false && v.Platform == platform && !v.IsDeleted)
                                    .ToList().OrderByDescending(v => Version.Parse(v.Version)).FirstOrDefault();
            if (latestVersion == null)
                throw new InternalServerErrorException($"Couldn't find a valid version for platform: {platform.ToString()}");

            return latestVersion;
        }

        public string GetMaximumApplicationVersionNumber(Domain.Enums.Platform platform)
        {
            Version version = _context.Set<ApplicationVersion>().Where(m => m.Platform == platform && !m.IsDeleted)
                                    .ToList().Select(m => Version.Parse(m.Version)).DefaultIfEmpty().Max();

           return version?.ToString();
        }

        public ApplicationVersion GetApplicationVersion(string version, Domain.Enums.Platform platform)
        {
            return _context.Set<ApplicationVersion>().Where(m => m.Version == version && m.Platform == platform && !m.IsDeleted).FirstOrDefault();
        }

        public ApplicationVersion GetApplicationVersionByCode(string versionCode)
        {
            return _context.Set<ApplicationVersion>().Where(v => v.Code == versionCode && !v.IsDeleted).FirstOrDefault();

        }

        public bool DeleteVersionByCode(string versionCode)
        {
            var version = _context.Set<ApplicationVersion>().FirstOrDefault(v => v.Code == versionCode);
            version.IsDeleted = true;
            _context.SaveChanges();
            return true;
        }

        public IQueryable<ApplicationVersion> GetApplicationVersions(string version, bool? isBeta, bool? isDiscontinued, Domain.Enums.Platform? platform)
        {
            return _context.Set<ApplicationVersion>().Where
                (v =>
                    (!v.IsDeleted)
                 && (isBeta == null || v.IsBeta == isBeta)
                 && (platform == null || v.Platform == platform)
                 && (version == null || v.Version == version)
                 && (isDiscontinued == null || v.IsDiscontinued == isDiscontinued)
                //&& (ForceUpdated == null || v.ForceUpdated == ForceUpdated)
                );
        }

        public List<ApplicationVersion> GetAllPreviousVersions(DateTime creationDate, Domain.Enums.Platform platform)
        {


            return _context.Set<ApplicationVersion>().Where
               (v =>
                   (!v.IsDeleted)
                && (v.CreationDate < creationDate && v.Platform == platform)
               ).ToList();
        }

        public bool IsTheOnlyActivePlatformVersion(Domain.Enums.Platform platform, string version)
        {
            var anotherActiveVersion = _context.Set<ApplicationVersion>()
                  .Where(m => m.IsDiscontinued == false && m.IsBeta == false && m.Platform == platform && m.Version != version && !m.IsDeleted).ToList().Select(m => Version.Parse(m.Version)).DefaultIfEmpty().Max();

            if (anotherActiveVersion == null)
                return true;
            else
                return false;
        }

    }
}
