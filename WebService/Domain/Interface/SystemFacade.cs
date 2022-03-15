using System;
using System.Threading.Tasks;
using WebService.Domain.Business.Queries;
using WebService.Utils;

namespace WebService.Domain.Interface
{
    public class SystemFacade
    {
        public QueriesController QueriesController { get; }

        public SystemFacade(QueriesController queriesController)
        {
            QueriesController = queriesController;
        }

        public Task<Result<QueryResult>> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateBoundaries(catalog, startDate, endDate);
        }

        public Task<Result<QueryResult>> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateCardYield(catalog, startDate, endDate);
        }
        public Task<Result<QueryResult>> CalculateStationsYield(DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateStationsYield(startDate, endDate);
        }

        public Task<Result<QueryResult>> CalculateStationAndCardYield(string station, string catalog, DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateStationAndCardYield(station, catalog, startDate, endDate);
        }
        public Task<Result<QueryResult>> CalculateNFF(string cardName, DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateNFF(cardName, startDate, endDate);
        }
        public Task<Result<QueryResult>> CalculateCardTestDuration(string catalog, DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateCardTestDuration(catalog, startDate, endDate);
        }
    }
}
