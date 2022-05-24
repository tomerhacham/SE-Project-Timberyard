using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using WebService.API.Controllers.Models;
using WebService.Domain.Business.Queries;

namespace WebService.API.Swagger.Example.QueriesController
{
    public class TesterLoadRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new DatesModel() { StartDate = new DateTime(2021, 01, 01), EndDate = new DateTime(2021, 01, 03) };
        }
    }

    public class TesterLoadResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var records = new List<object> {
                new {Station = "1T", NumberOfRuns=20 ,TotalRunTimeHours=10.440277 },
                new {Station = "L4", NumberOfRuns=19 ,TotalRunTimeHours=2.946944 },
                new {Station = "04", NumberOfRuns=14 ,TotalRunTimeHours=3.220833 }};

            var headers = new string[] { "Station", "NumberOfRuns", "TotalRunTimeHours" };
            return new QueryResult(headers, records);
        }
    }

}
