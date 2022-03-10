using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using WebService.API.Controllers.Models;
using WebService.Domain.Business.Queries;

namespace WebService.API.Swagger.Example.QueriesController
{
    public class StationAndCardYieldRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new StationAndCardYieldModel() { Station = "L4", Catalog = "X56490", StartDate = new DateTime(2021, 12, 21), EndDate = new DateTime(2022, 03, 09) };
        }
    }
    public class StationAndCardYieldResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var records = new List<object> { new { Catalog = "X56490", CardName = "AOC10_L2_JTAG", SuccessRatio = 56.231 } };
            var headers = new string[] { "Catalog", "CardName", "SuccessRatio" };
            return new QueryResult(headers, records);
        }
    }
}
