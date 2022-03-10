﻿using System;
using System.Collections.Generic;
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

        public NoFailureFound(string cardName, DateTime startDate, DateTime endDate)
        {
            CardName = cardName;
            StartDate = startDate;
            EndDate = endDate;
        }

        public async Task<Result<QueryResult>> Execute(LogsAndTestsRepository LogsAndTestsRepository)
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
        private async Task<Result<QueryResult>> AggregateResults(List<dynamic> records)
        {
            var logIds = records.Select(record => record.Id).Distinct().ToList();
            var aggregatedData = new List<dynamic>();
            foreach (var logId in logIds)
            {
                var sampleRecord = records.Where(record => record.Id == logId).First();
                var date = sampleRecord.Date;
                var cardName = sampleRecord.CardName;
                var catalog = sampleRecord.Catalog;
                var station = sampleRecord.Station;
                var @operator = sampleRecord.Operator;
                var failedTestNames = records.Where(record => record.Id == logId).Select(record => record.TestName).Distinct().ToList();
                aggregatedData.Add(new { Date = date, CardName = cardName, Catalog = catalog, Station = station, Operator = @operator, FailedTests = failedTestNames });
            }
            return new Result<QueryResult>(true, new QueryResult(aggregatedData, new string[] { "Date", "CardName", "Catalog", "Station", "Operator", "FailedTests" }), "\n");

        }
    }
}
