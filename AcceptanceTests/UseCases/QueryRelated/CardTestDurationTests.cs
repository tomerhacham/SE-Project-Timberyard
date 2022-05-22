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
    public class CardTestDurationTests : TimberyardTestCase
    {
        public CardTestDurationTests() : base()
        {
            Client.Authenticate();
        }


        [Theory]
        [InlineData("X93655", 2021, 2022, HttpStatusCode.OK, 2, new string[] { "75653", "73003" }, new double[] { 1085, 2052 }, new double[] { 1086, 2053 })]                                          // Happy : There are X records of the inputs out of Y records ( where X==Y )
        [InlineData("X17837", 2021, 2022, HttpStatusCode.OK, 1, new string[] { "71220" }, new double[] { 814 }, new double[] { 815 })]                                                                 // Happy : There are X records of the inputs out of Y records ( where X<Y )
        [InlineData("X93677", 2021, 2022, HttpStatusCode.OK, 0, new string[] { }, new double[] { }, new double[] { })]                                                                                    // Happy : There are 0 records of the inputs out of Y records  ( catalog does not exists )      
        [InlineData("", 2021, 2022, HttpStatusCode.BadRequest, 0, new string[] { }, new double[] { }, new double[] { })]                                                                                    // Bad : Invalid catalog           
        [InlineData("X16434", 2022, 2021, HttpStatusCode.BadRequest, 0, new string[] { }, new double[] { }, new double[] { })]                                                                                    // Bad : invalid dates         
        public async void CardTestDuration_Scenarios_Test
            (string catalog, int startDate, int endDate, HttpStatusCode expectedStatusCode, int records_count, string[] Operators_results, double[] NetTimeAvg_results, double[] TotalTimeAvg_results)
        {
            IRestResponse response = await Client.CalculateCardTestDuration(catalog, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedStatusCode, response.StatusCode);
            QueryResult queryResult = JsonConvert.DeserializeObject<QueryResult>(response.Content);
            string[] columnNames = queryResult.ColumnNames;
            List<dynamic> records = queryResult.Records;

            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                if (columnNames.Length > 0)
                {
                    Assert.Equal(new string[] { "Operator", "NetTimeAvg", "TotalTimeAvg" }, columnNames);
                }
                Assert.Equal(records_count, records.Count);

                for (int i = 0; i < records.Count; i++)
                {
                    string json = JsonConvert.SerializeObject(records[i]);
                    dynamic record = JsonConvert.DeserializeObject<ExpandoObject>(json);
                    Assert.Equal(Operators_results[i], record.Operator);
                    Assert.Equal(NetTimeAvg_results[i], record.NetTimeAvg);
                    Assert.Equal(TotalTimeAvg_results[i], record.TotalTimeAvg);
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
