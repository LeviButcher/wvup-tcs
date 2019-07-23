using System;
using System.Collections.Generic;

namespace tcs_service.Helpers
{
    public class PagingModel<T>
    {
        public int Skip { get; } = 0;
        public int Take { get; } = 5;
        public PagingModel(int skip, int take, int totalDataCount, IEnumerable<T> data)
        {
            this.Skip = skip;
            this.Take = take;
            this.TotalDataCount = totalDataCount;
            this.data = data;
        }

        public int TotalDataCount { get; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalDataCount / PageSize);

        public int PageSize => Take;

        public bool isNext => CurrentPage < TotalPages;
        public bool isPrev => CurrentPage > 1;

        public int CurrentPage => Skip < Take ? 1 : Skip / Take + 1;

        public IEnumerable<T> data { get; }
    }
}
