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
            Result<QueryResult>? result = default;
            var sqlResult = await LogsAndTestsRepository.ExecuteQuery(this);
            sqlResult.ContinueWith(
                success: (List<dynamic> data) =>
                     {
                         var columnNames = ((IDictionary<string, object>)data.FirstOrDefault()).Keys.ToArray();
                         var queryResult = new QueryResult(columnNames, data);
                         result = new Result<QueryResult>(true, queryResult, "");
                     },
                fail: (string message) =>
                    {
                        result = new Result<QueryResult>(false, null, message);
                    });
            return result;
        }
    }
}
