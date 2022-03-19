using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using WebService.API.Controllers.Models;
using WebService.Domain.Business.Queries;

namespace WebService.API.Swagger.Example.QueriesController
{
    public class CardTestDurationRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new CardTestDurationModel() { Catalog = "X93655", StartDate = new DateTime(2021, 11, 11), EndDate = new DateTime(2021, 11, 20) };
        }
    }
    public class CardTestDurationResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var records = new List<object> { new {Operator= "73531", NetTimeAvg= 686, TotalTimeAvg=1696 },
            new {Operator= "73003", NetTimeAvg= 602, TotalTimeAvg=605 },
            new {Operator= "71220", NetTimeAvg= 605, TotalTimeAvg=607 }};
            var headers = new string[] { "Operator", "NetTimeAvg", "TotalTimeAvg" };
            return new QueryResult(headers, records);
        }
    }
}
