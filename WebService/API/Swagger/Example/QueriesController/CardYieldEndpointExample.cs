using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;

namespace WebService.API.Swagger.Example.QueriesController
{
    public class CardYieldRequestExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new CardYieldModel() { Catalog = "X56868", StartDate = DateTime.Now, EndDate = DateTime.Now };
        }
    }
    public class CardYieldResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new List<object> { new {Catalog= "X56868", CardName= "OA_HF", SuccessRatio=93.12 },
            new {Catalog= "X56868", CardName= "OP_KLF", SuccessRatio=95.2 },
            new {Catalog= "X56868", CardName= "OA_ASDF", SuccessRatio=89.2 }};
        }
    }
}
