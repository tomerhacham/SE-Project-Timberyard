using System.Collections.Generic;
using System.Linq;

namespace WebService.Domain.Business.Queries
{
    public class QueryResult
    {
        public string[] ColumnNames { get; set; }
        public List<dynamic> Records { get; set; }

        public QueryResult() { }

        public QueryResult(List<dynamic> records)
        {
            ColumnNames = (records.Count() > 0) ? ((IDictionary<string, object>)records.FirstOrDefault()).Keys.ToArray() : new string[0];
            Records = records;
        }
        public QueryResult(string[] columnNames, List<object> record)
        {
            ColumnNames = columnNames;
            Records = record;
        }

    }
}
