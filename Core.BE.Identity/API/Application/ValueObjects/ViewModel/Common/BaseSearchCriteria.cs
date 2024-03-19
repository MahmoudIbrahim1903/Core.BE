using Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common.ApplicationVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.API.Application.ValueObjects.ViewModel.Common
{
    public class BaseSearchCriteria
    {
        public PaginationViewModel Pagination { get; set; }

        //public BaseSearchCriteria()
        //{
        //    Pagination = new PaginationViewModel();
        //}
    }
}
