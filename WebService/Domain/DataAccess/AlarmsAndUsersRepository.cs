using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebService.Domain.Business.Alarms;
using WebService.Domain.DataAccess.DTO;
using WebService.Utils;
using WebService.Utils.Models;

namespace WebService.Domain.DataAccess
{
    public interface IAlarmsAndUsersRepository
    {
        public Task<Result<List<Alarm>>> GetAllActiveAlarms();
        public Task<Result<List<Alarm>>> GetAllAlarms();
        public Task<Result<Alarm>> InsertAlarm(Alarm alarm);
        public Task<Result<Alarm>> UpdateAlarm(Alarm alarm);
        public Task<Result<bool>> DeleteAlarm(int Id);
        public Task<Result<UserDTO>> GetUserRecord(string email);
        public Task<Result<bool>> UpdateUser(UserDTO record);
        public Task<Result<bool>> AddUser(UserDTO record);
        public Task<Result<bool>> RemoveUser(string email);
        public Task<Result<List<UserDTO>>> GetAllUsers();
        public Task<Result<bool>> UpdateOrInsert(UserDTO user);

    }
    public class AlarmsAndUsersRepository : IAlarmsAndUsersRepository
    {
        //Properties
        DatabaseSettings DatabaseSettings { get; }
        ILogger Logger { get; }

        //Constructor
        public AlarmsAndUsersRepository(IOptions<DatabaseSettings> databaseSettings, ILogger logger)
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

