using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests.IntegrationTests
{
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
        [InlineData("10B", 2020, 2020, true, new string[] { "X16434" }, new string[] { "XFCMV14" }, new double[] { 54.545454545454 })]          // Happy : There are X records of the inputs out of Y records
        [InlineData("10B", 2020, 2019, false, new string[] { "X16434" }, new string[] { "XFCMV14" }, new double[] { 54.545454545454 })]         // Bad: invalid dates
        [InlineData("",2020, 2020, false, new string[] { "X16434" }, new string[] { "XFCMV14" }, new double[] { 54.545454545454 })]          // Bad: empty catalog                                                                                                                          
        public async void Boundaries_Scenarios_Test(string catalog, int startDate, int endDate, bool expectedResult, String[] testNames, double[] minValue, double[] maxValue, double[] average, double[] standardDeviation)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateBoundaries(catalog, new DateTime(startDate, 11, 15), new DateTime(endDate, 11, 30));
            Assert.Equal(expectedResult, queryResult.Status);
            if (expectedResult)
            {
                Assert.Equal(new string[] { "TestName", "Min", "Max", "Average", "StandardDeviation" }, queryResult.Data.ColumnNames);
                Assert.Equal(testNames.Length, queryResult.Data.Records.Count);
                var data = queryResult.Data;
                for (int i = 0; i < testNames.Length; i++)
                {
                    Assert.Equal(testNames[i], data.Records[i].TestName);
                    Assert.Equal(minValue[i], data.Records[i].Min);
                    Assert.Equal(maxValue[i], data.Records[i].Max);
                    Assert.Equal(average[i], data.Records[i].Average);
                    Assert.Equal(standardDeviation[i], data.Records[i].StandardDeviation);
                }
            }
        }
    }
}
