using System;
using System.Threading.Tasks;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace WebService.Domain.Business.Queries
{
    public class StationsYield : IQuery
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public StationsYield(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Execute Stations yield and returns the result
        /// </summary>
        /// <param name="LogsAndTestsRepository"></param>
        /// <returns></returns>
        public async Task<Result<QueryResult>> Execute(ILogsAndTestsRepository LogsAndTestsRepository)
        {
            if (LogsAndTestsRepository == null)
            {
                return new Result<QueryResult>(false, null, "The logs and test repository was not entered\n");
            }
            var sqlResult = await LogsAndTestsRepository.ExecuteQuery(this);
            if (sqlResult.Status)
            {
                return new Result<QueryResult>(true, new QueryResult(sqlResult.Data), "\n");
            }
            return new Result<QueryResult>(false, null, sqlResult.Message);
        }
    }
}
