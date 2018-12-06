using NUnit.Framework;
using Sweater.Api.Models;

namespace Sweater.Api.Test.Models
{
    [TestFixture]
    public class PaginatedResponse
    {
        private static int PageCount(
            int totalItemCount
            , int pageSize
        )
        {
            if (pageSize == 0)
            {
                return 0;
            }

            return (totalItemCount + pageSize - 1) / pageSize;
        }

        [Test]
        public void PageCount_Should_Return_Expected_Value(
            [Range(0, 10)] int totalItemCount,
            [Range(0, 10)] int pageIndex,
            [Range(0, 10)] int pageSize
        )
        {
            var model = new PaginatedResponse<int>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemCount = totalItemCount
            };

            Assert.AreEqual(
                PageCount(totalItemCount, pageSize)
                , model.PageCount
            );
        }
    }
}