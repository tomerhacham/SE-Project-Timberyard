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
        Dictionary<Type, IQuery> QueriesPrototypes { get; }
        public ILogsAndTestsRepository LogsAndTestsRepository { get; }
        public QueriesController(ILogsAndTestsRepository logsAndTestsRepository, ILogger logger)
        {
            LogsAndTestsRepository = logsAndTestsRepository;
            Logger = logger;
            QueriesPrototypes = new Dictionary<Type, IQuery>() { { typeof(Boundaries), new Boundaries() } };
        }

        internal Result<QueryResult> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}
