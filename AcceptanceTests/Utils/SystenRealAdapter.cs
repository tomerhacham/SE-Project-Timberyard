using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebService.Domain.Business.Queries;
using WebService.Domain.Interface;
using WebService.Utils;

namespace AcceptanceTests.Utils
{
    public class SystenRealAdapter : InitializeSystem, ISystemInterface
    {
        public SystemFacade system { set; get; }


        public SystenRealAdapter()
        {
            system = GetSystemFacade();
        }

        public Result<QueryResult> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            return new Result<QueryResult>(false, null, "");
        }
        public Result<QueryResult> CalculateCardYield(DateTime startDate, DateTime endDate, string catalog)
        {
            var fromSystem = system.CalculateCardYield(startDate, endDate, catalog);

            return fromSystem.Result;
        }
    }
}
