﻿using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebService.Domain.Business.Queries;
using WebService.Utils;
using WebService.Utils.Models;

namespace WebService.Domain.DataAccess
{
    public class LogsAndTestsRepository
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
        ///     [Catalog, CardName, NumberSuccessedTests(%)]        
        /// </returns>
        public virtual async Task<Result<List<dynamic>>> ExecuteQuery(CardYield cardYield)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var sqlCommand =
                @"
                SELECT COALESCE (T1.Catalog,T2.Catalog) as Catalog, COALESCE(T1.CardName, T2.CardName) as CardName, CAST(((IsNull(SuccessTests, 0) * 100.0) / (IsNull(SuccessTests, 0) + IsNull(FailedTests, 0))) AS FLOAT) AS SuccessRatio
                FROM (
	                (SELECT Catalog, CardName, COUNT(*) as SuccessTests
                From Logs
                WHERE Catalog=@Catalog AND Logs.Date between @StartDate and @EndDate AND FinalResult = 'PASS'
                GROUP BY Catalog, CardName 
                ) as T1 
                FULL JOIN
                (SELECT Catalog, CardName, COUNT(*) as FailedTests
                From Logs
                WHERE Catalog=@Catalog AND Logs.Date between @StartDate and @EndDate AND FinalResult = 'FAIL'
                GROUP BY Catalog, CardName
                ) as T2
	                ON T1.CardName = T2.CardName
                ) ";
                var objects = await connection.QueryAsync<dynamic>(sqlCommand,
                    new { Catalog = cardYield.Catalog, StartDate = cardYield.StartDate, EndDate = cardYield.EndDate });

                /*if (objects.Count() == 0)
                {
                    return new Result<List<dynamic>>(true, new List<dynamic>(), "No data was found");
                }
                */
                return new Result<List<dynamic>>(true, objects.AsList());
            }
            catch (Exception e)
            {
                return new Result<List<dynamic>>(false, new List<dynamic>(), "There was a problem with the DataBase");
            }

        }
    }
}
