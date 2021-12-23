using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace WebService.Domain.Business.Queries
{
    public class QueriesController
    {
        ILogger Logger { get; }
        public LogsAndTestsRepository LogsAndTestsRepository { get; }
        public QueriesController(LogsAndTestsRepository logsAndTestsRepository, ILogger logger)
        {
            LogsAndTestsRepository = logsAndTestsRepository;
            Logger = logger;
        }

        public Result<QueryResult> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<QueryResult>> CalculateCardYield(DateTime startDate, DateTime endDate, string catalog)
        {
            var query = new CardYield(catalog, startDate, endDate);
            return await query.Execute(LogsAndTestsRepository);
        }
    }
}
