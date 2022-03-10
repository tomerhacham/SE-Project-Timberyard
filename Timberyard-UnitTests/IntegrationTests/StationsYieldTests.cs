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
        [InlineData(2021, 2021,true,new string[] {"04","11","1P","1T","2L","7S","8D","B2","C2","L4" },new double[] {100,91.666666666666,0,94.736842105263,0,0,100,33.333333333333,100,60 } )]
        [InlineData(2021, 2020,false,new string[] {},new double[] {} )]
        public async void StationsYield_Scenarios_Test (int startDate, int endDate,bool expectedResult,string[] stationNames,double[] SuccessRatioValues)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateStationsYield( new DateTime(startDate, 01, 01), new DateTime(endDate, 01, 03));
            Assert.Equal(expectedResult, queryResult.Status);
            if (expectedResult)
            {
                Assert.Equal(new string[] { "Station", "SuccessRatio" }, queryResult.Data.ColumnNames);
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
