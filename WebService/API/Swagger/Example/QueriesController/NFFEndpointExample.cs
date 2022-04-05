using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using WebService.API.Controllers.Models;
using WebService.Domain.Business.Queries;

namespace WebService.API.Swagger.Example.QueriesController
{
    public class NFFRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new NoFailureFoundModel() { CardName = "XMCP-B", StartDate = new DateTime(2020, 11, 30), EndDate = new DateTime(2022, 11, 30), TimeInterval = 5 * 60 * 60 };
        }
    }
    public class NFFResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var sampleRecord = new {
                Date = new DateTime(2020, 11, 30),
                CardName = "XMCP-B",
                Catalog = "X74216",
                Station = "2X",
                Operator = "71489",
                FailedTests = new List<string>() {  "XMCP-B UUT LOAD",
                                                    "XMCP Rom Boot Version test",
                                                    "APLICATION Version test",
                                                    "MCP Serial Number Check",}
            };
            var records = new List<object> { sampleRecord };
            var headers = new string[] { "Date", "CardName", "Catalog", "Station", "Operator", "FailedTests" };
            return new QueryResult(headers, records);
        }
    }
}
