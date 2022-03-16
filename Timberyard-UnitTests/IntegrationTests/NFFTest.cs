using Microsoft.Extensions.DependencyInjection;
using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests.IntegrationTests
{
    public class NFFTest : TestSuit
    {
        // Properties
        public QueriesController QueriesController;

        // Constructor
        public NFFTest()
        {
            var serviceProvider = ConfigureServices("IntegrationTests");
            QueriesController = serviceProvider.GetService<QueriesController>();
        }

        [Theory]
        [InlineData("XMCP-B", 2020, 2020, true, 2, "2X", "71489", new string[] {   "NO INF ALARMS CHECK (INF A&B ON) - PC14",
                                                                                "NO INF ALARMS CHECK (INF A&B ON)-ALTERA",
                                                                                "INF A INPUT",
                                                                                "INF B INPUT ALARM CHECK (INF A ON , B OFF)- PC14",
                                                                                "INF B INPUT ALARM CHECK (INF A ON , B OFF)-ALTERA",
                                                                                "INF B INPUT",
                                                                                "INF A INPUT ALARM CHECK (INF A OFF , B ON)- PC14",
                                                                                "INF A INPUT ALARM CHECK (INF A OFF , B ON)-ALTERA",
                                                                                "MCP FLASH BOOT TEST",
                                                                                "NVM test" })]
        [InlineData("", 2020, 2020, false, 0, "2X", "71489", new string[] { "NO INF ALARMS CHECK (INF A&B ON) - PC14" })]
        public async void NFF_Scenarios_Test(string cardName, int startDate, int endDate, bool expectedResult, int expectedNumOfRecords, string stationNames, string operators, string[] failedTestNames)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateNFF(cardName, new DateTime(startDate, 11, 30), new DateTime(endDate, 11, 30));
            Assert.Equal(expectedResult, queryResult.Status);
            if (expectedResult)
            {
                Assert.Equal(new string[] { "Date", "CardName", "Catalog", "Station", "Operator", "FailedTests" }, queryResult.Data.ColumnNames);
                Assert.Equal(expectedNumOfRecords, queryResult.Data.Records.Count);
                var data = queryResult.Data;
                foreach (var record in data.Records)
                {
                    Assert.Equal(stationNames, record.Station);
                    Assert.Equal(operators, record.Operator);
                    foreach (var testName in failedTestNames)
                    {
                        Assert.Contains(testName, record.FailedTests);
                    }
                }
            }
        }
    }
}
