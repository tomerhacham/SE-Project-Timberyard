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

    public class BoundariesTests : TimberyardTestCase
    {
        public BoundariesTests() : base()
        {
            Client.Authenticate().Wait();
        }

        [Theory]
        [InlineData("X39337", 2021, 2022, HttpStatusCode.OK, new string[] { "Boundaries_Test2", "Boundaries_Test1" }, new double[] { 2, 1 }, new double[] { 6, 4 }, new double[] { 4, 2.24 }, new double[] { 1.41421, 0.749577 })]          // Happy : There are X records of the inputs out of Y records
        [InlineData("X12345", 2021, 2022, HttpStatusCode.OK, new string[] { }, new double[] { }, new double[] { }, new double[] { }, new double[] { })]                             // Happy: No records for the given catalog                                                                                                                          
        [InlineData("X39337", 2020, 2019, HttpStatusCode.BadRequest, new string[] { }, new double[] { }, new double[] { }, new double[] { }, new double[] { })]                     // Bad: invalid dates
        [InlineData("", 2021, 2022, HttpStatusCode.BadRequest, new string[] { }, new double[] { }, new double[] { }, new double[] { }, new double[] { })]                           // Bad: empty catalog

        public async void Boundaries_Scenarios_Test(string catalog, int startDate, int endDate, HttpStatusCode expectedStatusCode, String[] testNames, double[] minValue, double[] maxValue, double[] average, double[] standardDeviation)
        {
            IRestResponse response = await Client.CalculateBoundaries(catalog, new DateTime(startDate, 11, 15), new DateTime(endDate, 01, 22));
            Assert.Equal(expectedStatusCode, response.StatusCode);
            QueryResult queryResult = JsonConvert.DeserializeObject<QueryResult>(response.Content);
            string[] columnNames = queryResult.ColumnNames;
            List<dynamic> records = queryResult.Records;

            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                if (columnNames.Length > 0)
                {
                    Assert.Equal(new string[] { "TestName", "Min", "Max", "Average", "StandardDeviation", "Received" }, columnNames);
                }

                Assert.Equal(testNames.Length, records.Count);
                for (int i = 0; i < records.Count; i++)
                {
                    string json = JsonConvert.SerializeObject(records[i]);
                    dynamic record = JsonConvert.DeserializeObject<ExpandoObject>(json);
                    Assert.Equal(testNames[i], record.TestName);
                    Assert.Equal(minValue[i], record.Min);
                    Assert.Equal(maxValue[i], record.Max);
                    Assert.Equal(average[i], record.Average, 2);
                    Assert.Equal(standardDeviation[i], record.StandardDeviation, 5);
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
