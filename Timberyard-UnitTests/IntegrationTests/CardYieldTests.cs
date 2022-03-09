using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WebService.Domain.Business.Queries;
using WebService.Domain.DataAccess;
using WebService.Utils;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Timberyard_UnitTests.IntegrationTests
{
    public class CardYieldTests : TestSuit
    {
        // Properties
        public QueriesController QueriesController;

        // Constructor
        public CardYieldTests()
        {
            var serviceProvider = ConfigureServices("IntegrationTests");
            QueriesController = serviceProvider.GetService<QueriesController>();
        }

        // TODO - change inputs according to db
        [Theory]
        [InlineData("X16434", 2000, 2001, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]        // Happy : There are X records of the inputs out of Y records ( where X==Y )
        [InlineData("", 2010, 2011, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]                // Happy : There are X records of the inputs out of Y records ( where X<Y )
        [InlineData("X16434", 2001, 2000, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]          // Happy : There are 0 records of the inputs out of Y records        
        [InlineData("", 2000, 2001, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]                // Bad : There are 0 records of the inputs out of 0 records ( catalog does not exists )         
        [InlineData("", 2001, 2000, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]                // Bad : invalid catalog         
        [InlineData("X16434", 2011, 2000, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]                // Bad : invalid dates         
        public async void CardYield_Scenarios_Test
            (string catalog, int startDate, int endDate, bool query_result, int records_count, string[] CardName_results, double[] SuccessRatio_results)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateCardYield(catalog, new DateTime(startDate, 12, 12), new DateTime(endDate, 12, 12));
            Assert.Equal(query_result, queryResult.Status);
            Assert.Equal(new string[] { "Catalog", "CardName", "SuccessRatio" }, queryResult.Data.ColumnNames);
            Assert.Equal(records_count, queryResult.Data.Records.Count);

            for (int i = 0; i < records_count; i++)
            {
                Assert.Equal(catalog, queryResult.Data.Records[i]["catalog"]);
                Assert.Equal(CardName_results[i], queryResult.Data.Records[i]["cardName"]);
                Assert.Equal(SuccessRatio_results[i], queryResult.Data.Records[i]["successRatio"]);
            }

        }
    }
}
