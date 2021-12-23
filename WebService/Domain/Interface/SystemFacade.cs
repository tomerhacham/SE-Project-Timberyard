using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Authentication;
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

        public Result<QueryResult> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateBoundaries(catalog, startDate, endDate);
        }

        public Task<Result<QueryResult>> CalculateCardYield(DateTime startDate, DateTime endDate, string catalog)
        {
            return QueriesController.CalculateCardYield(startDate, endDate, catalog);
        }
    }
}
