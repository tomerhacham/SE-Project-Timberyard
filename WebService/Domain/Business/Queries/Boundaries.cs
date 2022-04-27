using System;
using System.Threading.Tasks;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace WebService.Domain.Business.Queries
{
    public class Boundaries : IQuery
    {
        string Catalog { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }

        public Task<Result<QueryResult>> Execute(ILogsAndTestsRepository LogsAndTestsRepository)
        {
            throw new NotImplementedException();
        }
    }
}
