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
            Result<QueryResult> inputValidation = IsValidInputs(catalog, startDate, endDate);
            if (!inputValidation.Status)
            {
                return inputValidation;
            }

            throw new NotImplementedException();
        }

        public async Task<Result<QueryResult>> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate)
        {
            Result<QueryResult> inputValidation = IsValidInputs(catalog, startDate, endDate);
            if (!inputValidation.Status)
            {
                return inputValidation;
            }
            var query = new CardYield(catalog, startDate, endDate);
            return await query.Execute(LogsAndTestsRepository);
        }

        private Result<QueryResult> IsValidInputs(string catalog, DateTime startDate, DateTime endDate)
        {
            if (catalog == "")
            {
                return new Result<QueryResult>(false, null, "Invalid catalog name\n");
            }
            if (startDate > endDate)
            {
                return new Result<QueryResult>(false, null, "Invalid range of dates\n");
            }
            return new Result<QueryResult>(true, null, "All inputs are valid\n");
        }
    }
}
