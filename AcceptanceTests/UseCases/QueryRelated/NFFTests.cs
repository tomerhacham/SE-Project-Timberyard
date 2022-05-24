using AcceptanceTests.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using Xunit;

namespace AcceptanceTests.UseCases.QueryRelated
{

    public class NFFTests : TimberyardTestCase
    {
        public NFFTests() : base()
        {
            Client.Authenticate().Wait();
        }

        [Theory]
        [InlineData("9901X_JTAG", 2021, 2022, HttpStatusCode.OK, 2, new string[] { "L5", "L5" }, new string[] { "75653", "73003" }, new string[] {   "CAPTURE",
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
        [InlineData("9901X_JTAG", 2017, 2018, HttpStatusCode.OK, 0, new string[] { }, new string[] { }, new string[] { })]                                                                         // Happy: no records since no data between dates
        [InlineData("X93677", 2021, 2022, HttpStatusCode.OK, 0, new string[] { }, new string[] { }, new string[] { })]                                                                         // Happy: no records since no catalog
        [InlineData("X93655", 2022, 2021, HttpStatusCode.BadRequest, 0, new string[] { }, new string[] { }, new string[] { })]                                                                         // Bad: Invalid dates
        [InlineData("", 2021, 2022, HttpStatusCode.BadRequest, 0, new string[] { }, new string[] { }, new string[] { })]                                                                            // Bad: Invalid catalog
        public async void NFF_Scenarios_Test(string cardName, int startDate, int endDate, HttpStatusCode expectedStatusCode, int expectedNumOfRecords, string[] stationNames, string[] operators, string[] failedTestNames)
        {
            IRestResponse response = await Client.CalculateNoFailureFound(cardName, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01), 5 * 60 * 60);
            Assert.Equal(expectedStatusCode, response.StatusCode);
            QueryResult queryResult = JsonConvert.DeserializeObject<QueryResult>(response.Content);
            string[] columnNames = queryResult.ColumnNames;
            List<dynamic> records = queryResult.Records;

            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                if (columnNames.Length > 0)
                {
                    Assert.Equal(new string[] { "Date", "CardName", "Catalog", "Station", "Operator", "FailedTests" }, columnNames);
                }
                Assert.Equal(expectedNumOfRecords, records.Count);

                for (int i = 0; i < records.Count; i++)
                {
                    string json = JsonConvert.SerializeObject(records[i]);
                    dynamic record = JsonConvert.DeserializeObject<ExpandoObject>(json);
                    Assert.Equal(stationNames[i], record.Station);
                    Assert.Equal(operators[i], record.Operator);
                    foreach (var testName in failedTestNames)
                    {
                        Assert.Contains(testName, record.FailedTests);
                    }
                }
            }

            if (response.StatusCode.Equals(HttpStatusCode.BadRequest))
            {
                Assert.Null(columnNames);
                Assert.Null(records);
            }
        }
    }
}
