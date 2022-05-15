using Dapper;
using ETL.Repository.DTO;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using WebService.Utils.Models;

namespace WebService.Domain.DataAccess
{
    public interface ILogsAndTestsRepository
    {
        Task<Result<List<dynamic>>> ExecuteQuery(CardYield cardYield);
        Task<Result<List<dynamic>>> ExecuteQuery(StationsYield stationsYield);
        Task<Result<List<dynamic>>> ExecuteQuery(NoFailureFound noFailureFound);
        Task<Result<List<dynamic>>> ExecuteQuery(StationAndCardYield stationAndCardYield);
        Task<Result<List<dynamic>>> ExecuteQuery(CardTestDuration cardTestDuration);
        Task<Result<List<dynamic>>> ExecuteQuery(TesterLoad testerLoad);
        Task<Result<List<dynamic>>> ExecuteQuery(Boundaries boundaries);
        Task<Result<List<LogDTO>>> GetAllLogsInTimeInterval(DateTime startTime, DateTime endTime);
    }

    public class LogsAndTestsRepository : ILogsAndTestsRepository
    {
        //Properties
        public DatabaseSettings DatabaseSettings { get; set; }
        public ILogger Logger { get; set; }

        //Constructor
        public LogsAndTestsRepository() { }

        public LogsAndTestsRepository(IOptions<DatabaseSettings> databaseSettings, ILogger logger)
        {
            DatabaseSettings = databaseSettings.Value;
            Logger = logger;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(DatabaseSettings.ConnectionString)
            {
                DataSource = DatabaseSettings.DbServer,
                Password = DatabaseSettings.DbPassword,
                UserID = DatabaseSettings.DbUsername
            };
            DatabaseSettings.ConnectionString = builder.ToString();
        }

