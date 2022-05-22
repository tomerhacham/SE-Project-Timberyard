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
    [Trait("Category", "Acceptance")]
    public class StationsYieldTests : TimberyardTestCase
    {
        public StationsYieldTests() : base()
        {
            Client.Authenticate().Wait();
        }

        [Theory]
        [InlineData(2021, 2022, HttpStatusCode.OK, new string[] { "11", "2T", "2X", "L4", "L5" }, new double[] { 60, 0, 50, 50, 70 })]                                       // Happy: there is data in these dates
        [InlineData(2017, 2018, HttpStatusCode.OK, new string[] { }, new double[] { })]                                                                                      // Happy: there is no data in these dates
        [InlineData(2022, 2021, HttpStatusCode.BadRequest, new string[] { }, new double[] { })]                                                                                     // Bad: Invalid dates
        public async void StationsYieldAcceptanceScenarios
            (int startDate, int endDate, HttpStatusCode expectedStatusCode, string[] stationNames, double[] SuccessRatioValues)
        {
            IRestResponse response = await Client.CalculateStationsYield(new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedStatusCode, response.StatusCode);

            QueryResult queryResult = JsonConvert.DeserializeObject<QueryResult>(response.Content);
            string[] columnNames = queryResult.ColumnNames;
            List<dynamic> records = queryResult.Records;

            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                if (columnNames.Length > 0)
                {
                    Assert.Equal(new string[] { "Station", "SuccessRatio" }, columnNames);
                }

                Assert.Equal(stationNames.Length, records.Count);

                for (int i = 0; i < records.Count; i++)
                {
                    string json = JsonConvert.SerializeObject(records[i]);
                    dynamic record = JsonConvert.DeserializeObject<ExpandoObject>(json);
                    Assert.Equal(stationNames[i], record.Station);
                    Assert.Equal(SuccessRatioValues[i], record.SuccessRatio);
                    Assert.True(0 <= record.SuccessRatio && record.SuccessRatio <= 100);
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