        #region Alarm
        /// <summary>
        /// Query the database for all active alarms.
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<Alarm>>> GetAllActiveAlarms()
        {
            var sqlCommand = @"select * from Alarms where Active=1";
            return await GetAlarmsQuery(sqlCommand);
        }
        /// <summary>
        /// Query the database for all alarms
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<Alarm>>> GetAllAlarms()
        {
            var sqlCommand = @"select * from Alarms";
            return await GetAlarmsQuery(sqlCommand);
        }
        /// <summary>
        /// Util function to query the databse by generic sql command and parameters
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        private async Task<Result<List<Alarm>>> GetAlarmsQuery(string sqlCommand, [Optional] object queryParams)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var dtos = queryParams == null ? await connection.QueryAsync<AlarmDTO>(sqlCommand) : await connection.QueryAsync<AlarmDTO>(sqlCommand, queryParams);
                var alarms = dtos.Select(x =>
                {
                    var dto = Alarm.ConstructFromDTO(x);
                    if (!dto.Status) { Logger.Warning($"An error occurred while attempting to construct the Alarm object from DTO. {dto.Message}"); }
                    return dto;
                }).Where(x => x.Status == true).Select(x => x.Data);
                return new Result<List<Alarm>>(true, alarms.ToList());
            }
            catch (Exception e)
            {
                Logger.Warning("A database error occurred while retrieving all the alarms from the database", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<List<Alarm>>(false, null, "There was a problem with the DataBase");
            }
        }
        /// <summary>
        /// Inserting new alarm record to the database
        /// </summary>
        /// <param name="alarm"></param>
        /// <returns></returns>
        public async Task<Result<Alarm>> InsertAlarm(Alarm alarm)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var id = await connection.InsertAsync(alarm.GetDTO());
                alarm.Id = id;
                return new Result<Alarm>(true, alarm);
            }
            catch (Exception e)
            {
                Logger.Warning($"A database error occurred while inserting alarm (id = {alarm.Id})", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<Alarm>(false, null, "There was a problem with the DataBase");
            }
        }
        /// <summary>
        /// Updating exisitng alarm record in the database
        /// </summary>
        /// <param name="alarm"></param>
        /// <returns></returns>
        public async Task<Result<Alarm>> UpdateAlarm(Alarm alarm)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var returnVal = await connection.UpdateAsync(alarm.GetDTO(true)) ? new Result<Alarm>(true, alarm) : new Result<Alarm>(false, alarm, $"There was a problem during the update of alarm {alarm.Id}");
                return returnVal;
            }
            catch (Exception e)
            {
                Logger.Warning($"A database error occurred while updating alarm (id = {alarm.Id})", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<Alarm>(false, null, "There was a problem with the DataBase");
            }
        }
        /// <summary>
        /// Deleting alarm record by its Id from the database
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<Result<bool>> DeleteAlarm(int Id)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var returnVal = await connection.DeleteAsync(new AlarmDTO() { Id = Id }) ? new Result<bool>(true, true) : new Result<bool>(false, false, $"There was a problem during the deletion of alarm {Id}");
                return returnVal;
            }
            catch (Exception e)
            {
                Logger.Warning($"A database error occurred while deleting alarm (id = {Id})", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<bool>(false, false, "There was a problem with the DataBase");
            }
        }
        #endregion

        #region Authentication 
        /// <summary>
        /// Qurey the databse for the user record by Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Result<UserDTO>> GetUserRecord(string email)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var user_record = await connection.GetAsync<UserDTO>(email);
                return user_record != default ? new Result<UserDTO>(true, user_record) : new Result<UserDTO>(false, null, $"User with email {email} was not found in data base");
            }
            catch (Exception e)
            {
                Logger.Warning($"A database error occurred while retreving user {email}", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<UserDTO>(false, null, "There was a problem with the DataBase");
            }
        }
        /// <summary>
        /// Updating user record in the database
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task<Result<bool>> UpdateUser(UserDTO record)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var returnVal = await connection.UpdateAsync(record) ? new Result<bool>(true, true) : new Result<bool>(false, false, $"User with email {record.Email} was not found in data base");
                return returnVal;
            }
            catch (Exception e)
            {
                Logger.Warning($"A database error occurred while updating user {record.Email}", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<bool>(false, false, "There was a problem with the DataBase");
            }
        }
        /// <summary>
        /// Removing user record by it email from the database
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Result<bool>> RemoveUser(string email)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var returnVal = await connection.DeleteAsync(new UserDTO { Email = email }) ? new Result<bool>(true, true) : new Result<bool>(false, false, $"User with email {email} was not found in data base");
                return returnVal;
            }
            catch (Exception e)
            {
                Logger.Warning($"A database error occurred while deleting user {email}", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<bool>(false, false, "There was a problem with the DataBase");
            }
        }
        /// <summary>
        /// Inserting new user record to the database if not exists.
        /// In case the user already exisits and result object indication failure will returns.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Result<bool>> AddUser(UserDTO user)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                using var transaction = await connection.BeginTransactionAsync();
                var userRecord = await connection.GetAsync<UserDTO>(user.Email, transaction);
                var status = false;
                if (userRecord == default)
                {
                    await connection.InsertAsync(user, transaction);
                    status = true;
                }
                await transaction.CommitAsync();
                return new Result<bool>(status, status, status ? "Succeed" : "User already exists");
            }
            catch (Exception e)
            {
                Logger.Warning($"A database error occurred while adding the user {user.Email}", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<bool>(false, false, "There was a problem with the DataBase");
            }
        }
        /// <summary>
        /// Query the database for all the listed users
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<UserDTO>>> GetAllUsers()
        {
            var sqlCommand = @"select * from Users";
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var users_dtos = await connection.QueryAsync<UserDTO>(sqlCommand);
                return new Result<List<UserDTO>>(true, users_dtos.ToList());
            }
            catch (Exception e)
            {
                Logger.Warning($"A database error occurred while retrieving all the users from the database", e, new Dictionary<LogEntry, string>() { { LogEntry.Component, GetType().Name } });
                return new Result<List<UserDTO>>(false, null, "There was a problem with the DataBase");
            }
        }
        /// <summary>
        /// Util function which use only to insert or update the defualt system admin on every system initialization event
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Result<bool>> UpdateOrInsert(UserDTO user)
        {
            var insertResult = await AddUser(user);
            if (!insertResult.Status)
            {
                return await UpdateUser(user);
            }
            else
            {
                return insertResult;
            }
        }
        #endregion
    }
}
