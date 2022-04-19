using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests.UnitTests
{
    public class CardTestDurationTests
    {
        // Properties
        public CardTestDuration cardTestDuration;

        // Constructor
        public CardTestDurationTests()
        {
            cardTestDuration = new CardTestDuration("X16434", new DateTime(2020, 12, 12), new DateTime(2021, 12, 12));
        }


        [Fact]
        public async void InvalidRepositoryOnExecute_Test()
        {
            Result<QueryResult> queryResult = await cardTestDuration.Execute(null);
            Assert.False(queryResult.Status);
        }
    }
}
