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
        [InlineData("10B", "X16434", 2020, 2020, true, new string[] { "X16434" }, new string[] { "XFCMV14" }, new double[] { 54.545454545454 })]          // Happy : There are X records of the inputs out of Y records
        [InlineData("10B", "X16434", 2020, 2019, false, new string[] { "X16434" }, new string[] { "XFCMV14" }, new double[] { 54.545454545454 })]         // Bad: invalid dates
        [InlineData("10B", "", 2020, 2020, false, new string[] { "X16434" }, new string[] { "XFCMV14" }, new double[] { 54.545454545454 })]               // Bad: empty catalog
        [InlineData("", "X16434", 2020, 2020, false, new string[] { "X16434" }, new string[] { "XFCMV14" }, new double[] { 54.545454545454 })]            // Bad: empty station

        public async void StationAndCardYield_Scenarios_Test(string station, string catalog, int startDate, int endDate, bool expectedResult, string[] catalogNames, string[] cardNames, double[] SuccessRatioValues)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateStationAndCardYield(station, catalog, new DateTime(startDate, 11, 15), new DateTime(endDate, 11, 30));
            Assert.Equal(expectedResult, queryResult.Status);
            if (expectedResult)
            {
                Assert.Equal(new string[] { "Catalog", "CardName", "SuccessRatio" }, queryResult.Data.ColumnNames);
                Assert.Equal(catalogNames.Length, queryResult.Data.Records.Count);
                var data = queryResult.Data;
                for (int i = 0; i < catalogNames.Length; i++)
                {
                    Assert.Equal(catalogNames[i], data.Records[i].Catalog);
                    Assert.Equal(cardNames[i], data.Records[i].CardName);
                    Assert.Equal(SuccessRatioValues[i], data.Records[i].SuccessRatio);
                    Assert.True(0 <= data.Records[i].SuccessRatio && data.Records[i].SuccessRatio <= 100);
                }
            }
        }
    }
}
