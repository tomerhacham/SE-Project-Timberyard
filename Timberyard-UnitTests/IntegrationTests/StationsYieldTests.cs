using Microsoft.Extensions.DependencyInjection;
using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests.IntegrationTests
{
    public class StationsYieldTests : TestSuit
    {
        // Properties
        public QueriesController QueriesController;

        // Constructor
        public StationsYieldTests()
        {
            var serviceProvider = ConfigureServices("IntegrationTests");
            QueriesController = serviceProvider.GetService<QueriesController>();
        }

        [Theory]
        [InlineData(2021, 2022, true, new string[] { "11", "2T", "2X", "L4", "L5" }, new double[] { 60, 0, 50, 50, 70 })]                                       // Happy: there is data in these dates
        [InlineData(2017, 2018, true, new string[] { }, new double[] { })]                                                                                      // Happy: there is no data in these dates
        [InlineData(2022, 2021, false, new string[] { }, new double[] { })]                                                                                     // Bad: Invalid dates
        public async void StationsYield_Scenarios_Test(int startDate, int endDate, bool expectedResult, string[] stationNames, double[] SuccessRatioValues)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateStationsYield(new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedResult, queryResult.Status);
            if (expectedResult)
            {
                if (queryResult.Data.ColumnNames.Length > 0)
                {
                    Assert.Equal(new string[] { "Station", "SuccessRatio" }, queryResult.Data.ColumnNames);
                }
                Assert.Equal(stationNames.Length, queryResult.Data.Records.Count);
                var data = queryResult.Data;
                for (int i = 0; i < stationNames.Length; i++)
                {
                    Assert.Equal(stationNames[i], data.Records[i].Station);
                    Assert.Equal(SuccessRatioValues[i], data.Records[i].SuccessRatio);
                    Assert.True(0 <= data.Records[i].SuccessRatio && data.Records[i].SuccessRatio <= 100);
                }
            }
        }
    }
}
