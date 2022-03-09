using System;
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

        public Task<Result<QueryResult>> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            /*            Result<QueryResult> inputValidation = IsValidInputs(catalog, startDate, endDate);
                        if (!inputValidation.Status)
                        {
                            return inputValidation;
                        }*/

            throw new NotImplementedException();
        }

        public async Task<Result<QueryResult>> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate)
        {
            var query = new CardYield(catalog, startDate, endDate);
            return await query.Execute(LogsAndTestsRepository);
        }

        public async Task<Result<QueryResult>> CalculateStationsYield(DateTime startDate, DateTime endDate)
        {
            var query = new StationsYield(startDate, endDate);
            return await query.Execute(LogsAndTestsRepository);
        }
        public async Task<Result<QueryResult>> CalculateStationAndCardYield(string station, string catalog, DateTime startDate, DateTime endDate)
        {
            var query = new StationAndCardYield(station, catalog, startDate, endDate);
            return await query.Execute(LogsAndTestsRepository);
        }

    }
}
