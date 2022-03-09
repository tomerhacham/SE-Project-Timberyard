using System;
using WebService.Domain.Business.Queries;
using WebService.Utils;

namespace WebService.Domain.Interface
{
    public interface ISystemInterface
    {
        public Result<QueryResult> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate);
        public Result<QueryResult> CalculateCardYield(DateTime startDate, DateTime endDate, string catalog);
    }
}
