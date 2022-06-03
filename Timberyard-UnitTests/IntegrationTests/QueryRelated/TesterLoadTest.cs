using Microsoft.Extensions.DependencyInjection;
using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests.IntegrationTests
{
    [Trait("Category", "Integration-Queries")]
    public class TesterLoadTest : TestSuit
    {
        // Properties
        public QueriesController QueriesController;

        // Constructor
        public TesterLoadTest()
        {
            var serviceProvider = ConfigureServices("IntegrationTests");
            QueriesController = serviceProvider.GetService<QueriesController>();
        }

        [Theory]
        [InlineData(2021, 2022, true, new string[] { "L5", "11", "7", "2T", "2X", "L4" },                       // happy : there is data
            new int[] { 10, 10, 5, 3, 2, 2 },
            new double[] { 3.199444, 1.059444, 0.162500, 0.551388, 0.944722, 0.039166 })]
        [InlineData(2017, 2018, true, new string[] { }, new int[] { }, new double[] { })]                       // happy : no data between dates                                             // Happy: no records since no data between dates
        [InlineData(2022, 2021, false, new string[] { }, new int[] { }, new double[] { })]                      // bad: Invalid dates                                                // Bad: invalid dates
        public async void TesterLoad_Scenarios_Test(int startDate, int endDate, bool expectedResult, string[] stationNames, int[] numberOfRuns, double[] totalRunTime)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateTesterLoad(new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedResult, queryResult.Status);
            if (expectedResult)
            {
                if (queryResult.Data.ColumnNames.Length > 0)
                {
                    Assert.Equal(new string[] { "Station", "NumberOfRuns", "TotalRunTimeHours" }, queryResult.Data.ColumnNames);
                }
                Assert.Equal(stationNames.Length, queryResult.Data.Records.Count);
                var data = queryResult.Data;
                for (int i = 0; i < stationNames.Length; i++)
                {
                    Assert.Equal(stationNames[i], data.Records[i].Station);
                    Assert.Equal(numberOfRuns[i], data.Records[i].NumberOfRuns);
                    Assert.Equal(totalRunTime[i], Decimal.ToDouble(data.Records[i].TotalRunTimeHours));
                }
            }
        }
    }
}
