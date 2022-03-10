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

        // TODO - change inputs according to db
        [Theory]
        [InlineData("X16434", 2000, 2001, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]        // Happy : There are X records of the inputs out of Y records ( where X==Y )
        [InlineData("", 2010, 2011, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]                // Happy : There are X records of the inputs out of Y records ( where X<Y )
        [InlineData("X16434", 2001, 2000, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]          // Happy : There are 0 records of the inputs out of Y records        
        [InlineData("", 2000, 2001, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]                // Bad : There are 0 records of the inputs out of 0 records ( catalog does not exists )         
        [InlineData("", 2001, 2000, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]                // Bad : invalid catalog         
        [InlineData("X16434", 2011, 2000, true, 3, new string[] { "OA_HF", "OP_KLF", "OA_ASDF" }, new double[] { 93.12, 95.2, 89.2 })]                // Bad : invalid dates   
        public async void StationAndCardYield_Scenarios_Test (string station, string catalog, int startDate, int endDate,bool expectedResult,string[] stationNames,double[] SuccessRatioValues)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateStationAndCardYield( new DateTime(startDate, 01, 01), new DateTime(endDate, 01, 03));
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
