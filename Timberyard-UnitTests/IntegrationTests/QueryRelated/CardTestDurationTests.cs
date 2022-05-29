using Microsoft.Extensions.DependencyInjection;
using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests.IntegrationTests
{
    [Trait("Category", "Integration-Queries")]
    public class CardTestDurationTests : TestSuit
    {

        // Properties
        public QueriesController QueriesController;

        // Constructor
        public CardTestDurationTests()
        {
            var serviceProvider = ConfigureServices("IntegrationTests");
            QueriesController = serviceProvider.GetService<QueriesController>();
        }

        [Theory]
        [InlineData("X93655", 2021, 2022, true, 2, new string[] { "75653", "73003" }, new double[] { 1085, 2052 }, new double[] { 1086, 2053 })]                                          // Happy : There are X records of the inputs out of Y records ( where X==Y )
        [InlineData("X17837", 2021, 2022, true, 1, new string[] { "71220" }, new double[] { 814 }, new double[] { 815 })]                                                                 // Happy : There are X records of the inputs out of Y records ( where X<Y )
        [InlineData("X93677", 2021, 2022, true, 0, new string[] { }, new double[] { }, new double[] { })]                                                                                    // Happy : There are 0 records of the inputs out of Y records  ( catalog does not exists )      
        [InlineData("", 2021, 2022, false, 0, new string[] { }, new double[] { }, new double[] { })]                                                                                    // Bad : Invalid catalog           
        [InlineData("X16434", 2022, 2021, false, 0, new string[] { }, new double[] { }, new double[] { })]                                                                                    // Bad : invalid dates         
        public async void CardTestDuration_Scenarios_Test
            (string catalog, int startDate, int endDate, bool query_result, int records_count, string[] Operators_results, double[] NetTimeAvg_results, double[] TotalTimeAvg_results)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateCardTestDuration(catalog, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(query_result, queryResult.Status);
            if (queryResult.Status)
            {
                if (queryResult.Data.ColumnNames.Length > 0)
                {
                    Assert.Equal(new string[] { "Operator", "NetTimeAvg", "TotalTimeAvg" }, queryResult.Data.ColumnNames);
                }
                Assert.Equal(records_count, queryResult.Data.Records.Count);

                for (int i = 0; i < records_count; i++)
                {
                    Assert.Equal(Operators_results[i], queryResult.Data.Records[i].Operator);
                    Assert.Equal(NetTimeAvg_results[i], queryResult.Data.Records[i].NetTimeAvg);
                    Assert.Equal(TotalTimeAvg_results[i], queryResult.Data.Records[i].TotalTimeAvg);
                }
            }

        }
    }
}
