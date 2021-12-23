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

        public async Result<QueryResult> Execute(LogsAndTestsRepository LogsAndTestsRepository)
        {
            var sql_result = await LogsAndTestsRepository.ExecuteQuery(this);
            if (sql_result.Count>0)
            {
                return new Result<QueryResult>(false, new QueryResult(), 
            }
            else { }
        }
    }
}
