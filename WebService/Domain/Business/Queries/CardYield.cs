using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace WebService.Domain.Business.Queries
{
    public class CardYield : IQuery
    {
        public string Catalog { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public CardYield(string catalog, DateTime startDate, DateTime endDate)
        {
            Catalog = catalog;
            StartDate = startDate;
            EndDate = endDate;
        }

        public async Task<Result<QueryResult>> Execute(LogsAndTestsRepository LogsAndTestsRepository)
        {
            var sqlResult = await LogsAndTestsRepository.ExecuteQuery(this);
            if (sqlResult.Status)
            {
                return new Result<QueryResult>(true, new QueryResult(sqlResult.Data), "");
            }
            return new Result<QueryResult>(false, null, sqlResult.Message);
        }
    }
}
