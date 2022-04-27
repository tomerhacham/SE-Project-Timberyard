using AcceptanceTests.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using WebService.Domain.Business.Queries;
using Xunit;

namespace AcceptanceTests.UseCases.QueryRelated
{
    public class TesterLoadTests : TimberyardTestCase
    {
        public TesterLoadTests() : base()
        { }

        [Theory]
        [InlineData(2021, 2022, HttpStatusCode.OK, new string[] { "L5", "11", "7", "2T", "2X", "L4" },                       // happy : there is data
           new int[] { 10, 10, 5, 3, 2, 2 },
           new double[] { 3.199444, 1.059444, 0.162500, 0.551388, 0.944722, 0.039166 })]
        [InlineData(2017, 2018, HttpStatusCode.OK, new string[] { }, new int[] { }, new double[] { })]                       // Happy: no records since no data between dates
        [InlineData(2022, 2021, HttpStatusCode.BadRequest, new string[] { }, new int[] { }, new double[] { })]               // Bad: invalid dates
        public async void TesterLoad_Scenarios_Test(int startDate, int endDate, HttpStatusCode expectedStatusCode, string[] stationNames, int[] numberOfRuns, double[] totalRunTime)
        {
            IRestResponse response = await Client.CalculateTesterLoad(new DateTime(startDate, 12, 01), new DateTime(endDate, 12, 01));
            Assert.Equal(expectedStatusCode, response.StatusCode);
            QueryResult queryResult = JsonConvert.DeserializeObject<QueryResult>(response.Content);
            string[] columnNames = queryResult.ColumnNames;
            List<dynamic> records = queryResult.Records;

            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                if (columnNames.Length > 0)
                {
                    Assert.Equal(new string[] { "Station", "NumberOfRuns", "TotalRunTimeHours" }, columnNames);
                }
                Assert.Equal(stationNames.Length, records.Count);

                for (int i = 0; i < records.Count; i++)
                {
                    string json = JsonConvert.SerializeObject(records[i]);
                    dynamic record = JsonConvert.DeserializeObject<ExpandoObject>(json);
                    Assert.Equal(stationNames[i], record.Station);
                    Assert.Equal(numberOfRuns[i], record.NumberOfRuns);
                    Assert.Equal(totalRunTime[i], record.TotalRunTimeHours);
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
