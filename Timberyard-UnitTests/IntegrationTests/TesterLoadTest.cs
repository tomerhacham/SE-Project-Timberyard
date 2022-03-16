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
        [InlineData(2021, 2021, true, new string[] { "1T","L4","04","2L","1P","11","8D","19","B2","C2","7S","1Y"},
            new int[] { 20,19,14,14,13,13,9,7,6,4,1,1},
            new double[] {10.440277,2.946944,3.220833,0.803333,2.149722,1.568055,5.912222,0.413888,0.586666,1.570277,0.718333,0.419166 })]
        [InlineData(2021, 2020, false, new string[] { }, new int[] { }, new double[] { })]
        public async void TesterLoad_Scenarios_Test(int startDate, int endDate, bool expectedResult, string[] stationNames, int[] numberOfRuns, double[] totalRunTime)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateTesterLoad(new DateTime(startDate, 01, 01), new DateTime(endDate, 01, 03));
            Assert.Equal(expectedResult, queryResult.Status);
            if (expectedResult)
            {
                Assert.Equal(new string[] { "Station", "NumberOfRuns", "TotalRunTimeHours" }, queryResult.Data.ColumnNames);
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
