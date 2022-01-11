using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebService.Domain.Business.Queries;
using WebService.Domain.Interface;
using WebService.Utils;

namespace AcceptanceTests.Utils
{
    public class SystemInterfaceProxy : ISystemInterface
    {
        public ISystemInterface Real { get; set; }

        public Result<QueryResult> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            if (Real == null)
                return new Result<QueryResult>(true, null, "");

            return new Result<QueryResult>(false, null, "");
        }
        public Result<QueryResult> CalculateCardYield(DateTime startDate, DateTime endDate, string catalog)
        {
            if (Real == null)
                return new Result<QueryResult>(false, null, "");

            return null;
        }
    }
}
