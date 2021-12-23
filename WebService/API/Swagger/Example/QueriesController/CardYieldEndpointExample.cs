using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.Domain.Business.Queries;

namespace WebService.API.Swagger.Example.QueriesController
{
    public class CardYieldRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new CardYieldModel() { Catalog = "X56868", StartDate = new DateTime(2021, 10, 19), EndDate = new DateTime(2021, 10, 19) };
        }
    }
    public class CardYieldResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var records = new List<object> { new {Catalog= "X56868", CardName= "OA_HF", SuccessRatio=93.12 },
            new {Catalog= "X56868", CardName= "OP_KLF", SuccessRatio=95.2 },
            new {Catalog= "X56868", CardName= "OA_ASDF", SuccessRatio=89.2 }};
            var headers = new string[] { "Catalog", "CardName", "SuccessRatio" };
            return new QueryResult(headers, records);
        }
    }
}
