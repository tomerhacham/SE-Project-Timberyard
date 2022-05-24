using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using WebService.API.Controllers.Models;
using WebService.Domain.Business.Queries;

namespace WebService.API.Swagger.Example.QueriesController
{
    public class BoundariesRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new CatalogWithDatesModel() { Catalog = "X39337", StartDate = new DateTime(2021, 12, 22), EndDate = new DateTime(2022, 02, 22) };
        }
    }
    public class BoundariesResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var records = new List<object> { new { TestName = "Boundaries_Test1", Min = 2, Max = 6, Average = 4, StandardDeviation = 1.41421, Received = (2, 4, 3, 5, 6) },
            new { TestName = "Boundaries_Test2", Min = 1, Max = 4, Average = 2.4, StandardDeviation = 0.749577, Received = (1, 4, 3) }};
            var headers = new string[] { "TestName", "Min", "Max", "Average", "StandardDeviation", "Received" };
            return new QueryResult(headers, records);
        }
    }
}
