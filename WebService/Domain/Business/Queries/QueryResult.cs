using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Domain.Business.Queries
{
    public class QueryResult
    {
        public string[] ColumnNames { get; set; }
        public List<object> Records { get; set; }

        public QueryResult(string[] columnNames, List<object> records)
        {
            ColumnNames = columnNames;
            Records = records;
        }

    }
}
