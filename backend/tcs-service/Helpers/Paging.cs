using System;
using System.Collections.Generic;
using System.Linq;

namespace tcs_service.Helpers
{
    ///<summary>Automatically create Paging Data for Use with large datasets in a REST API</summary>
    public class Paging<T>
    {
        private Paging() { }

        ///<summary>Create a new page of data at a specific page determined by the pagesize</summary>
        ///<remarks>Pass in the entire dataset as allData, and whatever page number you pass in will automatically be set in Data</remarks>
        public Paging(int page, int pageSize, IEnumerable<T> allData)
        {
            if (page <= 0) throw new Exception($"Page can't be less then or equal to 0: page was {page}");
            if (pageSize < 0) throw new Exception($"PageSize can't be less then 0: take was {pageSize}");

            var skip = page * pageSize - pageSize;
            PageSize = pageSize;
            CurrentPage = page;
            Data = allData.Skip(skip).Take(PageSize).ToList();
            TotalRecords = allData.Count();
        }

        ///<summary>Create of data at a default size of 20</summary>
        public Paging(int page, IEnumerable<T> allData) : this(page, 20, allData) { }

        ///<summary>The total amount of records within the dataset</summary>
        public int TotalRecords { get; set; }

        ///<summary>The total amount of pages for the dataset</summary>
        public int TotalPages => (int)Math.Ceiling((decimal)TotalRecords / PageSize);

        ///<summary>The size of a page</summary>
        public int PageSize { get; set; }

        ///<summary>Is there another page after the this page</summary>
        public bool HasNext => CurrentPage < TotalPages;

        ///<summary>Is there another page before the this page</summary>
        public bool HasPrev => CurrentPage > 1;

        ///<summary>The current page this page is on</summary>
        public int CurrentPage { get; set; }

        ///<summary>The page of data</summary>
        public IEnumerable<T> Data { get; set; }

        ///<summary>Wrap a dataset in a Page Object, does not trim the pageData</summary>
        public static Paging<T> ApplyPaging(int totalItems, IEnumerable<T> pageData,
        int currentPage, int PageSize = 10) =>
            new Paging<T>()
            {
                PageSize = PageSize,
                TotalRecords = totalItems,
                Data = pageData,
                CurrentPage = currentPage,
            };
    }
}