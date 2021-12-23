using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Domain.Business.Queries
{
    interface IQuery
    {
        public Task<QueryResult> Execute();
        public Task<IQuery> Clone();
    }
}
