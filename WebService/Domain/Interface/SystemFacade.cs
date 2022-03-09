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
    }
}
