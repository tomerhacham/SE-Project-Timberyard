using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests
{
    public class CardYieldTests
    {

        // Properties
        public CardYield CardYield;

        // Constructor
        public CardYieldTests()
        {
            CardYield = new CardYield("X16434", new DateTime(2020, 12, 12), new DateTime(2021, 12, 12));
        }


        [Fact]
        public async void InvalidRepositoryOnExecute_Test()
        {
            Result<QueryResult> queryResult = await CardYield.Execute(null);
            Assert.False(queryResult.Status);
        }
    }
}
