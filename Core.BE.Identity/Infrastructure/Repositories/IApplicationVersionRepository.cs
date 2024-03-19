using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion;
using Emeint.Core.BE.Identity.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Repositories
{
    public interface IApplicationVersionRepository : IRepository<ApplicationVersion>
    {
        CheckApplicationVersionResponseViewModel CheckApplicationVersion(string version, Domain.Enums.Platform platform);
        ApplicationVersion GetLatestApplicationVersion(Domain.Enums.Platform platform);

        ApplicationVersion GetApplicationVersion(string version, Domain.Enums.Platform platform);
        ApplicationVersion GetApplicationVersionByCode(string versionCode);
        bool DeleteVersionByCode(string versionCode);
        IQueryable<ApplicationVersion> GetApplicationVersions(string version, bool? isBeta, bool? isDiscontinued, Domain.Enums.Platform? platform);
        List<ApplicationVersion> GetAllPreviousVersions(DateTime creationDate, Domain.Enums.Platform platform);
        string GetMaximumApplicationVersionNumber(Domain.Enums.Platform platform);
        bool IsTheOnlyActivePlatformVersion(Domain.Enums.Platform platform, string version);

    }
}
