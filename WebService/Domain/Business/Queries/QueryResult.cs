using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Domain.Business.Queries
{
    public class QueryResult
    {
        public string[] ColumnNames { get; set; }
        public List<dynamic> Records { get; set; }

        public QueryResult(List<dynamic> records)
        {
            ColumnNames = ((IDictionary<string, object>)records.FirstOrDefault()).Keys.ToArray();
            Records = records;
        }
        public QueryResult(string[] columnNames, List<object> record)
        {
            ColumnNames = columnNames;
            Records = record;
        }

    }
}
