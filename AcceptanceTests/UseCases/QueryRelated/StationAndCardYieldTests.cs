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

    public class StationAndCardYieldTests : TimberyardTestCase
    {
        public StationAndCardYieldTests() : base()
        {
            Client.Authenticate().Wait();
        }

        [Theory]
        [InlineData("L5", "X93655", 2021, 2022, HttpStatusCode.OK, new string[] { "9901X_JTAG" }, new double[] { 70 })]                          // Happy : There are X records of the inputs out of Y records
        [InlineData("11", "X39337", 2021, 2022, HttpStatusCode.OK, new string[] { "RCP08_O_JTAG" }, new double[] { 80 })]                        // Happy : There are X records of the inputs out of Y records
        [InlineData("L", "X93655", 2021, 2022, HttpStatusCode.OK, new string[] { }, new double[] { })]                                           // Happy : There are no records since station name does not exsists
        [InlineData("L5", "X93677", 2021, 2022, HttpStatusCode.OK, new string[] { }, new double[] { })]                                          // Happy : There are no records since catalog does not exsists
        [InlineData("L5", "X93655", 2017, 2018, HttpStatusCode.OK, new string[] { }, new double[] { })]                                          // Happy : There are no records since catalog does not exsists
        [InlineData("L5", "X93655", 2022, 2021, HttpStatusCode.BadRequest, new string[] { }, new double[] { })]               // Bad: invalid dates
        [InlineData("10B", "", 2021, 2022, HttpStatusCode.BadRequest, new string[] { }, new double[] { })]                // Bad: invalid catalog
        [InlineData("", "X16434", 2021, 2022, HttpStatusCode.BadRequest, new string[] { }, new double[] { })]             // Bad: invalid station

        public async void StationAndCardYieldAcceptanceScenarios
            (string station, string catalog, int startDate, int endDate, HttpStatusCode expectedStatusCode, string[] cardNames, double[] SuccessRatioValues)
        {
            IRestResponse response = await Client.CalculateStationAndCardYield(station, catalog, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedStatusCode, response.StatusCode);
            QueryResult queryResult = JsonConvert.DeserializeObject<QueryResult>(response.Content);
            string[] columnNames = queryResult.ColumnNames;
            List<dynamic> records = queryResult.Records;

            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                if (columnNames.Length > 0)
                {
                    Assert.Equal(new string[] { "Catalog", "CardName", "SuccessRatio" }, columnNames);
                }

                Assert.Equal(cardNames.Length, records.Count);

                for (int i = 0; i < records.Count; i++)
                {
                    string json = JsonConvert.SerializeObject(records[i]);
                    dynamic record = JsonConvert.DeserializeObject<ExpandoObject>(json);
                    Assert.Equal(cardNames[i], record.CardName);
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
