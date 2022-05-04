using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Authentication;
using WebService.Domain.Business.Queries;
using WebService.Utils;

namespace WebService.Domain.Interface
{
    public class SystemFacade
    {
        QueriesController QueriesController { get; }
        AlarmsController AlarmsController { get; }
        AuthenticationController AuthenticationController { get; }
        public SystemFacade(QueriesController queriesController, AlarmsController alarmsController, AuthenticationController authenticationController)
        {
            QueriesController = queriesController;
            AlarmsController = alarmsController;
            AuthenticationController = authenticationController;
        }

        #region Queries
        public async Task<Result<QueryResult>> CalculateBoundaries(string catalog, DateTime startDate, DateTime endDate)
        {
            return await QueriesController.CalculateBoundaries(catalog, startDate, endDate);
        }
        public async Task<Result<QueryResult>> CalculateCardYield(string catalog, DateTime startDate, DateTime endDate)
        {
            return await QueriesController.CalculateCardYield(catalog, startDate, endDate);
        }

        public async Task<Result<QueryResult>> CalculateStationsYield(DateTime startDate, DateTime endDate)
        {
            return await QueriesController.CalculateStationsYield(startDate, endDate);
        }
        public async Task<Result<QueryResult>> CalculateStationAndCardYield(string station, string catalog, DateTime startDate, DateTime endDate)
        {
            return await QueriesController.CalculateStationAndCardYield(station, catalog, startDate, endDate);
        }

        public async Task<Result<QueryResult>> CalculateNFF(string cardName, DateTime startDate, DateTime endDate, int timeInterval)
        {
            return await QueriesController.CalculateNFF(cardName, startDate, endDate, timeInterval);
        }

        public async Task<Result<QueryResult>> CalculateTesterLoad(DateTime startDate, DateTime endDate)
        {
            return await QueriesController.CalculateTesterLoad(startDate, endDate);
        }

        public Task<Result<QueryResult>> CalculateCardTestDuration(string catalog, DateTime startDate, DateTime endDate)
        {
            return QueriesController.CalculateCardTestDuration(catalog, startDate, endDate);
        }

        #endregion

        #region Alarms

        public async Task<Result<FullAlarmModel>> AddNewAlarm(string name, Field field, string objective, int threshold, List<string> receivers)
        {
            var alarmResult = await AlarmsController.AddNewAlarm(name, field, objective, threshold, receivers);
            if (alarmResult.Status)
            {
                var model = new FullAlarmModel()
                {
                    Id = alarmResult.Data.Id,
                    Name = alarmResult.Data.Name,
                    Objective = alarmResult.Data.Objective,
                    Field = alarmResult.Data.Field,
                    Threshold = alarmResult.Data.Threshold,
                    Active = alarmResult.Data.Active,
                    Receivers = alarmResult.Data.Receivers
                };
                return new Result<FullAlarmModel>(true, model);
            }
            return new Result<FullAlarmModel>(alarmResult.Status, null, alarmResult.Message);

        }

        public async Task<Result<FullAlarmModel>> EditAlarm(int id, string name, Field field, string objective, int threshold, bool active, List<string> receivers)
        {
            var editAlarmResult = await AlarmsController.EditAlarm(new Alarm(id, name, field, objective, threshold, active, receivers));
            if (editAlarmResult.Status)
            {
                var model = new FullAlarmModel()
                {
                    Id = editAlarmResult.Data.Id,
                    Name = editAlarmResult.Data.Name,
                    Objective = editAlarmResult.Data.Objective,
                    Field = editAlarmResult.Data.Field,
                    Threshold = editAlarmResult.Data.Threshold,
                    Active = editAlarmResult.Data.Active,
                    Receivers = editAlarmResult.Data.Receivers
                };
                return new Result<FullAlarmModel>(true, model);
            }
            return new Result<FullAlarmModel>(editAlarmResult.Status, null, editAlarmResult.Message);
        }
        public async Task<Result<FullAlarmModel>> RemoveAlarm(int id, string name, Field field, string objective, int threshold, bool active, List<string> receivers)
        {
            var editAlarmResult = await AlarmsController.RemoveAlarm(new Alarm(id, name, field, objective, threshold, active, receivers));
            if (editAlarmResult.Status)
            {
                var model = new FullAlarmModel()
                {
                    Id = editAlarmResult.Data.Id,
                    Name = editAlarmResult.Data.Name,
                    Objective = editAlarmResult.Data.Objective,
                    Field = editAlarmResult.Data.Field,
                    Threshold = editAlarmResult.Data.Threshold,
                    Active = editAlarmResult.Data.Active,
                    Receivers = editAlarmResult.Data.Receivers
                };
                return new Result<FullAlarmModel>(true, model);
            }
            return new Result<FullAlarmModel>(editAlarmResult.Status, null, editAlarmResult.Message);
        }

        public async Task CheckAlarmsCondition()
        {
            await AlarmsController.CheckForAlarmsCondition();
        }

        #endregion

        #region Authentication

        public async Task RequestVerificationCode(string email)
        {
            await AuthenticationController.RequestVerificationCode(email);
        }
        public async Task<Result<JWTtoken>> Login(string email, string password)
        {
            return await AuthenticationController.Login(email, password);
        }

        public async Task<Result<bool>> AddUser(string email)
        {
            return await AuthenticationController.AddUser(email);
        }

        public async Task<Result<bool>> RemoveUser(string email)
        {
            return await AuthenticationController.RemoveUser(email);
        }



        #endregion

    }
}
