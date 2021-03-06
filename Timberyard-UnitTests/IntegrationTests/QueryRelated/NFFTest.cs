using Microsoft.Extensions.DependencyInjection;
using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using Xunit;

namespace Timberyard_UnitTests.IntegrationTests
{
    [Trait("Category", "Integration-Queries")]
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
        [InlineData("9901X_JTAG", 2021, 2022, true, 2, new string[] { "L5", "L5" }, new string[] { "75653", "73003" }, new string[] {   "CAPTURE",
                                                                                "IDENT",
                                                                                "Program_SFL3710",
                                                                                "QSPI Programming",
                                                                                "Program_SFL3710",
                                                                                "SHUTDOWN",
                                                                                "inter_U25_U54_U60_U61",
                                                                                "inter_U60_U120",
                                                                                "inter_U61_U120",
                                                                                "inter_U153_U120",
                                                                                "COMPARE UUT_CN",
                                                                                "COMPARE UUT OPTION",
                                                                                "COMPARE UUT_REV_VER",
                                                                                "Init Dynamic-File-Creator",
                                                                                "Create Dynamic-File-Creator",
                                                                                "Compile Bin File",
                                                                                "Program_ICL3710",
                                                                                "QSPI Programming",
                                                                                "inter_U25_U54_U60_U61",
                                                                                "inter_U60_U120",
                                                                                "Init Dynamic-File-Creator",
                                                                                "Create Dynamic-File-Creator"
                                                                                })]
        [InlineData("9901X_JTAG", 2017, 2018, true, 0, new string[] { }, new string[] { }, new string[] { })]                                                                         // Happy: no records since no data between dates
        [InlineData("X93677", 2021, 2022, true, 0, new string[] { }, new string[] { }, new string[] { })]                                                                         // Happy: no records since no catalog
        [InlineData("X93655", 2022, 2021, false, 0, new string[] { }, new string[] { }, new string[] { })]                                                                         // Bad: Invalid dates
        [InlineData("", 2021, 2022, false, 0, new string[] { }, new string[] { }, new string[] { })]                                                                            // Bad: Invalid catalog
        public async void NFF_Scenarios_Test(string cardName, int startDate, int endDate, bool expectedResult, int expectedNumOfRecords, string[] stationNames, string[] operators, string[] failedTestNames)
        {
            Result<QueryResult> queryResult = await QueriesController.CalculateNFF(cardName, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01), 5 * 60 * 60);
            Assert.Equal(expectedResult, queryResult.Status);
            if (expectedResult)
            {
                if (queryResult.Data.ColumnNames.Length > 0)
                {
                    Assert.Equal(new string[] { "Date", "CardName", "Catalog", "Station", "Operator", "FailedTests" }, queryResult.Data.ColumnNames);
                }
                Assert.Equal(expectedNumOfRecords, queryResult.Data.Records.Count);
                queryResult.Data.Records.Sort((r1, r2) => r2.Operator.CompareTo(r1.Operator));
                for (int i = 0; i < expectedNumOfRecords; i++)
                {
                    Assert.Equal(stationNames[i], queryResult.Data.Records[i].Station);
                    Assert.Equal(operators[i], queryResult.Data.Records[i].Operator);
                    foreach (var testName in failedTestNames)
                    {
                        Assert.Contains(testName, queryResult.Data.Records[i].FailedTests);
                    }
                }
            }
        }
    }
}
