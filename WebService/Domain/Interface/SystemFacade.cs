using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebService.API.Controllers.Models;
using WebService.Domain.Business.Alarms;
using WebService.Domain.Business.Queries;
using WebService.Utils;

namespace WebService.Domain.Interface
{
    public class SystemFacade
    {
        QueriesController QueriesController { get; }
        AlarmsController AlarmsController { get; }
        public SystemFacade(QueriesController queriesController, AlarmsController alarmsController)
        {
            QueriesController = queriesController;
            AlarmsController = alarmsController;
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

        public async Task CheckAlarmsCondition()
        {
            AlarmsController.CheckForAlarmsCondition();
        }

        public async Task<Result<QueryResult>> CalculateTesterLoad(DateTime startDate, DateTime endDate)
        {
            return await QueriesController.CalculateTesterLoad(startDate, endDate);
        }
        #endregion

        public async Task<Result<AlarmModel>> AddNewAlarm(string name, Field field, string objective, int threshold, List<string> receivers)
        {
            var alarmResult = await AlarmsController.AddNewAlarm(name, field, objective, threshold, receivers);
            if (alarmResult.Status)
            {
                var model = new AlarmModel()
                {
                    Id = alarmResult.Data.Id,
                    Name = alarmResult.Data.Name,
                    Objective = alarmResult.Data.Objective,
                    Field = alarmResult.Data.Field,
                    Threshold = alarmResult.Data.Threshold,
                    Active = alarmResult.Data.Active,
                    Receivers = alarmResult.Data.Receivers
                };
                return new Result<AlarmModel>(true, model);
            }
            return new Result<AlarmModel>(alarmResult.Status, null, alarmResult.Message);

        }

    }
}
