﻿using Dapper.Contrib.Extensions;
using ETL.DataObjects;
using ETL.Repository.DTO;
using ETL.Utils;
using ETL.Utils.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Repository
{
    public interface IRepository
    {
        public Result<bool> InsertLog(Log log);
    }
    public class Repository : IRepository
    {
        private readonly DatabaseSettings _databaseSettings;
        private readonly ILogger _logger;

        public Repository(DatabaseSettings databaseSettings, ILogger logger=null)
        {
            _databaseSettings = databaseSettings;
            _logger = logger;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_databaseSettings.ConnectionString)
            {
                DataSource = _databaseSettings.DbServer,
                Password = _databaseSettings.DbPassword,
                UserID = _databaseSettings.DbUsername
            };
            _databaseSettings.ConnectionString = builder.ToString();
        }

        public  Result<bool> InsertLog(Log log)
        {
            try
            {
                using DbConnection connection = new SqlConnection(_databaseSettings.ConnectionString);      
                connection.Open();
                using DbTransaction transaction = connection.BeginTransaction();
                var logDTO = log.GetDTO();
                try
                {
                    var id = connection.InsertAsync(logDTO, transaction: transaction).Result;
                    var testsDTOs = log.TESTS.Select(t => { var dto = t.GetDTO();dto.LogId = id;return dto; });
                    connection.Insert(testsDTOs, transaction: transaction);
                    
                    transaction.Commit();
                    return new Result<bool>(id > 0, id > 0);
                }

                //In case transaction needs to be rolled back
                catch (Exception e) 
                {
                    transaction.Rollback();
                    Console.WriteLine(e.Message);
                    return new Result<bool>(false, false,e.Message);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return new Result<bool>(false, false, e.Message);
            }
        }
    }
}
