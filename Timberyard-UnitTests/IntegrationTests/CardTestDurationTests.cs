using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Timberyard_UnitTests.IntegrationTests
{
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

        // TODO - change inputs according to db
        [Theory]
        [InlineData("X93655", 2020, 2020, true, 2, new string[] { "73531", "73003"}, new double[] { 594, 602 }, new double[] { 596, 605 })]                                               // Happy : There are X records of the inputs out of Y records ( where X==Y )
        [InlineData("", 2020, 2020, true, 3, new string[] { "ate", "73531", "73003", "***" }, new double[] { 6306, 234, 591, 303 }, new double[] { 6308, 235, 591, 304 })]                // Happy : There are X records of the inputs out of Y records ( where X<Y )
        [InlineData("X16298", 2001, 2000, true, 0, new string[] {}, new double[] {}, new double[] {})]                                                                                    // Happy : There are 0 records of the inputs out of Y records        
        [InlineData("X16434", 2000, 2001, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 }, new double[] { 93.12, 95.2, 89.2 })]                // Sad : There are 0 records of the inputs out of 0 records ( catalog does not exists )         
        [InlineData("X16434", 2011, 2000, true, 0, new string[] {}, new double[] {}, new double[] {})]                                                                                    // Bad : invalid dates         
        public async void CardTestDuration_Scenarios_Test
            (string catalog, int startDate, int endDate, bool query_result, int records_count, string[] Operators_results, double[] NetTimeAvg_results, double[] TotalTimeAvg_results)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateCardTestDuration(catalog, new DateTime(startDate, 11, 11), new DateTime(endDate, 11, 12));
            Assert.Equal(query_result, queryResult.Status);
            Assert.Equal(new string[] { "Operator", "NetTimeAvg", "TotalTimeAvg" }, queryResult.Data.ColumnNames);
            Assert.Equal(records_count, queryResult.Data.Records.Count);

            for (int i = 0; i < records_count; i++)
            {
                Assert.Equal(Operators_results[i], queryResult.Data.Records[i]["Operator"]);
                Assert.Equal(NetTimeAvg_results[i], queryResult.Data.Records[i]["NetTimeAvg"]);
                Assert.Equal(TotalTimeAvg_results[i], queryResult.Data.Records[i]["TotalTimeAvg"]);
            }

        }
    }
}
