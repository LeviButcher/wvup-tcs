using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using tcs_service.Helpers;

namespace tcs_service_test.Helpers
{
    public class PagingTest
    {
        [Fact]
        public void Page1WithTakeHalfOfListSizeAndListHasSizeOf20_ShouldHaveDataSizeOfTakeAndPrevAsFalseAndNextAsTrue()
        {
            var list = Enumerable.Repeat(1, 20).ToList();
            var take = list.Count / 2;
            var page = new Paging<int>(1, take, list);
            Assert.Equal(take, page.Data.Count());
            Assert.False(page.HasPrev);
            Assert.True(page.HasNext);
        }

        [Fact]
        public void Page2WithTakeSizeOfListAndAndListHasSizeOf10_ShouldHaveEmptyDataWithPrevAsTrueAndNextAsFalse()
        {
            var list = Enumerable.Repeat(1, 10).ToList();
            var take = list.Count;
            var page = new Paging<int>(2, take, list);
            Assert.Empty(page.Data);
            Assert.True(page.HasPrev);
            Assert.False(page.HasNext);
        }

        [Fact]
        public void Page0Should_ThrowError()
        {
            var list = Enumerable.Repeat(1, 10).ToList();
            var take = list.Count;
            Assert.ThrowsAny<Exception>(() =>
                new Paging<int>(0, take, list)
            );
        }

        [Fact]
        public void Page1WithTakeOfNegative1_ShouldThrowError()
        {
            var list = Enumerable.Repeat(1, 10).ToList();
            var take = -1;
            Assert.ThrowsAny<Exception>(() =>
                new Paging<int>(0, take, list)
            );
        }

        [Fact]
        public void Page1WithEmptyList_ShouldHaveDataBeEmptyWithPrevAndNextAsFalse()
        {
            var list = new List<int>();
            var take = 10;
            var page = new Paging<int>(1, take, list);
            Assert.Empty(page.Data);
            Assert.False(page.HasPrev);
            Assert.False(page.HasNext);
        }

        [Fact]
        public void Page1WithTake1AndListSizeOf99_ShouldHaveDataSize1AndCurrentPageAs1AndTotalPagesAs99()
        {
            var list = Enumerable.Repeat(1, 99).ToList();
            var take = 1;
            var page = new Paging<int>(1, take, list);
            Assert.Equal(take, page.Data.Count());
            Assert.Equal(1, page.CurrentPage);
            Assert.Equal(list.Count, page.TotalPages);
        }

        [Fact]
        public void Page1WithTake2AndListSizeOf100_ShouldHaveDataSize2AndCurrentPageAs1AndTotalPagesAs50AndTotalRecordsAs100()
        {
            var list = Enumerable.Repeat(1, 100).ToList();
            var take = 2;
            var page = new Paging<int>(1, take, list);
            Assert.Equal(take, page.Data.Count());
            Assert.Equal(1, page.CurrentPage);
            Assert.Equal(list.Count / 2, page.TotalPages);
            Assert.Equal(list.Count, page.TotalRecords);
        }
    }
}