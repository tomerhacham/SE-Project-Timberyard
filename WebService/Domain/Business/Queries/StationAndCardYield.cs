using System;
using System.Threading.Tasks;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace WebService.Domain.Business.Queries
{
    public class StationAndCardYield : IQuery
    {
        public string Station { get; set; }
        public string Catalog { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public StationAndCardYield(string station, string catalog, DateTime startDate, DateTime endDate)
        {
            Station = station;
            Catalog = catalog;
            StartDate = startDate;
            EndDate = endDate;
        }
        /// <summary>
        /// Execution Station and Card Yield query and returns the result
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
