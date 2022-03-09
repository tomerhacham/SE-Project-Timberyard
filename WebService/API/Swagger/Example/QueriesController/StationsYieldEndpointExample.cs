using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using WebService.API.Controllers.Models;
using WebService.Domain.Business.Queries;

namespace WebService.API.Swagger.Example.QueriesController
{
    public class StationsYieldRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new StationsYieldModel() { StartDate = new DateTime(2021, 10, 19), EndDate = new DateTime(2021, 10, 19) };
        }

    }

    public class StationsYieldResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var records = new List<object> {
                new {Station = "04", SuccessRatio=33.333333333333 },
                new {Station = "10C", SuccessRatio=76.470588235294 },
                new {Station = "10D", SuccessRatio=100 }};

            var headers = new string[] { "Station", "SuccessRatio" };
            return new QueryResult(headers, records);
        }
    }

}
