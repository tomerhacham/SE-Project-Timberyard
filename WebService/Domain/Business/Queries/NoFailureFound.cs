using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using WebService.Domain.DataAccess;
using WebService.Utils;

namespace WebService.Domain.Business.Queries
{
    public class NoFailureFound : IQuery
    {

        public string CardName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TimeInterval { get; set; }

        public NoFailureFound(string cardName, DateTime startDate, DateTime endDate, int timeInterval)
        {
            CardName = cardName;
            StartDate = startDate;
            EndDate = endDate;
            TimeInterval = timeInterval;
        }

        /// <summary>
        /// Executing NoFailureFound query and aggregate the raw results
        /// </summary>
        /// <param name="LogsAndTestsRepository"></param>
        /// <returns></returns>
        public async Task<Result<QueryResult>> Execute(ILogsAndTestsRepository LogsAndTestsRepository)
        {
            if (LogsAndTestsRepository == null)
            {
                return new Result<QueryResult>(false, null, "The logs and test repository was not entered\n");
            }
            var sqlResult = await LogsAndTestsRepository.ExecuteQuery(this);
            if (sqlResult.Status)
            {
                return await AggregateResults(sqlResult.Data);
            }
            return new Result<QueryResult>(false, null, sqlResult.Message);
        }

        /// <summary>
        /// Util function to perform aggregation of the test names which yield the failure.
        /// Thhe function will find all the records which return with the same LogId and makes a list of all the test names associate to it
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        private async Task<Result<QueryResult>> AggregateResults(List<dynamic> records)
        {
            dynamic InstanceExpandoObject(DateTime date, string cardName, string catalog, string station, string @operator, List<dynamic> failedTestNames)
            {
                dynamic obj = new ExpandoObject();
                obj.Date = date; obj.CardName = cardName; obj.Catalog = catalog; obj.Station = station; obj.Operator = @operator; obj.FailedTests = failedTestNames;
                return obj;
            }
            try
            {
                var logIds = records.AsParallel().Select(record => record.Id).Distinct().ToList();
                var aggregatedData = new ConcurrentBag<dynamic>();
                Parallel.ForEach(logIds, logId =>
                {
                    var sampleRecord = records.Where(record => record.Id == logId).First();
                    var date = sampleRecord.Date;
                    var cardName = sampleRecord.CardName;
                    var catalog = sampleRecord.Catalog;
                    var station = sampleRecord.Station;
                    var @operator = sampleRecord.Operator;
                    var failedTestNames = records.Where(record => record.Id == logId).Select(record => record.TestName).ToList(); //might be not distinct results
                    aggregatedData.Add(InstanceExpandoObject(date, cardName, catalog, station, @operator, failedTestNames));
                });
                return new Result<QueryResult>(true, new QueryResult(new string[] { "Date", "CardName", "Catalog", "Station", "Operator", "FailedTests" }, aggregatedData.ToList()), "\n");
            }
            catch (Exception exception)
            {
                return new Result<QueryResult>(false, null, exception.ToString());
            }
        }
    }
}
