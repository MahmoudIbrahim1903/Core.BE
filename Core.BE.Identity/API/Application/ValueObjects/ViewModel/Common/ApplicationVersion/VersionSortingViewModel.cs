using Emeint.Core.BE.Identity.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion
{
    public class VersionSortingViewModel : BaseSortingViewModel
    {
        public VersionSortBy SortBy { get; set; }
    }
}
