using AcceptanceTests.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using Xunit;

namespace AcceptanceTests
{
    [Trait("Category", "Acceptance")]
    public class CardYieldTests : TimberyardTestCase
    {

        public CardYieldTests() : base()
        {
            Client.Authenticate();
        }

        [Theory]
        [InlineData("X93655", 2021, 2022, HttpStatusCode.OK, 2, new string[] { "9901X_JTAG", "HIO400A_JTAG" }, new double[] { 70, 50 })]                              // Happy : There are X records of the inputs out of Y records 
        [InlineData("XH8655", 2021, 2022, HttpStatusCode.OK, 1, new string[] { "XMCP-B_HIK" }, new double[] { 0 })]                                                   // Happy : There are 0 success records of the inputs out of Y records
        [InlineData("X93677", 2021, 2022, HttpStatusCode.OK, 0, new string[] { }, new double[] { })]                                                                  // Happy : There are 0 records of the inputs out of Y records (catalog not exists)   
        [InlineData("", 2021, 2022, HttpStatusCode.BadRequest, 0, new string[] { }, new double[] { })]                                                                // Bad : invalid catalog         
        [InlineData("X93655", 2022, 2021, HttpStatusCode.BadRequest, 0, new string[] { }, new double[] { })]                                                          // Bad : invalid dates   // Bad : invalid dates         
        public async void CardYieldAcceptenceScenarioes
            (string catalog, int startDate, int endDate, HttpStatusCode expectedStatusCode, int records_count, string[] CardName_results, double[] SuccessRatio_results)
        {
            IRestResponse response = await Client.CalculateCardYield(catalog, new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
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
                Assert.Equal(records_count, records.Count);

                for (int i = 0; i < records_count; i++)
                {
                    string json = JsonConvert.SerializeObject(records[i]);
                    dynamic record = JsonConvert.DeserializeObject<ExpandoObject>(json);
                    Assert.Equal(catalog, record.Catalog);
                    Assert.Equal(CardName_results[i], record.CardName);
                    Assert.Equal(SuccessRatio_results[i], record.SuccessRatio);
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