        public async Task<List<dynamic>> DynamicReturnTypeExampleQuery()
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var sqlCommand = @"SELECT CardRev, CardName, SwRev From Logs WHERE Catalog=@Catalog";
                var objects = await connection.QueryAsync<dynamic>(sqlCommand, new { Catalog = "X56868" });
                return objects.AsList();
            }
            catch (Exception e)
            {
                return null;
            }
        }


        /// <summary>
        /// Execute Card Yield Query 
        /// </summary>
        /// <param name="cardYield">
        ///     Catalog:string
        ///     StartDate:DateTime
        ///     EndDate:DateTime
        /// </param>
        /// <returns>
        ///     [Catalog, CardName, SuccessRatio(%)]        
        /// </returns>
        public virtual async Task<Result<List<dynamic>>> ExecuteQuery(CardYield cardYield)
        {
            var sqlCommand =
                @"
                SELECT COALESCE (T1.Catalog,T2.Catalog) as Catalog, COALESCE(T1.CardName, T2.CardName) as CardName, CAST(((IsNull(SuccessTests, 0) * 100.0) / (IsNull(SuccessTests, 0) + IsNull(FailedTests, 0))) AS FLOAT) AS SuccessRatio
                FROM (
	                (SELECT Catalog, CardName, COUNT(*) as SuccessTests
                From Logs
                WHERE   Catalog=@Catalog AND
                        Logs.Date between @StartDate AND @EndDate AND
                        FinalResult = 'PASS' AND
                        ContinueOnFail = 'FALSE' AND
                        TECHMode = 'FALSE' AND
                        ABORT = 'FALSE' AND
                        SN != '0'
                GROUP BY Catalog, CardName 
                ) as T1 
                FULL JOIN
                (SELECT Catalog, CardName, COUNT(*) as FailedTests
                From Logs
                WHERE   Catalog=@Catalog AND
                        Logs.Date between @StartDate and @EndDate AND
                        FinalResult = 'FAIL' AND
                        ContinueOnFail = 'FALSE' AND
                        TECHMode = 'FALSE' AND
                        ABORT = 'FALSE' AND
                        SN != '0'
                GROUP BY Catalog, CardName
                ) as T2
	                ON T1.CardName = T2.CardName
                ) ";
            var queryParams = new { Catalog = cardYield.Catalog, StartDate = cardYield.StartDate, EndDate = cardYield.EndDate };
            return await ExecuteQuery<dynamic>(sqlCommand, queryParams);

        }
        /// <summary>
        /// Execute Stations Yield Query 
        /// </summary>
        /// <param name="stationsYield">
        ///     StartDate:DateTime
        ///     EndDate:DateTime
        /// </param>
        /// <returns>
        ///     [Station, SuccessRatio(%)]        
        /// </returns>
        public virtual async Task<Result<List<dynamic>>> ExecuteQuery(StationsYield stationsYield)
        {
            var sqlCommand =
                @"
                SELECT COALESCE (T1.Station,T2.Station) as Station, CAST(((IsNull(Success, 0) * 100.0) / (IsNull(Success, 0) + IsNull(Fail, 0))) AS FLOAT) AS SuccessRatio
                from
                (
	                (Select Logs.Station , count(*) as Success
                from Logs
                WHERE  Logs.Date between @StartDate AND @EndDate AND
                            FinalResult = 'PASS' AND
                            ContinueOnFail = 'FALSE' AND
                            TECHMode = 'FALSE' AND
                            ABORT = 'FALSE' AND
                            SN != '0'
                GROUP BY Station) as T1
	                full join 
	                (Select Logs.Station , count(*) as Fail
                from Logs
                WHERE  Logs.Date between @StartDate AND @EndDate AND
                            FinalResult = 'FAIL' AND
                            ContinueOnFail = 'FALSE' AND
                            TECHMode = 'FALSE' AND
                            ABORT = 'FALSE' AND
                            SN != '0'
                GROUP BY Station ) as T2
	                on T1.Station = T2.Station
                )
                ";
            var queryParams = new { StartDate = stationsYield.StartDate, EndDate = stationsYield.EndDate };
            return await ExecuteQuery<dynamic>(sqlCommand, queryParams);

        }
        /// <summary>
        /// Execute No Failure Found Query 
        /// </summary>
        /// <param name="noFailureFound">
        ///     CardName:string
        ///     StartDate:DateTime
        ///     EndDate:DateTime
        /// </param>
        /// <returns>
        ///     [Id, Date, Catalog, CardName, Station, Operator, TestName]        
        /// </returns>
        public virtual async Task<Result<List<dynamic>>> ExecuteQuery(NoFailureFound noFailureFound)
        {
            var sqlCommand =
                @"
                select Id,Date,Catalog,CardName,Station,Operator,TestName
                from 
                (	(select failLogs.Id as Id,failLogs.Catalog,failLogs.CardName,failLogs.Station,Operator,Date
                from (
	                (select * 
                from Logs
                where finalresult='FAIL' and
		                date between @StartDate and @EndDate and
                        ContinueOnFail = 'FALSE' AND
                        TECHMode = 'FALSE' AND
                        ABORT = 'FALSE' AND
                        SN != '0') as failLogs
	                join
	                (select distinct(SN), finalResult ,StartTime
                from Logs
                where finalresult='PASS' and
		                date between @StartDate and @EndDate and
                        ContinueOnFail = 'FALSE' AND
                        TECHMode = 'FALSE' AND
                        ABORT = 'FALSE' AND
                        SN != '0'
                ) as passLogs
	                on failLogs.SN=passLogs.SN 	and DATEDIFF(SECOND,failLogs.StartTime,passLogs.StartTime) between 0 and @TimeInterval

                )) as nffLogs
	                inner join
	                (select TestName,LogId from Tests) as failTests
	                on nffLogs.Id=failTests.LogId
                )
                where CardName=@CardName
                ";
            var queryParams = new { CardName = noFailureFound.CardName, StartDate = noFailureFound.StartDate, EndDate = noFailureFound.EndDate, TimeInterval = noFailureFound.TimeInterval };
            return await ExecuteQuery<dynamic>(sqlCommand, queryParams);

        }
        /// <summary>
        /// Execute Station and Card Yield Query 
        /// </summary>
        /// <param name="stationAndCardYield">
        ///     Station:string
        ///     Catalog:string
        ///     StartDate:DateTime
        ///     EndDate:DateTime
        /// </param>
        /// <returns>
        ///     [Catalog, CardName, SuccessRatio(%)]        
        /// </returns>
        public virtual async Task<Result<List<dynamic>>> ExecuteQuery(StationAndCardYield stationAndCardYield)
        {
            var sqlCommand =
                @"
                SELECT COALESCE (T1.Catalog,T2.Catalog) as Catalog, COALESCE(T1.CardName, T2.CardName) as CardName, CAST(((IsNull(SuccessTests, 0) * 100.0) / (IsNull(SuccessTests, 0) + IsNull(FailedTests, 0))) AS FLOAT) AS SuccessRatio
                FROM (
	                (SELECT Catalog, CardName, COUNT(*) as SuccessTests
                From Logs
                WHERE   Catalog=@Catalog AND
                        Station = @Station AND
                        Logs.Date between @StartDate AND @EndDate AND
                        FinalResult = 'PASS' AND
                        ContinueOnFail = 'FALSE' AND
                        TECHMode = 'FALSE' AND
                        ABORT = 'FALSE' AND
                        SN != '0'
                GROUP BY Catalog, CardName 
                ) as T1 
                FULL JOIN
                (SELECT Catalog, CardName, COUNT(*) as FailedTests
                From Logs
                WHERE   Catalog=@Catalog AND
                        Station = @Station AND
                        Logs.Date between @StartDate and @EndDate AND
                        FinalResult = 'FAIL' AND
                        ContinueOnFail = 'FALSE' AND
                        TECHMode = 'FALSE' AND
                        ABORT = 'FALSE' AND
                        SN != '0'
                GROUP BY Catalog, CardName
                ) as T2
	                ON T1.CardName = T2.CardName
                ) ";
            var queryParams = new { Station = stationAndCardYield.Station, Catalog = stationAndCardYield.Catalog, StartDate = stationAndCardYield.StartDate, EndDate = stationAndCardYield.EndDate };
            return await ExecuteQuery<dynamic>(sqlCommand, queryParams);

        }
        /// <summary>
        /// Execute Card Test Duration query
        /// </summary>
        /// <param name="cardTestDuration">
        ///     Catalog:string
        ///     StartDate:DateTime
        ///     EndDate:DateTime
        /// </param>
        /// <returns>
        ///     [Operator, NetTimeAvg, TotalTimeAvg]        
        /// </returns>
        public virtual async Task<Result<List<dynamic>>> ExecuteQuery(CardTestDuration cardTestDuration)
        {
            var sqlCommand =
                @"
                SELECT Logs.Operator, AVG(datediff(second, cast('00:00' as time(7)), Logs.NetTime)) as NetTimeAvg , AVG(datediff(second, Logs.StartTime, Logs.EndTime)) as TotalTimeAvg
                From Logs
                WHERE Catalog=@Catalog AND
                      Date between @StartDate AND @EndDate AND
					  FinalResult = 'PASS' AND
					  ContinueOnFail = 'FALSE' AND
					  TECHMode = 'FALSE' AND
					  ABORT = 'FALSE' AND
					  DBMode != 'BYPASS'
                GROUP BY Operator
                ORDER BY Operator DESC
                ";

            var queryParams = new { Catalog = cardTestDuration.Catalog, StartDate = cardTestDuration.StartDate, EndDate = cardTestDuration.EndDate };
            return await ExecuteQuery<dynamic>(sqlCommand, queryParams);
        }

        /// Execute Tester Load Query 
        /// </summary>
        /// <param name="testerLoad">
        ///     [Station, NumberOfRuns, TotalRunTimeHours]        
        /// </returns>
        public virtual async Task<Result<List<dynamic>>> ExecuteQuery(TesterLoad testerLoad)
        {
            var sqlCommand =
                @"
                SELECT Station, count(*) as NumberOfRuns, (sum(DATEDIFF(SECOND,StartTime,EndTime)) /3600.0) as TotalRunTimeHours
                from Logs
                where Logs.Date BETWEEN @StartDate AND @EndDate
                group by Station
                order by NumberOfRuns desc, TotalRunTimeHours desc";
            var queryParams = new { StartDate = testerLoad.StartDate, EndDate = testerLoad.EndDate };
            return await ExecuteQuery<dynamic>(sqlCommand, queryParams);

        }

        public virtual async Task<Result<List<dynamic>>> ExecuteQuery(Boundaries boundaries)
        {
            var sqlCommand =
                @"
                SELECT Tests.TestName, Tests.Min, Tests.Max, Tests.Received
                From Logs Join Tests
                On Tests.Type='Boundaries'
                Where Catalog=@Catalog AND Date BETWEEN @StartDate AND @EndDate
                ";
            var queryParams = new { Catalog = boundaries.Catalog, StartDate = boundaries.StartDate, EndDate = boundaries.EndDate };
            return await ExecuteQuery<dynamic>(sqlCommand, queryParams);

        }

        public virtual async Task<Result<List<LogDTO>>> GetAllLogsInTimeInterval(DateTime startTime, DateTime endTime)
        {
            var sqlCommand =
                @"
                SELECT *
                from Logs
                where Logs.EndTime BETWEEN @StartDate AND @EndDate";
            var queryParams = new { StartDate = startTime, EndDate = endTime };
            return await ExecuteQuery<LogDTO>(sqlCommand, queryParams);
        }

        /// <summary>
        /// Private function to wrap and handle execptions in query execution process
        /// </summary>
        /// <param name="sqlCommand">string represent the SQL command, can be parameterized</param>
        /// <param name="queryParams">object hold SQL query parameters</param>
        /// <returns></returns>
        private async Task<Result<List<T>>> ExecuteQuery<T>(string sqlCommand, object queryParams)
        {
/*            try
            {*/
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var objects = await connection.QueryAsync<T>(sqlCommand, queryParams);
                return new Result<List<T>>(true, objects.AsList());
/*            }
            catch (Exception e)
            {
                Logger.Warning("Exception in logs and test repository", e);
                return new Result<List<T>>(false, new List<T>(), "There was a problem with the DataBase");
            }*/
        }



    }
}
