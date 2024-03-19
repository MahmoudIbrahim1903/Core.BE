using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Emeint.Core.BE.API.Application.ValueObjects.ViewModels
{
    [DataContract]
    public class PagedList<T>
    {
        [DataMember]
        public List<T> List { set; get; }
        [DataMember]
        public int TotalCount { set; get; }
        [DataMember]
        public bool HasMore { set; get; }


        private PaginationVm PaginationViewModel { get; }
        public PagedList(int totalCount, PaginationVm pagination)
        {

            if (pagination == null || pagination.PageNumber == 0 || pagination.PageSize == 0)
                pagination = new PaginationVm { PageSize = totalCount, PageNumber = 1 };

            PaginationViewModel = pagination;
            TotalCount = totalCount;
            HasMore = (pagination.PageNumber * pagination.PageSize) < totalCount;
        }
        public PagedList()
        {


        }


        public int SkipCount
        {
            get { return PaginationViewModel.PageSize * (PaginationViewModel.PageNumber - 1); }
        }

        public int TakeCount
        {
            get { return PaginationViewModel.PageSize; }
        }


    }
}
