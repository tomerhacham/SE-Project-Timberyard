using Microsoft.Extensions.DependencyInjection;
using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

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
        [InlineData("X93655", 2021, 2022, true, 3, new string[] { "9901X_JTAG", "HIO400A_JTAG" }, new double[] { 70, 50 })]                     // Happy : There are X records of the inputs out of Y records 
        [InlineData("", 2010, 2011, true, 3, new string[] { "XMCP-B_HIK" }, new double[] { 0 })]                                                // Happy : There are 0 success records of the inputs out of Y records
        [InlineData("X93677", 2001, 2000, true, 3, new string[] { }, new double[] { })]                                                         // Happy : There are 0 records of the inputs out of Y records (catalog not exists)   
        [InlineData("____", 2022, 2021, true, 3, new string[] { }, new double[] { })]                                                           // Bad : invalid catalog         
        [InlineData("X93655", 2022, 2021, true, 3, new string[] { }, new double[] { })]                                                         // Bad : invalid dates         
        public async void CardYield_Scenarios_Test
            (string catalog, int startDate, int endDate, bool query_result, int records_count, string[] CardName_results, double[] SuccessRatio_results)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateCardYield(catalog, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
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
