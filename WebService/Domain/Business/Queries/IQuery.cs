using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace WebService.Domain.Business.Queries
{
    public interface IQuery
    {
        public Result<QueryResult> Execute(LogsAndTestsRepository LogsAndTestsRepository);
    }
}
