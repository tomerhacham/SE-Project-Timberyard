using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using WebService.Domain.DataAccess;
using WebService.Utils;
using WebService.Utils.ExtentionMethods;

namespace WebService.Domain.Business.Queries
{
    public class Boundaries : IQuery
    {
        public string Catalog { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Boundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            Catalog = catalog;
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Execute Boundaries query and aggregate the raw results
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
        /// Util function to aggreate raw results returned from the persistance and calcualte Std, Avg and selection all tset names which related to the query
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        private async Task<Result<QueryResult>> AggregateResults(List<dynamic> records)
        {
            dynamic InstanceExpandoObject(string TestName, double Min, double Max, double average, double standardDeviation, List<double> Received)
            {
                dynamic obj = new ExpandoObject();
                obj.TestName = TestName; obj.Min = Min; obj.Max = Max; obj.Average = average; obj.StandardDeviation = standardDeviation; obj.Received = Received;
                return obj;
            }
            try
            {
                var testNames = records.AsParallel().Select(record => record.TestName).Distinct().ToList();
                var aggregatedData = new ConcurrentBag<dynamic>();
                Parallel.ForEach(testNames, test =>
                 {
                     var matchingRecords = records.AsParallel().Where(record => record.TestName == test);
                     var min = matchingRecords.First().Min;
                     var max = matchingRecords.First().Max;
                     List<double> receivedValues = matchingRecords.AsParallel().Select(record => record.Received).Cast<double>().ToList();
                     var stdDev = receivedValues.StdDev();
                     var avg = receivedValues.Average();
                     aggregatedData.Add(InstanceExpandoObject(test, min, max, avg, stdDev, receivedValues));

                 });
                return new Result<QueryResult>(true, new QueryResult(new string[] { "TestName", "Min", "Max", "Average", "StandardDeviation", "Received" }, aggregatedData.ToList()), "\n");
            }
            catch (Exception exception)
            {
                return new Result<QueryResult>(false, null, exception.ToString());
            }
        }
    }
}
