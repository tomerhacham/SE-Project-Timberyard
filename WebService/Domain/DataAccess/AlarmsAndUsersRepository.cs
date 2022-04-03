﻿using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebService.Domain.Business.Alarms;
using WebService.Domain.DataAccess.DTO;
using WebService.Utils;
using WebService.Utils.Models;

namespace WebService.Domain.DataAccess
{
    public class AlarmsAndUsersRepository
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

        public async Task<Result<List<Alarm>>> GetAllAlarms() 
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var dtos = await connection.GetAllAsync<AlarmDTO>();
                var alarms = dtos.Select(x => Alarm.ConstructFromDTO(x)).Where(x=>x.Status==true).Select(x=>x.Data);
                return new Result<List<Alarm>>(true, alarms.ToList());
            }
            catch (Exception e)
            {
                return new Result<List<Alarm>>(false, null, "There was a problem with the DataBase");
            }
        }
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
                return new Result<Alarm>(false, null, "There was a problem with the DataBase");
            }
        }
        public async Task<Result<Alarm>> UpdateAlarm(Alarm alarm)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var returnVal = await connection.UpdateAsync(alarm.GetDTO()) ? new Result<Alarm>(true, alarm) : new Result<Alarm>(false,alarm, $"There was a problem during the update of alarm {alarm.Id}")  ;
                return returnVal;
            }
            catch (Exception e)
            {
                return new Result<Alarm>(false, null, "There was a problem with the DataBase");
            }
        }
        public async Task<Result<Alarm>> DeleteAlarm(Alarm alarm)
        {
            try
            {
                using var connection = new SqlConnection(DatabaseSettings.ConnectionString);
                await connection.OpenAsync();
                var returnVal = await connection.DeleteAsync(alarm.GetDTO()) ? new Result<Alarm>(true, alarm) : new Result<Alarm>(false, alarm, $"There was a problem during the deletion of alarm {alarm.Id}");
                return returnVal;
            }
            catch (Exception e)
            {
                return new Result<Alarm>(false, null, "There was a problem with the DataBase");
            }
        }
    }
}
