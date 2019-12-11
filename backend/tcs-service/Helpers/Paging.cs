using System;
using System.Collections.Generic;
using System.Linq;

namespace tcs_service.Helpers
{
    public class Paging<T>
    {
        public Paging()
        {
        }

        public Paging(int page, int pageSize, IEnumerable<T> allData)
        {
            if (page <= 0) throw new Exception($"Page can't be less then or equal to 0: page was {page}");
            if (pageSize < 0) throw new Exception($"PageSize can't be less then 0: take was {pageSize}");

            var skip = page * pageSize - pageSize;
            this.PageSize = pageSize;
            this.CurrentPage = page;
            this.Data = allData.Skip(skip).Take(this.PageSize);
            this.TotalRecords = allData.Count();
        }

        public Paging(int page, IEnumerable<T> allData) : this(page, 20, allData)
        {
        }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalRecords / PageSize);
        public int PageSize { get; set; }
        public bool HasNext => CurrentPage < TotalPages;
        public bool HasPrev => CurrentPage > 1;
        public int CurrentPage { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}