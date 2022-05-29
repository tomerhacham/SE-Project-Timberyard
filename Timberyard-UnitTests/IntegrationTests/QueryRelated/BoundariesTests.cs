using Microsoft.Extensions.DependencyInjection;
using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests.IntegrationTests
{
    [Trait("Category", "Integration-Queries")]
    public class BoundariesTests : TestSuit
    {
        // Properties
        public QueriesController QueriesController;

        public BoundariesTests()
        {
            var serviceProvider = ConfigureServices("IntegrationTests");
            QueriesController = serviceProvider.GetService<QueriesController>();
        }


        [Theory]
        [InlineData("X39337", 2021, 2022, true, new string[] { "Boundaries_Test2", "Boundaries_Test1" }, new double[] { 2, 1 }, new double[] { 6, 4 }, new double[] { 4, 2.24 }, new double[] { 1.41421, 0.749577 })]          // Happy : There are X records of the inputs out of Y records
        [InlineData("X39337", 2020, 2019, false, new string[] { }, new double[] { }, new double[] { }, new double[] { }, new double[] { })]                     // Bad: invalid dates
        [InlineData("", 2021, 2022, false, new string[] { }, new double[] { }, new double[] { }, new double[] { }, new double[] { })]                           // Bad: empty catalog
        [InlineData("X12345", 2021, 2022, true, new string[] { }, new double[] { }, new double[] { }, new double[] { }, new double[] { })]                      // Happy: No records for the given catalog                                                                                                                          
        public async void Boundaries_Scenarios_Test(string catalog, int startDate, int endDate, bool expectedResult, String[] testNames, double[] minValue, double[] maxValue, double[] average, double[] standardDeviation)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateBoundaries(catalog, new DateTime(startDate, 11, 15), new DateTime(endDate, 01, 22));
            Assert.Equal(expectedResult, queryResult.Status);
            if (expectedResult)
            {
                if (queryResult.Data.ColumnNames.Length > 0)
                {
                    Assert.Equal(new string[] { "TestName", "Min", "Max", "Average", "StandardDeviation", "Received" }, queryResult.Data.ColumnNames);
                }

                Assert.Equal(testNames.Length, queryResult.Data.Records.Count);
                var data = queryResult.Data;
                for (int i = 0; i < testNames.Length; i++)
                {
                    Assert.Equal(testNames[i], data.Records[i].TestName);
                    Assert.Equal(minValue[i], data.Records[i].Min);
                    Assert.Equal(maxValue[i], data.Records[i].Max);
                    Assert.Equal(average[i], data.Records[i].Average, 2);
                    Assert.Equal(standardDeviation[i], data.Records[i].StandardDeviation, 5);
                }
            }
        }
    }
}
