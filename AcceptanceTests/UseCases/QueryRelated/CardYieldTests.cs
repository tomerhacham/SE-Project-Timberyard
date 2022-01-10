using AcceptanceTests.Utils;
using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

namespace AcceptanceTests
{
    public class CardYieldTests : TimberyardTestCase
    {

        public CardYieldTests() : base()
        {
            
        }

        [Theory]
        [InlineData(2020, 2022, "OA_HF")]           // catalog ID exists
        [InlineData(2020, 2022, "OP_KLF")]         
        public void SuccessCaseTest(int start, int end, string catalog)
        {
            Result<QueryResult> res = sut.CalculateCardYield(new DateTime(start, 1, 10), new DateTime(end, 1, 10), catalog);
            Assert.True(res.Status);

        }

        [Theory]
        [InlineData (2020 , 2022, "CCH1")]      // catalog ID not exists
        [InlineData (2020 , 2022, "")]          // empty catalog ID
        [InlineData (2022 , 2020, "")]          // start date > end date
        public void NoExistCatalogNumberTest(int start, int end, string catalog)
        {
            Result<QueryResult> res = sut.CalculateCardYield(new DateTime(start, 1, 10), new DateTime(end, 1, 10), catalog);
            Assert.False(res.Status);
            Assert.Empty(res.Data.Records);
        }

    }
}
