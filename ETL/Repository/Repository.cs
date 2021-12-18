using Dapper.Contrib.Extensions;
using ETL.DataObjects;
using ETL.Repository.DTO;
using ETL.Utils;
using ETL.Utils.Models;
using Microsoft.Extensions.Options;
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

        public Repository(IOptions<DatabaseSettings> databaseSettings, ILogger logger)
        {
            _databaseSettings = databaseSettings.Value;
            _logger = logger;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_databaseSettings.ConnectionString)
            {
                DataSource = _databaseSettings.DbServer,
                Password = _databaseSettings.DbPassword,
                UserID = _databaseSettings.DbUsername
            };
            _databaseSettings.ConnectionString = builder.ToString();
        }

        /// <summary>
        /// Insert log in transaction. Preform rollback in case an error has been occurred
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public Result<bool> InsertLog(Log log)
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
                    var testsDTOs = log.TESTS.Select(t => { var dto = t.GetDTO(); dto.LogId = id; return dto; });
                    connection.Insert(testsDTOs, transaction: transaction);

                    transaction.Commit();
                    return new Result<bool>(id > 0, id > 0);
                }

                //In case transaction needs to be rolled back
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.Warning($"Error raise in transaction - preforming rollback", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                    return new Result<bool>(false, false, e.Message);
                }
            }
            catch (Exception e)
            {
                _logger.Warning($"Error communication with DB", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<bool>(false, false, e.Message);
            }
        }
    }
}
