using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace WebService.Domain.Business.Queries
{
    public class QueriesController
    {
        ILogger Logger { get; }
        public ILogsAndTestsRepository LogsAndTestsRepository { get; }
        public QueriesController(ILogsAndTestsRepository logsAndTestsRepository, ILogger logger)
        {
            LogsAndTestsRepository = logsAndTestsRepository;
            Logger = logger;
        }

        public async Task<Result<QueryResult>> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            Result<QueryResult> inputValidation = IsValidInputs(startDate, endDate, catalog: catalog);
            if (!inputValidation.Status)
            {
                return inputValidation;
            }
            var query = new Boundaries(catalog, startDate, endDate);
            Result<QueryResult> query_result = await query.Execute(LogsAndTestsRepository);
            AddInfoToLogger(query.GetType().Name, query_result);
            return query_result;
        }

        public async Task<Result<QueryResult>> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate)
        {
            Result<QueryResult> inputValidation = IsValidInputs(startDate, endDate, catalog: catalog);
            if (!inputValidation.Status)
            {
                return inputValidation;
            }
            var query = new CardYield(catalog, startDate, endDate);
            Result<QueryResult> query_result = await query.Execute(LogsAndTestsRepository);
            AddInfoToLogger(query.GetType().Name, query_result);
            return query_result;
        }

        public async Task<Result<QueryResult>> CalculateStationsYield(DateTime startDate, DateTime endDate)
        {
            Result<QueryResult> inputValidation = IsValidInputs(startDate, endDate);
            if (!inputValidation.Status)
            {
                return inputValidation;
            }
            var query = new StationsYield(startDate, endDate);
            Result<QueryResult> query_result = await query.Execute(LogsAndTestsRepository);
            AddInfoToLogger(query.GetType().Name, query_result);
            return query_result;
        }
        public async Task<Result<QueryResult>> CalculateStationAndCardYield(string station, string catalog, DateTime startDate, DateTime endDate)
        {
            Result<QueryResult> inputValidation = IsValidInputs(startDate, endDate, catalog: catalog, station: station);
            if (!inputValidation.Status)
            {
                return inputValidation;
            }
            var query = new StationAndCardYield(station, catalog, startDate, endDate);
            Result<QueryResult> query_result = await query.Execute(LogsAndTestsRepository);
            AddInfoToLogger(query.GetType().Name, query_result);
            return query_result;
        }
        public async Task<Result<QueryResult>> CalculateNFF(string cardName, DateTime startDate, DateTime endDate, int timeInterval)
        {
            Result<QueryResult> inputValidation = IsValidInputs(startDate, endDate, cardName: cardName);
            if (!inputValidation.Status)
            {
                return inputValidation;
            }
            var query = new NoFailureFound(cardName, startDate, endDate, timeInterval);
            Result<QueryResult> query_result = await query.Execute(LogsAndTestsRepository);
            AddInfoToLogger(query.GetType().Name, query_result);
            return query_result;
        }
        public async Task<Result<QueryResult>> CalculateTesterLoad(DateTime startDate, DateTime endDate)
        {
            Result<QueryResult> inputValidation = IsValidInputs(startDate, endDate);
            if (!inputValidation.Status)
            {
                return inputValidation;
            }
            var query = new TesterLoad(startDate, endDate);
            Result<QueryResult> query_result = await query.Execute(LogsAndTestsRepository);
            AddInfoToLogger(query.GetType().Name, query_result);
            return query_result;
        }

        public async Task<Result<QueryResult>> CalculateCardTestDuration(string catalog, DateTime startDate, DateTime endDate)
        {
            Result<QueryResult> inputValidation = IsValidInputs(startDate, endDate, catalog: catalog);
            if (!inputValidation.Status)
            {
                return inputValidation;
            }
            var query = new CardTestDuration(catalog, startDate, endDate);
            Result<QueryResult> query_result = await query.Execute(LogsAndTestsRepository);
            AddInfoToLogger(query.GetType().Name, query_result);
            return query_result;
        }

        private Result<QueryResult> IsValidInputs(DateTime startDate, DateTime endDate, [Optional] string catalog, [Optional] string station, [Optional] string cardName)
        {

            if (catalog != null && catalog == "")
            {
                Logger.Warning("An invalid catalog was entered while attempting to execute the query", null, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<QueryResult>(false, null, "Invalid catalog name\n");
            }
            if (cardName != null && cardName == "")
            {
                Logger.Warning("An invalid card name was entered while attempting to execute the query", null, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<QueryResult>(false, null, "Invalid card name\n");
            }
            if (station != null && station == "")
            {
                Logger.Warning("An invalid station was entered while attempting to execute the query", null, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<QueryResult>(false, null, "Invalid station name\n");
            }
            if (startDate > endDate)
            {
                Logger.Warning("An invalid range of dates was entered while attempting to execute the query", null, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<QueryResult>(false, null, "Invalid range of dates\n");
            }
            return new Result<QueryResult>(true, null, "All inputs are valid\n");
        }

        private void AddInfoToLogger(string query, Result<QueryResult> query_result)
        {
            if (!query_result.Status)
            {
                Logger.Warning($"Attempting to execute the query {query} failed. {query_result.Message} ", null, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
            }
        }

    }
}
