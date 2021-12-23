using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Domain.Business.Queries
{
    public class QueryResult
    {
        public List<string> ColumnNames { get; set; }
        public List<object> Records { get; set; }
    }
}
