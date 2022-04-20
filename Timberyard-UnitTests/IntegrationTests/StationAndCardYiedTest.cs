using Microsoft.Extensions.DependencyInjection;
using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests.IntegrationTests
{
    public class StationAndCardYiedTest : TestSuit
    {
        // Properties
        public QueriesController QueriesController;

        // Constructor
        public StationAndCardYiedTest()
        {
            var serviceProvider = ConfigureServices("IntegrationTests");
            QueriesController = serviceProvider.GetService<QueriesController>();
        }

        [Theory]
        [InlineData("L5", "X93655", 2021, 2022, true, new string[] { "9901X_JTAG" }, new double[] { 70 })]                          // Happy : There are X records of the inputs out of Y records
        [InlineData("11", "X39337", 2021, 2022, true, new string[] { "RCP08_O_JTAG" }, new double[] { 80 })]                        // Happy : There are X records of the inputs out of Y records
        [InlineData("L", "X93655", 2021, 2022, true, new string[] { }, new double[] { })]                                           // Happy : There are no records since station name does not exsists
        [InlineData("L5", "X93677", 2021, 2022, true, new string[] { }, new double[] { })]                                          // Happy : There are no records since catalog does not exsists
        [InlineData("L5", "X93655", 2017, 2018, true, new string[] { }, new double[] { })]                                          // Happy : There are no records since catalog does not exsists
        [InlineData("10B", "X16434", 2020, 2019, false, new string[] { }, new double[] { })]              // Bad: invalid dates
        [InlineData("10B", "____", 2021, 2022, false, new string[] { }, new double[] { })]                // Bad: invalid catalog
        [InlineData("____", "X16434", 2021, 2022, false, new string[] { }, new double[] { })]             // Bad: invalid station

        public async void StationAndCardYield_Scenarios_Test(string station, string catalog, int startDate, int endDate, bool expectedResult, string[] cardNames, double[] SuccessRatioValues)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateStationAndCardYield(station, catalog, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedResult, queryResult.Status);
            if (expectedResult)
            {
                Assert.Equal(new string[] { "Catalog", "CardName", "SuccessRatio" }, queryResult.Data.ColumnNames);
                var data = queryResult.Data;
                for (int i = 0; i < cardNames.Length; i++)
                {
                    Assert.Equal(cardNames[i], data.Records[i].CardName);
                    Assert.Equal(SuccessRatioValues[i], data.Records[i].SuccessRatio);
                    Assert.True(0 <= data.Records[i].SuccessRatio && data.Records[i].SuccessRatio <= 100);
                }
            }
        }
    }
}
